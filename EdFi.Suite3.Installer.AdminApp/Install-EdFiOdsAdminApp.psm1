# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5

$ErrorActionPreference = "Stop"

<#
To run manually from source code, instead of from an expanded NuGet package,
run the prep-installer-package.ps1 script first. Think of it as a "restore-packages"
step before compiling in C#.
#>
function Set-TlsVersion {

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12 -bor [Net.SecurityProtocolType]::Tls13
}

Import-Module "$PSScriptRoot/key-management.psm1"

$appCommonDirectory = "$PSScriptRoot/AppCommon"
$RequiredDotNetHostingBundleVersion = "6.0.0"

Import-Module -Force "$appCommonDirectory/Environment/Prerequisites.psm1" -Scope Global
Set-TlsVersion
Install-DotNetCore "C:\temp\tools"

Import-Module -Force "$appCommonDirectory/Utility/hashtable.psm1" -Scope Global
Import-Module -Force "$appCommonDirectory/Utility/nuget-helper.psm1"
Import-Module -Force "$appCommonDirectory/Utility/TaskHelper.psm1"
Import-Module -Force "$appCommonDirectory/Utility/ToolsHelper.psm1"

# Import the following with global scope so that they are available inside of script blocks
Import-Module -Force "$appCommonDirectory/Application/Install.psm1" -Scope Global
Import-Module -Force "$appCommonDirectory/Application/Uninstall.psm1" -Scope Global
Import-Module -Force "$appCommonDirectory/Application/Configuration.psm1" -Scope Global

$DbDeployVersion = "2.3.10068"

