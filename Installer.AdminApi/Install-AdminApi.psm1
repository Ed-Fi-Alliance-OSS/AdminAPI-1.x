# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5

$ErrorActionPreference = "Stop"

function Set-TlsVersion {

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12 -bor [Net.SecurityProtocolType]::Tls13
}

$appCommonDirectory = "$PSScriptRoot/AppCommon"
$RequiredDotNetHostingBundleVersion = "6.0.0"

Import-Module -Force "$appCommonDirectory/Environment/Prerequisites.psm1" -Scope Global
Set-TlsVersion

Import-Module -Force "$appCommonDirectory/Utility/hashtable.psm1" -Scope Global
Import-Module -Force "$appCommonDirectory/Utility/nuget-helper.psm1"
Import-Module -Force "$appCommonDirectory/Utility/TaskHelper.psm1"
Import-Module -Force "$appCommonDirectory/Utility/ToolsHelper.psm1"

# Import the following with global scope so that they are available inside of script blocks
Import-Module -Force "$appCommonDirectory/Application/Install.psm1" -Scope Global
Import-Module -Force "$appCommonDirectory/Application/Uninstall.psm1" -Scope Global
Import-Module -Force "$appCommonDirectory/Application/Configuration.psm1" -Scope Global

$DbDeployVersion = "3.0.1"

function Install-AdminApi {
    <#
    .SYNOPSIS
        Installs the Ed-Fi ODS/API AdminApi application into IIS.

    .DESCRIPTION
        Installs and configures the Ed-Fi ODS/API AdminApi application in IIS running in Windows 10 or
        Windows Server 2016+. As needed, will create a new "Ed-Fi" website in IIS, configure it
        for HTTPS, and load the Admin Api binaries as an an application. Transforms the web.config
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
        PS c:\> Install-AdminApi @parameters

        Installs Admin Api to SQL Server with mainly defaults, except for the custom Sandbox ODS
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
        PS c:\> Install-AdminApi @parameters

        Installs Admin Api to multiple SQL Servers for each of the databases with some
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
        PS c:\> Install-AdminApi @parameters

        Installs Admin Api to PostgreSQL Server with integrated security install credentials and
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
            AdminApiFeatures = @{
                ApiMode="yearspecific"
                SecurityMetadataCacheTimeoutMinutes="5"
            }
        }
        PS c:\> Install-AdminApi @parameters

        Installs Admin Api to SQL Server in Year Specific mode for 2020. The installer will also
        install Admin Api in ASP.NET Identity mode, rather than AD Authentication, and will set the
        SecurityMetadataCacheTimeoutMinutes to 5 minutes.
    #>
    [CmdletBinding()]
    param (
        # NuGet package name. Default: EdFi.Suite3.ODS.Admin.Api.
        [string]
        $PackageName = "EdFi.Suite3.ODS.Admin.Api",

        # NuGet package version. If not set, will retrieve the latest full release package.
        [string]
        $PackageVersion,

        # NuGet package source. Please specify the path to the AdminApi sub-directory within the Admin.Api nuget package.
        [Parameter(Mandatory=$true)]
        [string]
        $PackageSource,

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

        # Web application name. Default: "AdminApi".
        [string]
        $WebApplicationName = "AdminApi",

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
        # default, AdminApi is installed in Shared
        # Instance mode with AD Authentication and a 10 minute security metadata cache timeout.
        [hashtable]
        $AdminApiFeatures,

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
        AdminApiFeatures = $AdminApiFeatures
        NoDuration = $NoDuration
    }

    $elapsed = Use-StopWatch {
        $result += Invoke-InstallationPreCheck -Config $Config
        $result += Initialize-Configuration -Config $config
        $result += Set-AdminApiPackageSource -Config $Config
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
        $existingAdminApiApplication = get-webapplication $Config.WebApplicationName

        if($webSite -AND $existingAdminApiApplication)
        {
            $existingApplicationPath, $versionString = GetExistingAppVersion $webSite.PhysicalPath $existingAdminApiApplication
            $installVersionString = $Config.PackageVersion

            $targetIsNewer = IsVersionHigherThanOther $installVersionString $versionString

            if($targetIsNewer) {
                Write-Host "We found a preexisting Admin Api $versionString installation. If you are seeking to upgrade to the new version, consider using the included upgrade script instead." -ForegroundColor Green
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
                    }
                }
            }elseif ($targetIsNewer) {
                Write-Warning "We found a preexisting Admin Api $versionString installation. That version cannot be automatically upgraded in-place by this script. Please refer to https://techdocs.ed-fi.org/display/ADMIN/Upgrading+Admin+App+from+1.x+Line for setting up the newer version of AdminApi. Exiting."
                exit
            }else {
                Write-Warning "We found a preexisting Admin Api $versionString installation newer than the target version $installVersionString. Downgrades are not supported. Please fully uninstall the existing Admin Api first and retry. Exiting."
                exit
            }
        }

        if($Config.AdminApiFeatures.ContainsKey("ApiMode") -and $Config.AdminApiFeatures.ApiMode) {
            $apiMode = $Config.AdminApiFeatures.ApiMode
            $supportedModes = @('sandbox', 'sharedinstance', 'yearspecific', 'districtspecific')
            if ($supportedModes -NotContains $apiMode) {
                Write-Warning "Not supported ApiMode: '$apiMode'. Please use one of the supported modes for the ApiMode Admin Api feature. Supported modes:'$($supportedModes -join "','")'"
                exit
            }
        }
    }
}

