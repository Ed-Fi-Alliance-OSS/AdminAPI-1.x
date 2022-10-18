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
$existingCompatibleVersion = '2.2.0'
$existingInCompatibleVersion = '2.1.0'
$newVersion = '2.3.0-pre0003'

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
        WebApplicationName = $WebApplicationName
    }
    Install-EdFiOdsAdminApp @p
}

function Invoke-Install-CompatibleVersion {

    Invoke-InstallApplication $existingCompatibleVersion

    # Install newer version
    Invoke-InstallApplication $newVersion
}

function Invoke-Install-InCompatibleVersion {

    Invoke-InstallApplication $existingInCompatibleVersion

    # Install newer version (should exit with warning)
    Invoke-InstallApplication $newVersion
}

function Invoke-Install-OldVersion {

    Invoke-InstallApplication $existingInCompatibleVersion

    # Install older version (should exit with warning)
    Invoke-InstallApplication $existingInCompatibleVersion
}

function Invoke-Install-WithCustomSettings{

    # Install with custom web site and web application names
    $p = @{
        Version = $newVersion
        WebSiteName = "Ed-Fi-Custom"
        WebSitePath = "c:\inetpub\Ed-Fi-Custom"
        WebApplicationName = "AdminApp-Custom"
    }
    Invoke-InstallApplication @p
}

function Invoke-Upgrade-CompatibleVersion{

    Invoke-InstallApplication $existingCompatibleVersion

    # Upgrade to newer version
    Update-EdFiOdsAdminApp -PackageVersion $newVersion

}

function Invoke-Upgrade-InCompatibleVersion{

    Invoke-InstallApplication $existingInCompatibleVersion

    # Upgrade to newer version
    Update-EdFiOdsAdminApp -PackageVersion $newVersion
}

function Invoke-Upgrade-WithCustomSettings{

    # Install with custom web site and web application names
    $p = @{
        Version = $existingCompatibleVersion
        WebSiteName = "Ed-Fi-Custom"
        WebSitePath = "c:\inetpub\Ed-Fi-Custom"
        WebApplicationName = "AdminApp-Custom"
    }
    Invoke-InstallApplication @p

    $upgradeParam =  $p = @{
        PackageVersion = $newVersion
        WebSiteName = "Ed-Fi-Custom"
        WebSitePath = "c:\inetpub\Ed-Fi-Custom"
        WebApplicationName = "AdminApp-Custom"
    }
    Update-EdFiOdsAdminApp @upgradeParam
}

function Invoke-Upgrade-SameVersion{

    Invoke-InstallApplication $existingCompatibleVersion

    # Upgrade to same version (should fail)
    Update-EdFiOdsAdminApp -PackageVersion $existingCompatibleVersion
}

function Invoke-Upgrade-OldVersion{

    Invoke-InstallApplication $existingCompatibleVersion

    # Upgrade to older version (should fail)
    Update-EdFiOdsAdminApp -PackageVersion $existingInCompatibleVersion
}

function Invoke-Upgrade-Nonsense{

    Invoke-InstallApplication $existingCompatibleVersion

    # Upgrade to not a version (should fail)
    Update-EdFiOdsAdminApp -PackageVersion "asfjaslkdja"
}

function Invoke-Upgrade-Prerelease{

    Invoke-InstallApplication '2.3.0-pre0003'

    # Upgrade to newer 'pre' build (should succeed)
    Update-EdFiOdsAdminApp -PackageVersion "2.3.0-pre0005"
}

function Invoke-InstallMultiInstanceSqlServer {

    $dbConnectionInfo = @{
        Server = "(local)"
        Engine = "SqlServer"
        UseIntegratedSecurity=$true
    }

    $adminAppFeatures = @{
        ApiMode = "yearspecific"
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
        "Install-OldVersion" { Invoke-Install-OldVersion }
        "Install-WithCustomSettings" { Invoke-Install-WithCustomSettings }
        "Upgrade-CompatibleVersion" { Invoke-Upgrade-CompatibleVersion }
        "Upgrade-InCompatibleVersion" { Invoke-Upgrade-InCompatibleVersion }
        "Upgrade-ApplicationWithCustomSettings" { Invoke-Upgrade-WithCustomSettings }
        "Upgrade-SameVersion" { Invoke-Upgrade-SameVersion }
        "Upgrade-OldVersion" { Invoke-Upgrade-OldVersion }
        "Upgrade-Nonsense" { Invoke-Upgrade-Nonsense }
        "Upgrade-Prerelease" { Invoke-Upgrade-Prerelease }
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
            Write-Host "    Install-OldVersion"
            Write-Host "    Install-WithCustomSettings"
            Write-Host "    Upgrade-CompatibleVersion"
            Write-Host "    Upgrade-InCompatibleVersion"
            Write-Host "    Upgrade-ApplicationWithCustomSettings"
            Write-Host "    Upgrade-SameVersion"
            Write-Host "    Upgrade-OldVersion"
            Write-Host "    Upgrade-Nonsense"
            Write-Host "    Upgrade-Prerelease"
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