function Install-EdFiOdsAdminApp {
    <#
    .SYNOPSIS
        Installs the Ed-Fi ODS/API AdminApp application into IIS.

    .DESCRIPTION
        Installs and configures the Ed-Fi ODS/API AdminApp application in IIS running in Windows 10 or
        Windows Server 2016+. As needed, will create a new "Ed-Fi" website in IIS, configure it
        for HTTPS, and load the Admin App binaries as an an application. Transforms the web.config
        and invokes dbup migrations.
    .EXAMPLE
        PS c:\> $dbConnectionInfo = @{
            Server = "(local)"
            Engine = "SqlServer"
            UseIntegratedSecurity=$true
        }
        PS c:\> $parameters = @{
            ToolsPath = "C:/temp/tools"
            DbConnectionInfo = $dbConnectionInfo
            OdsDatabaseName = "EdFi_Ods_Sandbox"
            OdsApiUrl = "http://example-web-api.com/WebApi"
        }
        PS c:\> Install-EdFiOdsAdminApp @parameters

        Installs Admin App to SQL Server with mainly defaults, except for the custom Sandbox ODS
        and the required ODS/API URL.

    .EXAMPLE
        PS c:\> $parameters = @{
            ToolsPath = "C:/temp/tools"
            OdsApiUrl = "http://example-web-api.com/WebApi"
            AdminDbConnectionInfo = @{
                Engine="SqlServer"
                Server="edfi-auth.my-sql-server.example"
                UseIntegratedSecurity=$true
            }
            OdsDbConnectionInfo = @{
                DatabaseName="EdFi_ODS_Staging"
                Engine="SqlServer"
                Server="edfi-stage.my-sql-server.example"
                Username="ods-write"
                Password="@#$%^&*(GHJ%^&*YUKSDF"
            }
            SecurityDbConnectionInfo = @{
                Engine="SqlServer"
                Server="edfi-auth.my-sql-server.example"
                UseIntegratedSecurity=$true
            }
        }
        PS c:\> Install-EdFiOdsAdminApp @parameters

        Installs Admin App to multiple SQL Servers for each of the databases with some
        using integrated security and some using username and password credentials,
        along with a custom staging ODS name and the required ODS/API URL.

    .EXAMPLE
        PS c:\> $dbConnectionInfo = @{
            Server = "localhost"
            Engine = "PostgreSQL"
            Username="install-user"
            Password="@#$%^&*(GHJ%^&*YUKSDF"
        }
        PS c:\> $parameters = @{
            ToolsPath = "C:/temp/tools"
            DbConnectionInfo = $dbConnectionInfo
            InstallCredentialsUseIntegratedSecurity = $true
            OdsApiUrl = "http://example-web-api.com/WebApi"
        }
        PS c:\> Install-EdFiOdsAdminApp @parameters

        Installs Admin App to PostgreSQL Server with integrated security install credentials and
        username and password application credentials, along with the required ODS/API URL.

    .EXAMPLE
        PS c:\> $dbConnectionInfo = @{
            Server = "(local)"
            Engine = "SqlServer"
            UseIntegratedSecurity=$true
        }
        PS c:\> $parameters = @{
            ToolsPath = "C:/temp/tools"
            DbConnectionInfo = $dbConnectionInfo
            OdsApiUrl = "http://example-web-api.com/WebApi"
            AdminAppFeatures = @{
                ApiMode="yearspecific"
                SecurityMetadataCacheTimeoutMinutes="5"
            }
        }
        PS c:\> Install-EdFiOdsAdminApp @parameters

        Installs Admin App to SQL Server in Year Specific mode for 2020. The installer will also
        install Admin App in ASP.NET Identity mode, rather than AD Authentication, and will set the
        SecurityMetadataCacheTimeoutMinutes to 5 minutes.
    #>
    [CmdletBinding()]
    param (
        # NuGet package name. Default: EdFi.ODS.AdminApp.Web.
        [string]
        $PackageName = "EdFi.Suite3.ODS.AdminApp.Web",

        # NuGet package version. If not set, will retrieve the latest full release package.
        [string]
        $PackageVersion,

        # NuGet package source. Defaults to "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json".
        [string]
        $PackageSource = "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json",

        # Path for storing installation tools, e.g. nuget.exe. Default: "C:\temp\tools".
        [string]
        $ToolsPath = "C:\temp\tools",

        # Path for storing downloaded packages
        [string]
        $DownloadPath = "C:\temp\downloads",

        # Path for the IIS WebSite. Default: c:\inetpub\Ed-Fi.
        [string]
        $WebSitePath = "c:\inetpub\Ed-Fi",

        # Web site name. Default: "Ed-Fi".
        [string]
        $WebsiteName = "Ed-Fi",

        # Web site port number. Default: 443.
        [int]
        $WebSitePort = 443,

        # Web application name. Default: "AdminApp".
        [string]
        $WebApplicationName = "AdminApp",

        # TLS certificiate thumbprint, optional. When not set, a self-signed certificate will be created.
        [string]
        $CertThumbprint,

        # Full URL to the Ed-Fi ODS / API version endpoint.
        [string]
        [Parameter(Mandatory=$true)]
        $OdsApiUrl,

        # Install Credentials: User
        [string]
        $InstallCredentialsUser,

        # Install Credentials: Password
        [string]
        $InstallCredentialsPassword,

        # Install Credentials: UseIntegratedSecurity setting
        [switch]
        $InstallCredentialsUseIntegratedSecurity,

        # Admin Database Name
        [string]
        [Parameter(ParameterSetName="SharedCredentials")]
        $AdminDatabaseName = "EdFi_Admin",

        # ODS Database Name
        [string]
        [Parameter(ParameterSetName="SharedCredentials")]
        $OdsDatabaseName = "EdFi_Ods",

        # Security Database Name
        [string]
        [Parameter(ParameterSetName="SharedCredentials")]
        $SecurityDatabaseName = "EdFi_Security",

        # Shared database connectivity information.
        #
        # The hashtable must include: Server, Engine (SqlServer or PostgreSQL), and
        # either UseIntegratedSecurity or Username and Password (Password can be skipped
        # for PostgreSQL when using pgconf file). Optionally can include Port.
        [hashtable]
        [Parameter(Mandatory=$true, ParameterSetName="SharedCredentials")]
        $DbConnectionInfo,

        # Database connectivity only for the admin database.
        #
        # The hashtable must include: Server, Engine (SqlServer or PostgreSQL), and
        # either UseIntegratedSecurity or Username and Password (Password can be skipped
        # for PostgreSQL when using pgconf file). Optionally can include Port and
        # DatabaseName.
        [hashtable]
        [Parameter(Mandatory=$true, ParameterSetName="SeparateCredentials")]
        $AdminDbConnectionInfo,

        # Database connectivity only for the ODS database.
        #
        # The hashtable must include: Server, Engine (SqlServer or PostgreSQL), and
        # either UseIntegratedSecurity or Username and Password (Password can be skipped
        # for PostgreSQL when using pgconf file). Optionally can include Port and
        # DatabaseName.
        [hashtable]
        [Parameter(Mandatory=$true, ParameterSetName="SeparateCredentials")]
        $OdsDbConnectionInfo,

        # Database connectivity only for the security database.
        #
        # The hashtable must include: Server, Engine (SqlServer or PostgreSQL), and
        # either UseIntegratedSecurity or Username and Password (Password can be skipped
        # for PostgreSQL when using pgconf file). Optionally can include Port and
        # DatabaseName.
        [hashtable]
        [Parameter(Mandatory=$true, ParameterSetName="SeparateCredentials")]
        $SecurityDbConnectionInfo,

        # Optional overrides for features and settings in the web.config.
        #
        # The hashtable can include: ApiMode and SecurityMetadataCacheTimeoutMinutes. By
        # default, AdminApp is installed in Shared
        # Instance mode with AD Authentication and a 10 minute security metadata cache timeout.
        [hashtable]
        $AdminAppFeatures,

        # Database Config
        [switch]
        $NoDuration,

        # App startup will decide admin app startup type. Valid values are OnPrem, Azure. Installer always used in
        # OnPrem mode. So, the default value set to OnPrem
        [ValidateSet("OnPrem", "Azure")]
        [String]
        $AppStartUp = "OnPrem"
    )

    Write-InvocationInfo $MyInvocation

    Clear-Error

    $result = @()

    $Config = @{
        WebApplicationPath = (Join-Path $WebSitePath $WebApplicationName)
        PackageName = $PackageName
        PackageVersion = $PackageVersion
        PackageSource = $PackageSource
        ToolsPath = $ToolsPath
        DownloadPath = $DownloadPath
        WebSitePath = $WebSitePath
        WebSiteName = $WebsiteName
        WebSitePort = $WebsitePort
        CertThumbprint = $CertThumbprint
        WebApplicationName = $WebApplicationName
        OdsApiUrl = $OdsApiUrl
        DatabaseInstallCredentials = @{
            DatabaseUser = $InstallCredentialsUser
            DatabasePassword = $InstallCredentialsPassword
            UseIntegratedSecurity = $InstallCredentialsUseIntegratedSecurity
        }
        AdminDatabaseName = $AdminDatabaseName
        OdsDatabaseName = $OdsDatabaseName
        SecurityDatabaseName = $SecurityDatabaseName
        DbConnectionInfo = $DbConnectionInfo
        AdminDbConnectionInfo = $AdminDbConnectionInfo
        OdsDbConnectionInfo = $OdsDbConnectionInfo
        SecurityDbConnectionInfo = $SecurityDbConnectionInfo
        AdminAppFeatures = $AdminAppFeatures
        NoDuration = $NoDuration
        AppStartUp = $AppStartUp
    }

    $elapsed = Use-StopWatch {
        $result += Invoke-InstallationPreCheck -Config $Config
        $result += Initialize-Configuration -Config $config
        $result += Get-AdminAppPackage -Config $Config
        $result += Get-DbDeploy -Config $Config
        $result += Invoke-TransformAppSettings -Config $Config
        $result += Invoke-TransformConnectionStrings -Config $config
        $result += Install-Application -Config $Config
        $result += Set-SqlLogins -Config $Config
        $result += Invoke-DbUpScripts -Config $Config

        $result
    }

    Test-Error

    if (-not $NoDuration) {
        $result += New-TaskResult -name "-" -duration "-"
        $result += New-TaskResult -name $MyInvocation.MyCommand.Name -duration $elapsed.format
        $result | Format-Table
    }
}

