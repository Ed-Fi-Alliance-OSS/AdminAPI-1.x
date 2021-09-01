# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# This script should not be included in the NuGet package. It can be used
# to run various test scenarios during manual/exploratory testing.
Param(
    $Scenario
)

Copy-Item -Path "$PSScriptRoot/../eng/key-management.psm1" -Destination "$PSScriptRoot/key-management.psm1"
import-module -force "$PSScriptRoot/Install-EdFiOdsAdminApp.psm1"

$PackageVersion = '2.2.1'

function Invoke-InstallSqlServer {

    $dbConnectionInfo = @{
        Server = "(local)"
        Engine = "SqlServer"
        UseIntegratedSecurity=$true
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://example-web-api.com/WebApi"
        PackageVersion = $PackageVersion
    }

    Install-EdFiOdsAdminApp @p
}

function Invoke-InstallApplication{
    param(
        [string] $Version,
        [string] $WebSiteName = "Ed-Fi",
        [string] $WebSitePath = "c:\inetpub\Ed-Fi",
        [string] $WebApplicationPath = "C:\inetpub\Ed-Fi\AdminApp",
        [string] $WebApplicationName = "AdminApp"
    )

    $dbConnectionInfo = @{
        Server = "(local)"
        Engine = "SqlServer"
        UseIntegratedSecurity=$true
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://example-web-api.com/WebApi"
        PackageVersion = $Version
        WebSiteName = $WebSiteName
        WebSitePath = $WebSitePath
        WebApplicationPath = $WebApplicationPath
        WebApplicationName = $WebApplicationName
    }
    Install-EdFiOdsAdminApp @p
}

function Invoke-Install-CompatibleVersion {

    $existingApplicationVersion = '2.2.0'
    $version = '2.3.0-pre0003'

    Invoke-InstallApplication $existingApplicationVersion

    # Install newer version
    Invoke-InstallApplication $version
}

function Invoke-Install-InCompatibleVersion {

    $existingApplicationVersion = '2.1.0'
    $version = '2.3.0-pre0003'

    Invoke-InstallApplication $existingApplicationVersion

    # Install newer version
    Invoke-InstallApplication $version
}

function Invoke-Install-WithCustomSettings{

    $existingApplicationVersion = '2.1.0'
    $version = '2.3.0-pre0003'

    Invoke-InstallApplication $existingApplicationVersion

    # Install with custom web site and web application names
    $p = @{
        Version = $version
        WebSiteName = "Ed-Fi-Custom"
        WebSitePath = "c:\inetpub\Ed-Fi-Custom"
        WebApplicationPath = "C:\inetpub\Ed-Fi-Custom\AdminApp-Custom"
        WebApplicationName = "AdminApp-Custom"
    }
    Invoke-InstallApplication @p
}

function Invoke-Upgrade-CompatibleVersion{

    $existingApplicationVersion = '2.2.0'
    $version = '2.3.0-pre0003'

    Invoke-InstallApplication $existingApplicationVersion

    # Upgrade to newer version
    Upgrade-EdFiOdsAdminApp -PackageVersion $version

}

function Invoke-Upgrade-InCompatibleVersion{
    $existingApplicationVersion = '2.1.0'
    $version = '2.3.0-pre0003'

    Invoke-InstallApplication $existingApplicationVersion

    # Upgrade to newer version
    Upgrade-EdFiOdsAdminApp -PackageVersion $version
}

function Invoke-Upgrade-WithCustomSettings{

    $existingApplicationVersion = '2.2.0'
    $version = '2.3.0-pre0003'

    # Install with custom web site and web application names
    $p = @{
        Version = $existingApplicationVersion
        WebSiteName = "Ed-Fi-Custom"
        WebSitePath = "c:\inetpub\Ed-Fi-Custom"
        WebApplicationPath = "C:\inetpub\Ed-Fi-Custom\AdminApp-Custom"
        WebApplicationName = "AdminApp-Custom"
    }
    Invoke-InstallApplication @p

    $upgradeParam =  $p = @{
        PackageVersion = $version
        WebSiteName = "Ed-Fi-Custom"
        WebSitePath = "c:\inetpub\Ed-Fi-Custom"
        WebApplicationPath = "C:\inetpub\Ed-Fi-Custom\AdminApp-Custom"
        WebApplicationName = "AdminApp-Custom"
    }
    Upgrade-EdFiOdsAdminApp @upgradeParam
}

