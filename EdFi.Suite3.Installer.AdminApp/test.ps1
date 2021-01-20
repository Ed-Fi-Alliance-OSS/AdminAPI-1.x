# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# This script should not be included in the NuGet package. It can be used
# to run various test scenarios during manual/exploratory testing.
Param(
    $Scenario
)

import-module -force "$PSScriptRoot/Install-EdFiOdsAdminApp.psm1"

$PackageVersion = '2.1.0'

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
        "Uninstall" { Invoke-Uninstall }
        default { 
            Write-Host "Valid test scenarios are: "
            Write-Host "    InstallSql"
            Write-Host "    InstallSqlMultiInstance"
            Write-Host "    InstallPostgres"
            Write-Host "    InstallPostgresMultiInstance"
            Write-Host "    Uninstall"
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