function Update-EdFiOdsAdminApp {
    <#
    .SYNOPSIS
        Upgrade the Ed-Fi ODS/API AdminApp application in IIS.

    .DESCRIPTION
        Upgrades and configures the Ed-Fi ODS/API AdminApp application in IIS running in Windows 10 or
        Windows Server 2016+. Admin App will be upgraded and reside under "Ed-Fi" website in IIS.
        Appsettings values and connection strings will be copied over from existing Admin App application.
        Invokes dbup migrations for updating the EdFi_Admin database accordingly.
    .EXAMPLE
        PS c:\> $parameters = @{
            PackageVersion = '2.2.1'
        }
        PS c:\> Upgrade-EdFiOdsAdminApp @parameters

        Upgrades Admin App to specified version and preseve the appsettings values and connection strings from existing application.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    param (
        # NuGet package name. Default: EdFi.ODS.AdminApp.Web.
        [string]
        $PackageName = "EdFi.Suite3.ODS.AdminApp.Web",

        # NuGet package version. If not set, will retrieve the latest full release package.
        [string]
        $PackageVersion,

        # NuGet package source. Defaults to "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json".
        [string]
        $PackageSource = "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json",

        # Path for storing installation tools, e.g. nuget.exe. Default: "C:\temp\tools".
        [string]
        $ToolsPath = "C:\temp\tools",

        # Path for storing downloaded packages
        [string]
        $DownloadPath = "C:\temp\downloads",

        # Path for the IIS WebSite. Default: c:\inetpub\Ed-Fi.
        [string]
        $WebSitePath = "c:\inetpub\Ed-Fi",

        # Web site name. Default: "Ed-Fi".
        [string]
        $WebsiteName = "Ed-Fi",

        # Web site port number. Default: 443.
        [int]
        $WebSitePort = 443,

        # Web application name. Default: "AdminApp".
        [string]
        $WebApplicationName = "AdminApp",

        # TLS certificiate thumbprint, optional. When not set, a self-signed certificate will be created.
        [string]
        $CertThumbprint,

        # Install Credentials: User
        [Parameter(ParameterSetName="InstallCredentials")]
        [string]
        $InstallCredentialsUser,

        # Install Credentials: Password
        [Parameter(ParameterSetName="InstallCredentials")]
        [string]
        $InstallCredentialsPassword,

        # Install Credentials: UseIntegratedSecurity setting
        [Parameter(ParameterSetName="InstallCredentials")]
        [switch]
        $InstallCredentialsUseIntegratedSecurity,

        # The hashtable must include: Server, Engine (SqlServer or PostgreSQL), and
        # either UseIntegratedSecurity or Username and Password (Password can be skipped
        # for PostgreSQL when using pgconf file). Optionally can include Port and
        # DatabaseName.
        [hashtable]
        [Parameter(Mandatory=$true, ParameterSetName="InstallCredentials")]
        $AdminDbConnectionInfo,

        # Database Config
        [switch]
        $NoDuration
    )

    Write-InvocationInfo $MyInvocation

    Clear-Error

    $result = @()

    $Config = @{
        WebApplicationPath = (Join-Path $WebSitePath $WebApplicationName)
        PackageName = $PackageName
        PackageVersion = $PackageVersion
        PackageSource = $PackageSource
        DatabaseInstallCredentials = @{
            DatabaseUser = $InstallCredentialsUser
            DatabasePassword = $InstallCredentialsPassword
            UseIntegratedSecurity = $InstallCredentialsUseIntegratedSecurity
        }
        ToolsPath = $ToolsPath
        DownloadPath = $DownloadPath
        WebSitePath = $WebSitePath
        WebSiteName = $WebsiteName
        WebSitePort = $WebsitePort
        CertThumbprint = $CertThumbprint
        WebApplicationName = $WebApplicationName
        NoDuration = $NoDuration
        AppStartUp = $AppStartUp
        ApplicationInstallType = "Upgrade"
        AdminDbConnectionInfo = $AdminDbConnectionInfo
    }

    $elapsed = Use-StopWatch {
        $result += Invoke-ResetIIS
        $result += Invoke-ApplicationUpgrade -Config $Config
        $result += Get-AdminAppPackage -Config $Config
        $result += Get-DbDeploy -Config $Config
        $result += Invoke-TransferAppsettings -Config $Config
        $result += Invoke-TransferConnectionStrings -Config $Config
        $result += Install-Application -Config $Config
        $result += Invoke-DbUpScripts -Config $Config
        $result += Invoke-StartWebSite $Config.WebSiteName $Config.WebSitePort

        $result
    }

    Test-Error

    if (-not $NoDuration) {
        $result += New-TaskResult -name "-" -duration "-"
        $result += New-TaskResult -name $MyInvocation.MyCommand.Name -duration $elapsed.format
        $result | Format-Table
    }
}