function Invoke-InstallMultiInstanceSqlServer {

    $dbConnectionInfo = @{
        Server = "(local)"
        Engine = "SqlServer"
        UseIntegratedSecurity=$true
    }

    $adminAppFeatures = @{
        ApiMode = "yearSpecific"
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://example-web-api.com/WebApi"
        PackageVersion = $PackageVersion
        AdminAppFeatures = $adminAppFeatures
    }

    Install-EdFiOdsAdminApp @p
}

function Invoke-InstallPostgres {

    $dbConnectionInfo = @{
        Server = "localhost"
        Engine = "PostgreSQL"
        DatabasePort = "5432"
        UseIntegratedSecurity=$true
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://example-web-api.com/WebApi"
        PackageVersion = $PackageVersion
    }

    Install-EdFiOdsAdminApp @p
}

function Invoke-InstallMultiInstancePostgres {

    $dbConnectionInfo = @{
        Server = "localhost"
        Engine = "PostgreSQL"
        DatabasePort = "5432"
        UseIntegratedSecurity=$true
    }

    $adminAppFeatures = @{
        ApiMode = "districtspecifc"
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://example-web-api.com/WebApi"
        PackageVersion = $PackageVersion
        AdminAppFeatures = $adminAppFeatures
    }

    Install-EdFiOdsAdminApp @p
}

function Invoke-Uninstall {
    $p = @{
        ToolsPath = "C:/temp/tools"
    }

    UnInstall-EdFiOdsAdminApp @p
}

try {
    switch ($Scenario) {
        "InstallSql" { Invoke-InstallSqlServer }
        "InstallSqlMultiInstance" { Invoke-InstallMultiInstanceSqlServer }
        "InstallPostgres" { Invoke-InstallPostgres }
        "InstallPostgresMultiInstance" { Invoke-InstallMultiInstancePostgres }
        "Install-CompatibleVersion" { Invoke-Install-CompatibleVersion }
        "Install-InCompatibleVersion" { Invoke-Install-InCompatibleVersion }
        "Install-WithCustomSettings" { Invoke-Install-WithCustomSettings }
        "Upgrade-CompatibleVersion" { Invoke-Upgrade-CompatibleVersion }
        "Upgrade-InCompatibleVersion" { Invoke-Upgrade-InCompatibleVersion }
        "Upgrade-ApplicationWithCustomSettings" { Invoke-Upgrade-WithCustomSettings }
        "Uninstall" { Invoke-Uninstall }
        default {
            Write-Host "Valid test scenarios are: "
            Write-Host "    InstallSql"
            Write-Host "    InstallSqlMultiInstance"
            Write-Host "    InstallPostgres"
            Write-Host "    InstallPostgresMultiInstance"
            Write-Host "    Uninstall"
            Write-Host "    Install-CompatibleVersion"
            Write-Host "    Install-InCompatibleVersion"
            Write-Host "    Install-WithCustomSettings"
            Write-Host "    Upgrade-CompatibleVersion"
            Write-Host "    Upgrade-InCompatibleVersion"
            Write-Host "    Upgrade-ApplicationWithCustomSettings"
        }
    }
}
catch {
    $ErrorRecord= $_
    $ErrorRecord | Format-List * -Force
    $ErrorRecord.InvocationInfo |Format-List *
    $Exception = $ErrorRecord.Exception
    for ($i = 0; $Exception; $i++, ($Exception = $Exception.InnerException))
    {
        "$i" * 80
        $Exception |Format-List * -Force
    }
}
