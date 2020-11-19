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

$env:PathResolverRepositoryOverride = "Ed-Fi-Ods;Ed-Fi-ODS-Implementation;"
Import-Module -Force -Scope Global "$PSScriptRoot/Ed-Fi-ODS-Implementation/logistics/scripts/modules/path-resolver.psm1"

Import-Module -Force $folders.modules.invoke("Environment/Prerequisites.psm1") -Scope Global
Set-TlsVersion
Install-DotNetCore "C:\temp\tools"

Import-Module -Force -Scope Global $folders.modules.invoke("utility/hashtable.psm1")
Import-Module -Force $folders.modules.invoke("packaging/nuget-helper.psm1")
Import-Module -Force $folders.modules.invoke("tasks/TaskHelper.psm1")
Import-Module -Force $folders.modules.invoke("tools/ToolsHelper.psm1")

# Import the following with global scope so that they are available inside of script blocks
Import-Module -Force $folders.modules.invoke("Application/Install.psm1") -Scope Global
Import-Module -Force $folders.modules.invoke("Application/Uninstall.psm1") -Scope Global
Import-Module -Force $folders.modules.invoke("Application/Configuration.psm1") -Scope Global

$DbDeployVersion = "1.1.0"

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

        # Path for storing installation tools, e.g. nuget.exe. Default: "./tools".
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

        # Path for the web application. Default: "c:\inetpub\Ed-Fi\AdminApp".
        [string]
        $WebApplicationPath = "C:\inetpub\Ed-Fi\AdminApp",

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
        $NoDuration
    )

    Write-InvocationInfo $MyInvocation

    Clear-Error

    $result = @()

    $Config = @{
        WebApplicationPath = $WebApplicationPath
        PackageName = $PackageName
        PackageVersion = $PackageVersion
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
    }

    $elapsed = Use-StopWatch {
        $result += Initialize-Configuration -Config $config
        $result += Get-AdminAppPackage -Config $Config
        $result += Get-DbDeploy -Config $Config        

        if (Test-IsPostgreSQL -Engine $config.engine) {
            $result += Invoke-TransformWebConfigPostgreSQL -Config $Config
        }
        $result += Invoke-TransformWebConfigAppSettings -Config $Config
        $result += Invoke-TransformWebConfigConnectionStrings -Config $config
        $result += Install-Application -Config $Config
        $result += Create-SqlLogins -Config $Config
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
            WebApplicationPath="d:/octopus/applications/staging/AdminApp-3"
            WebApplicationName = "AdminApp"
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
        $parameters = @{
            WebApplicationPath = $config.WebApplicationPath
            WebApplicationName = $config.WebApplicationName
            WebSiteName = $config.WebSiteName
        }

        Uninstall-EdFiApplicationFromIIS @parameters

        $result
    }

    Test-Error

    if (-not $NoDuration) {
        $result += New-TaskResult -name "-" -duration "-"
        $result += New-TaskResult -name $MyInvocation.MyCommand.Name -duration $elapsed.format
        $result | Format-Table
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
        }
        $packageDir = Get-NugetPackage @parameters
        Test-Error

        $Config.PackageDirectory = $packageDir
        $Config.WebConfigLocation = $packageDir
    }
}

function Invoke-TransformWebConfigPostgreSQL {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {
        $transformParams = @{
            sourceFile = "$($Config.PackageDirectory)/Web.base.config"
            transformFile = "$($Config.PackageDirectory)/Web.OnPremNpgsql.config"
            destinationFile = "$($Config.PackageDirectory)/Web.config"
            toolsPath = $Config.ToolsPath
        }
        Invoke-ConfigTransformation @transformParams
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

function Invoke-TransformWebConfigAppSettings {
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

        if ($Config.AdminAppFeatures) {
            if ($Config.AdminAppFeatures.ContainsKey("ApiMode") -and $Config.AdminAppFeatures.ApiMode) {
                $settings.AppSettings.ApiStartupType = $Config.AdminAppFeatures.ApiMode
                if ($Config.AdminAppFeatures.ApiMode -ieq "yearspecific" -or $Config.AdminAppFeatures.ApiMode -ieq "districtspecific") {
                    $Config.OdsDatabaseName = "EdFi_{0}"
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

function Invoke-TransformWebConfigConnectionStrings {
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
        }

        Install-EdFiApplicationIntoIIS @iisParams
    }
}

function Create-SqlLogins {
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory=$true)]
        $Config
    )

    Invoke-Task -Name ($MyInvocation.MyCommand.Name) -Task {

        if($Config.usingSharedCredentials)
        {
            Add-SqlLogins $Config.DbConnectionInfo $Config.WebApplicationName
        }
        else
        {
            Add-SqlLogins $Config.AdminDbConnectionInfo $Config.WebApplicationName
            Add-SqlLogins $Config.OdsDbConnectionInfo $Config.WebApplicationName
            Add-SqlLogins $Config.SecurityDbConnectionInfo $Config.WebApplicationName
        }
    }
}

Export-ModuleMember -Function Install-EdFiOdsAdminApp, Uninstall-EdFiOdsAdminApp