function Uninstall-EdFiOdsAdminApp {
    <#
    .SYNOPSIS
        Removes the Ed-Fi ODS/API AdminApp web application from IIS.
    .DESCRIPTION
        Removes the Ed-Fi ODS/API AdminApp web application from IIS, including its application
        pool (if not used for any other application). Removes the web site as well if
        there are no remaining applications, and the site's app pool.

        Does not remove IIS or the URL Rewrite module.

    .EXAMPLE
        PS c:\> Uninstall-EdFiOdsAdminApp

        Uninstall using all default values.
    .EXAMPLE
        PS c:\> $p = @{
            WebSiteName="Ed-Fi-3"
            WebApplicationPath="c:/inetpub/Ed-Fi-3/AdminApp-3"
            WebApplicationName = "AdminApp-3"
        }
        PS c:\> Uninstall-EdFiOdsAdminApp @p

        Uninstall when the web application and web site were setup with non-default values.
    #>
    [CmdletBinding()]
    param (
        # Path for storing installation tools, e.g. nuget.exe. Default: "./tools".
        [string]
        $ToolsPath = "$PSScriptRoot/tools",

        # Path for the web application. Default: "c:\inetpub\Ed-Fi\AdminApp".
        [string]
        $WebApplicationPath = "C:\inetpub\Ed-Fi\AdminApp",

        # Web application name. Default: "AdminApp".
        [string]
        $WebApplicationName = "AdminApp",

        # Web site name. Default: "Ed-Fi".
        [string]
        $WebSiteName = "Ed-Fi",

        # Turns off display of script run-time duration.
        [switch]
        $NoDuration
    )

    $config = @{
        ToolsPath = $ToolsPath
        WebApplicationPath = $WebApplicationPath
        WebApplicationName = $WebApplicationName
        WebSiteName = $WebSiteName
    }

    $result = @()

    $elapsed = Use-StopWatch {

        Invoke-ResetIIS

        UninstallAdminApp $config

        $result
    }

    Test-Error

    if (-not $NoDuration) {
        $result += New-TaskResult -name "-" -duration "-"
        $result += New-TaskResult -name $MyInvocation.MyCommand.Name -duration $elapsed.format
        $result | Format-Table
    }
}

function UninstallAdminApp($config)
{
    $parameters = @{
        WebApplicationPath = $config.WebApplicationPath
        WebApplicationName = $config.WebApplicationName
        WebSiteName = $config.WebSiteName
    }

    Uninstall-EdFiApplicationFromIIS @parameters
}


function Invoke-InstallationPreCheck{
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $existingWebSiteName = $Config.WebsiteName
        $webSite = Get-Website | Where-Object { $_.name -eq $existingWebSiteName }
        $existingAdminAppApplication = get-webapplication $Config.WebApplicationName

        if($webSite -AND $existingAdminAppApplication)
        {
            $existingApplicationPath, $versionString = GetExistingAppVersion $webSite.PhysicalPath $existingAdminAppApplication
            $installVersionString = $Config.PackageVersion

            $targetIsNewer = IsVersionHigherThanOther $installVersionString $versionString
            $upgradeIsSupported = CheckVersionSupportsUpgrade $versionString

            if($targetIsNewer -and $upgradeIsSupported) {
                Write-Host "We found a preexisting Admin App $versionString installation. If you are seeking to upgrade to the new version, consider using the included upgrade script instead." -ForegroundColor Green
                Write-Host "Note: Using the upgrade script, all the appsettings and database connection string values would be copied forward from the existing installation, so only continue if you are you seeking to change the configuration." -ForegroundColor Yellow

                $confirmation = Request-Information -DefaultValue 'y' -Prompt "Please enter 'y' to continue the installation process, or enter 'n' to cancel the installation so that you can instead run the upgrade script"

                if(-not ($confirmation -ieq 'y')) {
                    Write-Host "Exiting."
                    exit
                }else {
                    $appsettingsFile =  Join-Path $existingApplicationPath "appsettings.json"
                    if(Test-Path -Path $appsettingsFile)
                    {
                        Write-Host "To ensure your existing ODS / API Key and Secret values will continue to work, your existing encryption key is being copied forward from the appsettings.json file at $appsettingsFile"
                        $appSettings = Get-Content $appsettingsFile | ConvertFrom-Json | ConvertTo-Hashtable
                        $Config.EncryptionKey = $appSettings.AppSettings.EncryptionKey
                    }
                }
            }elseif ($targetIsNewer) {
                Write-Warning "We found a preexisting Admin App $versionString installation. That version cannot be automatically upgraded in-place by this script. Please refer to https://techdocs.ed-fi.org/display/ADMIN/Upgrading+Admin+App+from+1.x+Line for setting up the newer version of AdminApp. Exiting."
                exit
            }else {
                Write-Warning "We found a preexisting Admin App $versionString installation newer than the target version $installVersionString. Downgrades are not supported. Please fully uninstall the existing Admin App first and retry. Exiting."
                exit
            }
        }
    }
}