function GetExistingAppVersion($webSitePath,  $existingAdminApi) {
    $existingApplicationPath = ($existingAdminApi).PhysicalPath
    if(!$existingApplicationPath)
    {
        # In case of existingApplicationPath is empty, then generating application physical path by combining site physical path and application path
        $appPath = $existingAdminApi.path.trimstart('/')
        $existingApplicationPath = "$webSitePath\$appPath"
    }

    $versionString = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$existingApplicationPath\EdFi.Ods.Admin.Api.dll").FileVersion

    return $existingApplicationPath, $versionString
}

function IsVersionHigherThanOther($versionString, $otherVersionString) {
    $version = ParseVersion($versionString)
    $otherVersion = ParseVersion($otherVersionString)

    $result = $version.CompareTo($otherVersion)
    return $result -gt 0
}

function IsVersionMismatch($versionString, $otherVersionString) {
    $version = ParseVersion($versionString)
    $otherVersion = ParseVersion($otherVersionString)

    $result = $version.CompareTo($otherVersion)
    return $result -ne 0
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
            $Config.DbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApi"
            $Config.engine = $Config.DbConnectionInfo.Engine
        }
        else {
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.AdminDbConnectionInfo
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.OdsDbConnectionInfo
            Assert-DatabaseConnectionInfo -DbConnectionInfo $Config.SecurityDbConnectionInfo
            $Config.AdminDbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApi"
            $Config.OdsDbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApi"
            $Config.SecurityDbConnectionInfo.ApplicationName = "Ed-Fi ODS/API AdminApi"
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

function Set-AdminApiPackageSource {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {

        $adminApiSource = $Config.PackageSource
        $configVersionString = $Config.PackageVersion
        $versionString = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$adminApiSource\EdFi.Ods.Admin.Api.dll").FileVersion

        if (IsVersionMismatch $versionString $configVersionString) {
            Write-Warning "The specified Admin Api package version $configVersionString in the configuration does not match the file version $versionString of the package used as source. Please specify the correct version in the installer configuration or use the correct source."
            exit
        }

        $Config.PackageDirectory = $adminApiSource
        $Config.WebConfigLocation = $adminApiSource
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

        if ($Config.AdminApiFeatures) {
            if ($Config.AdminApiFeatures.ContainsKey("ApiMode") -and $Config.AdminApiFeatures.ApiMode) {
                $settings.AppSettings.ApiStartupType = $Config.AdminApiFeatures.ApiMode
                if ($Config.AdminApiFeatures.ApiMode -ieq "yearspecific" -or $Config.AdminApiFeatures.ApiMode -ieq "districtspecific") {
                    if (-not $Config.OdsDatabaseName.Contains("{0}")) {
                        $Config.OdsDatabaseName += "_{0}"

                        $Config.OdsDatabaseName = $Config.OdsDatabaseName -replace "_Ods_\{0\}", "_{0}"
                    }
                }
            }
            if ($Config.AdminApiFeatures.ContainsKey("SecurityMetadataCacheTimeoutMinutes")) {
                $settings.AppSettings.SecurityMetadataCacheTimeoutMinutes = $Config.AdminApiFeatures.SecurityMetadataCacheTimeoutMinutes
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

Export-ModuleMember -Function Install-AdminApi