function Invoke-ApplicationUpgrade {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )
    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {

        $existingWebSiteName = $Config.WebsiteName
        $webSite = Get-Website | Where-Object { $_.name -eq $existingWebSiteName }
        if($null -eq $webSite)
        {
            Write-Warning "Unable to find $existingWebSiteName on IIS."
            $customWebSiteName = Request-Information -DefaultValue "Ed-Fi" -Prompt "Ed-Fi applications are usually deployed in IIS underneath a 'Ed-Fi' website entry. If you previously installed with a custom name for that entry other than 'Ed-Fi', please enter that custom name"
            $customWebSite = Get-Website | Where-Object { $_.name -eq $customWebSiteName }
            if($null -eq $customWebSite)
            {
                throw "Unable to find $customWebSite on IIS. Please use install.ps1 for installing Ed-Fi website."
            }
            $webSite = $customWebSite
            $existingWebSiteName =  $customWebSiteName
        }
        $existingWebSitePath = ($webSite).PhysicalPath

        $existingAppName = $Config.WebApplicationName
        $existingAdminApp = get-webapplication $existingAppName
        if($null -eq $existingAdminApp)
        {
            Write-Warning "Unable to find $existingAppName on IIS."
            $customApplicationName = Request-Information -DefaultValue "AdminApp" -Prompt "If you previously installed AdminApp with a custom name, please enter that custom name"
            $customAdminAppApplication = get-webapplication $customApplicationName
            if($null -eq $customAdminAppApplication)
            {
                throw "Unable to find $customAdminAppApplication on IIS. Please use install.ps1 for installing AdminApp web application."
            }
            $existingAdminApp =  $customAdminAppApplication
            $existingAppName = $customApplicationName
        }

        $existingApplicationPath, $versionString = CheckForCompatibleUpdate $existingWebSitePath $existingAdminApp $Config.PackageVersion

        Write-Host "Stopping the $existingWebSiteName before taking application files back up."
        Stop-IISSite -Name $existingWebSiteName

        Write-Host "Taking back up on existing application folder $existingApplicationPath"
        $date = Get-Date -Format yyyy-MM-dd-hh-mm
        $backupApplicationFolderName = "$existingAppName-$versionString-$date"
        $basePath = (get-item $existingApplicationPath).parent.FullName
        $destinationBackupPath = "$basePath\$backupApplicationFolderName\"

        if(Test-Path -Path $destinationBackupPath)
        {
            Write-Warning "Back up folder already exists $destinationBackupPath."
            $overwriteConfirmation = Request-Information -DefaultValue 'y' -Prompt "Please enter 'y' to overwrite the content. Else enter 'n' to create new back up folder"
            if($overwriteConfirmation -ieq 'y')
            {
                Get-ChildItem -Path $destinationBackupPath -Force -Recurse | Sort-Object -Property FullName -Descending | Remove-Item
            }
            else {
                $newDirectory = Request-Information -DefaultValue "\" -Prompt "Please enter back up folder name"
                $destinationBackupPath = "$basePath\$newDirectory\"
                New-Item -ItemType directory -Path $destinationBackupPath
            }
        }
        else {
            New-Item -ItemType directory -Path $destinationBackupPath
        }

        Copy-Item -Path "$existingApplicationPath\*" -Destination $destinationBackupPath  -Recurse
        Write-Host "Completed application files back up on $destinationBackupPath"

        $Config.ApplicationBackupPath = $destinationBackupPath
        $Config.WebApplicationName = $existingAppName
        $Config.WebApplicationPath = $existingApplicationPath
        $Config.WebSiteName = $existingWebSiteName
        $Config.WebSitePath = $existingWebSitePath

        $parameters = @{
            WebApplicationPath = $existingApplicationPath
            WebApplicationName = $existingAppName
            WebSiteName = $existingWebSiteName
        }
        UninstallAdminApp $parameters
    }
}

function GetExistingAppVersion($webSitePath,  $existingAdminApp) {
    $existingApplicationPath = ($existingAdminApp).PhysicalPath
    if(!$existingApplicationPath)
    {
        # In case of existingApplicationPath is empty, then generating application physical path by combining site physical path and application path
        $appPath = $existingAdminApp.path.trimstart('/')
        $existingApplicationPath = "$webSitePath\$appPath"
    }

    $versionString = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$existingApplicationPath\EdFi.Ods.AdminApp.Web.exe").FileVersion

    return $existingApplicationPath, $versionString
}

function CheckVersionSupportsUpgrade($versionString) {
    $versionIsBeforeUpgradeSupport = IsVersionHigherThanOther '2.2' $versionString
    return -not $versionIsBeforeUpgradeSupport
}

function CheckForCompatibleUpdate($webSitePath,  $existingAdminApp, $targetVersionString) {
    $existingApplicationPath, $versionString = GetExistingAppVersion $webSitePath $existingAdminApp

    if(-not (CheckVersionSupportsUpgrade $versionString))
    {
        Write-Warning "Preexisting Admin App version $versionString cannot be automatically upgraded in-place by this script. Please refer to https://techdocs.ed-fi.org/display/ADMIN/Upgrading+Admin+App+from+1.x+Line for setting up the newer version of AdminApp."
        exit
    }

    $targetIsNewer = IsVersionHigherThanOther $targetVersionString $versionString

    if(-not $targetIsNewer)
    {
        Write-Warning "Upgrade version $targetVersionString is the same or lower than existing installation $versionString. Downgrades are not supported. Instead,  fully uninstall the existing Admin App and install the desired version."
        exit
    }

    return $existingApplicationPath, $versionString
}

function IsVersionHigherThanOther($versionString, $otherVersionString) {
    $version = ParseVersion($versionString)
    $otherVersion = ParseVersion($otherVersionString)

    $result = $version.CompareTo($otherVersion)
    return $result -gt 0
}

function ParseVersion($versionString) {
    $splitByTags = $versionString -split '-'
    $version = $splitByTags[0];

    for ($i = 1; $i -lt $splitByTags.Length; $i++) {
        $preVersion = $splitByTags[$i] -replace '[^0-9.]', ''
        $cleanedPreVersion = $preVersion.Trim('.')
        if($cleanedPreVersion -ne '') {
            $version += ".$cleanedPreVersion"
        }
    }

    try { return [System.Version]::Parse($version) }
    catch
    {
        Write-Warning "Failed to parse version configuration $versionString. Please correct and try again."
        exit
    }
}

function Invoke-TransferAppsettings {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {


        $backUpPath = $Config.ApplicationBackupPath
        Write-Warning "The following appsettings will be copied over from existing application: "
        $appSettings = @('ProductionApiUrl','SecurityMetadataCacheTimeoutMinutes','DatabaseEngine', 'AppStartup', 'EncryptionKey', 'ApiStartupType', 'ApiExternalUrl', 'XsdFolder', 'PathBase', 'Log4NetConfigFileName')
        foreach ($property in $appSettings) {
           Write-Host $property;
        }
        $oldSettingsFile =  Join-Path $backUpPath "appsettings.json"
        $oldSettings = Get-Content $oldSettingsFile | ConvertFrom-Json | ConvertTo-Hashtable

        $newSettingsFile = Join-Path $Config.WebConfigLocation "appsettings.json"
        $newSettings = Get-Content $newSettingsFile | ConvertFrom-Json | ConvertTo-Hashtable

        $newSettings.AppSettings.ProductionApiUrl = $oldSettings.AppSettings.ProductionApiUrl
        $newSettings.AppSettings.SecurityMetadataCacheTimeoutMinutes = $oldSettings.AppSettings.SecurityMetadataCacheTimeoutMinutes
        $newSettings.AppSettings.DatabaseEngine = $oldSettings.AppSettings.DatabaseEngine
        $newSettings.AppSettings.AppStartup = $oldSettings.AppSettings.AppStartup
        $newSettings.AppSettings.EncryptionKey = $oldSettings.AppSettings.EncryptionKey
        $newSettings.AppSettings.ApiStartupType = $oldSettings.AppSettings.ApiStartupType
        $newSettings.AppSettings.ApiExternalUrl =  $oldSettings.AppSettings.ApiExternalUrl
        $newSettings.AppSettings.XsdFolder = $oldSettings.AppSettings.XsdFolder
        $newSettings.AppSettings.PathBase = $oldSettings.AppSettings.PathBase
        $newSettings.Log4NetCore.Log4NetConfigFileName = $oldSettings.Log4NetCore.Log4NetConfigFileName

        $EmptyHashTable=@{}
        $mergedSettings = Merge-Hashtables $newSettings, $EmptyHashTable
        New-JsonFile $newSettingsFile $mergedSettings -Overwrite
    }
}

function Invoke-TransferConnectionStrings{
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $backUpPath = $Config.ApplicationBackupPath
        Write-Warning "Connection strings will be copied over from existing appsettings ($backUpPath)."
        $oldSettingsFile =  Join-Path $backUpPath "appsettings.json"
        $oldSettings = Get-Content $oldSettingsFile | ConvertFrom-Json | ConvertTo-Hashtable

        $newSettingsFile = Join-Path $Config.WebConfigLocation "appsettings.json"
        $newSettings = Get-Content $newSettingsFile | ConvertFrom-Json | ConvertTo-Hashtable

        $connectionstrings = @{
            ConnectionStrings = @{
                ProductionOds = $oldSettings.ConnectionStrings.ProductionOds
                Admin = $oldSettings.ConnectionStrings.Admin
                Security = $oldSettings.ConnectionStrings.Security
            }
        }

        $Config.AdminConnectionString = $oldSettings.ConnectionStrings.Admin
        $Config.engine = $oldSettings.AppSettings.DatabaseEngine
        $mergedSettings = Merge-Hashtables $newSettings, $connectionstrings
        New-JsonFile $newSettingsFile  $mergedSettings -Overwrite
    }
}

function Invoke-StartWebSite($webSiteName, $portNumber){

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $webSite = Get-Website | Where-Object { $_.name -eq $webSiteName -and $_.State -eq 'Stopped'}
        if($webSite)
        {
            $Websites = Get-ChildItem IIS:\Sites
            foreach ($Site in $Websites)
            {
                if($Site.Name -ne $webSiteName -and $Site.State -eq 'Started')
                {
                    $webBinding = Get-WebBinding -Port $portNumber -Name $Site.Name -Protocol 'HTTPS'
                    if($webBinding)
                    {
                        $webSiteUsingSamePort = $true
                        break
                    }
                }
            }

            if(-not $webSiteUsingSamePort)
            {
                Write-Host "Starting $webSiteName."
                Start-IISSite -Name $webSiteName
            }
            else
            {
                Write-Warning "Can not start the website: $webSiteName. Since, the same port: $portNumber is in use."
            }
        }
    }
}

function Invoke-ResetIIS {
    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $default = 'n'
        Write-Warning "NOTICE: In order to upgrade or uninstall, Information Internet Service (IIS) needs to be stopped during the process. This will impact availability if users are using applications hosted with IIS."
        $confirmation = Request-Information -DefaultValue 'y' -Prompt "Please enter 'y' to proceed with an IIS reset or enter 'n' to stop the upgrade or uninstall. [Default Action: '$default']"

        if (!$confirmation) { $confirmation = $default}
        if ($confirmation -ieq 'y') {
            & {iisreset}
        }
        else {
            Write-Warning "Exiting the uninstall/upgrade process."
            exit
        }
    }
}

function Initialize-Configuration {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )
    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        # Validate the input parameters. Couldn't do so in the parameter declaration
        # because the function is contained in the Configuration module imported above.
        $Config.usingSharedCredentials = $Config.ContainsKey("DbConnectionInfo") -and (-not $null -eq $Config.DbConnectionInfo)
        if ($Config.usingSharedCredentials) {
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.DbConnectionInfo
            $Config.DbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApp"
            $Config.engine = $Config.DbConnectionInfo.Engine
        }
        else {
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.AdminDbConnectionInfo
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.OdsDbConnectionInfo
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.SecurityDbConnectionInfo
            $Config.AdminDbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApp"
            $Config.OdsDbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApp"
            $Config.SecurityDbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApp"
            $Config.engine = $Config.OdsDbConnectionInfo.Engine
        }
    }
}

function Get-DbDeploy {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )
    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $parameters = @{
            toolVersion = $DbDeployVersion
            toolsPath = $Config.ToolsPath
        }
        Install-ToolDbDeploy @parameters
    }
}

function Get-AdminAppPackage {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $parameters = @{
            PackageName = $Config.PackageName
            PackageVersion = $Config.PackageVersion
            ToolsPath = $Config.ToolsPath
            OutputDirectory = $Config.DownloadPath
            PackageSource = $Config.PackageSource
        }
        $packageDir = Get-NugetPackage @parameters
        Test-Error

        $Config.PackageDirectory = $packageDir
        $Config.WebConfigLocation = $packageDir
    }
}

function New-JsonFile {
    param(
        [string] $FilePath,

        [hashtable] $Hashtable,

        [switch] $Overwrite
    )

    if (-not $Overwrite -and (Test-Path $FilePath)) { return }

    $Hashtable | ConvertTo-Json -Depth 10 | Out-File -FilePath $FilePath -NoNewline -Encoding UTF8
}

function Invoke-TransformAppSettings {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $settingsFile = Join-Path $Config.WebConfigLocation "appsettings.json"
        $settings = Get-Content $settingsFile | ConvertFrom-Json | ConvertTo-Hashtable
        $settings.AppSettings.ProductionApiUrl = $Config.OdsApiUrl
        $settings.AppSettings.SecurityMetadataCacheTimeoutMinutes = '10'
        $settings.AppSettings.DatabaseEngine = $config.engine
        $settings.AppSettings.AppStartup = $Config.AppStartUp

        if(!$settings.AppSettings.EncryptionKey)
        {
            if($Config.ContainsKey("EncryptionKey") -AND $Config.EncryptionKey)
            {
                $encryptionKey = $Config.EncryptionKey
            }
            else {
                $encryptionKey = New-AESKey
            }
            $settings.AppSettings.EncryptionKey = $encryptionKey
        }
        if("OnPrem" -ieq $Config.AppStartup)
        {
            $settings.Log4NetCore.Log4NetConfigFileName = "log4net\log4net.OnPremisesRelease.config"

            # When installing Admin App 2.1.1 or below, DbSetupEnabled must be set to "false"
            # for OnPrem installations, to suppress the deprecated Setup tab from being mistakenly
            # displayed. As soon as this installer is updated to work with a release *after*
            # Admin App 2.1.1, this line can be safely removed as no longer applicable.
            $settings.AppSettings.DbSetupEnabled = "false"
        }

        if ($Config.AdminAppFeatures) {
            if ($Config.AdminAppFeatures.ContainsKey("ApiMode") -and $Config.AdminAppFeatures.ApiMode) {
                $settings.AppSettings.ApiStartupType = $Config.AdminAppFeatures.ApiMode
                if ($Config.AdminAppFeatures.ApiMode -ieq "yearspecific" -or $Config.AdminAppFeatures.ApiMode -ieq "districtspecific") {
                    if (-not $Config.OdsDatabaseName.Contains("{0}")) {
                        $Config.OdsDatabaseName += "_{0}"

                        $Config.OdsDatabaseName = $Config.OdsDatabaseName -replace "_Ods_\{0\}", "_{0}"
                    }
                }
            }
            if ($Config.AdminAppFeatures.ContainsKey("SecurityMetadataCacheTimeoutMinutes")) {
                $settings.AppSettings.SecurityMetadataCacheTimeoutMinutes = $Config.AdminAppFeatures.SecurityMetadataCacheTimeoutMinutes
            }
        }

        $EmptyHashTable=@{}
        $mergedSettings = Merge-Hashtables $settings, $EmptyHashTable
        New-JsonFile $settingsFile $mergedSettings -Overwrite
    }
}

function Invoke-TransformConnectionStrings {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )
    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        if ($Config.usingSharedCredentials) {
            $Config.AdminDbConnectionInfo = $Config.DbConnectionInfo.Clone()
            $Config.AdminDbConnectionInfo.DatabaseName = $Config.AdminDatabaseName

            $Config.OdsDbConnectionInfo = $Config.DbConnectionInfo.Clone()
            $Config.OdsDbConnectionInfo.DatabaseName = $Config.OdsDatabaseName

            $Config.SecurityDbConnectionInfo = $Config.DbConnectionInfo.Clone()
            $Config.SecurityDbConnectionInfo.DatabaseName = $Config.SecurityDatabaseName
        }
        else {
            # Inject default database names if not provided
            if (-not $Config.AdminDbConnectionInfo.DatabaseName) {
                $Config.AdminDbConnectionInfo.DatabaseName = "EdFi_Admin"
            }
            if (-not $Config.OdsDbConnectionInfo.DatabaseName) {
                $Config.OdsDbConnectionInfo.DatabaseName = "EdFi_Ods"
            }
            if (-not $Config.SecurityDbConnectionInfo.DatabaseName) {
                $Config.SecurityDbConnectionInfo.DatabaseName = "EdFi_Security"
            }
        }

        $settingsFile = Join-Path $Config.WebConfigLocation "appsettings.json"
        $settings = Get-Content $settingsFile | ConvertFrom-Json | ConvertTo-Hashtable

        Write-Host "Setting database connections in $($Config.WebConfigLocation)"
        $adminconnString = New-ConnectionString -ConnectionInfo $Config.AdminDbConnectionInfo -SspiUsername $Config.WebApplicationName
        $odsconnString = New-ConnectionString -ConnectionInfo $Config.OdsDbConnectionInfo -SspiUsername $Config.WebApplicationName
        $securityConnString = New-ConnectionString -ConnectionInfo $Config.SecurityDbConnectionInfo -SspiUsername $Config.WebApplicationName

        $connectionstrings = @{
            ConnectionStrings = @{
                ProductionOds = $odsconnString
                Admin = $adminconnString
                Security = $securityConnString
            }
        }

        $mergedSettings = Merge-Hashtables $settings, $connectionstrings
        New-JsonFile $settingsFile  $mergedSettings -Overwrite
    }
}

function Get-AdminInstallConnectionString {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    $dbInstallCredentials = $Config.DatabaseInstallCredentials
    $adminDbConnectionInfo = $Config.AdminDbConnectionInfo

    $useInstallCredentials = ($dbInstallCredentials.UseIntegratedSecurity) -or ($dbInstallCredentials.DatabaseUser -and $dbInstallCredentials.DatabasePassword -and -not $dbInstallCredentials.UseIntegratedSecurity)

    if ($useInstallCredentials){
        $adminDbConnectionInfo.UseIntegratedSecurity = $dbInstallCredentials.UseIntegratedSecurity
        $adminDbConnectionInfo.Username = $dbInstallCredentials.DatabaseUser
        $adminDbConnectionInfo.Password = $dbInstallCredentials.DatabasePassword
    }
    else
    {
        if($Config.ApplicationInstallType -ieq "Upgrade" -and $Config.AdminConnectionString)
        {
            return $Config.AdminConnectionString
        }
    }

    return New-ConnectionString $adminDbConnectionInfo
}

function Invoke-DbUpScripts {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {

        $adminConnectionString = Get-AdminInstallConnectionString $Config
        $engine = "PostgreSql"

        if(!(Test-IsPostgreSQL -Engine $Config.engine)){
            $engine = "SqlServer"
        }

        $params = @{
            Verb = "Deploy"
            Engine = $engine
            Database = "Admin"
            ConnectionString = $adminConnectionString
            FilePaths = $Config.WebApplicationPath
            ToolsPath = $Config.ToolsPath
        }

        Invoke-DbDeploy @params
    }
}

function Request-Information {
  [CmdletBinding()]
  param (
      [Parameter(Mandatory=$true)]
      $Prompt,
      [Parameter(Mandatory=$true)]
      $DefaultValue
  )

  $isInteractive = [Environment]::UserInteractive
  if($isInteractive) {
      $confirmation = Read-Host -Prompt $Prompt
  } else {
      $confirmation = $DefaultValue
  }

  return $confirmation
}

function Install-Application {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {

        $iisParams = @{
            SourceLocation = $Config.PackageDirectory
            WebApplicationPath = $Config.WebApplicationPath
            WebApplicationName = $Config.WebApplicationName
            WebSitePath = $Config.WebSitePath
            WebSitePort = $Config.WebSitePort
            WebSiteName = $Config.WebSiteName
            CertThumbprint = $Config.CertThumbprint
            DotNetVersion = $RequiredDotNetHostingBundleVersion
        }

        Install-EdFiApplicationIntoIIS @iisParams
    }
}

function Set-SqlLogins {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {

        if($Config.usingSharedCredentials)
        {
            Add-SqlLogins $Config.DbConnectionInfo $Config.WebApplicationName -IsCustomLogin
        }
        else
        {
            Write-Host "Adding Sql Login for Admin Database:";
            Add-SqlLogins $Config.AdminDbConnectionInfo $Config.WebApplicationName -IsCustomLogin
            
            Write-Host "Adding Sql Login for Ed-Fi ODS Database:";
            Add-SqlLogins $Config.OdsDbConnectionInfo $Config.WebApplicationName -IsCustomLogin
            
            Write-Host "Adding Sql Login for Security Database:";
            Add-SqlLogins $Config.SecurityDbConnectionInfo $Config.WebApplicationName -IsCustomLogin
        }
    }
}

Export-ModuleMember -Function Install-EdFiOdsAdminApp, Uninstall-EdFiOdsAdminApp, Update-EdFiOdsAdminApp
