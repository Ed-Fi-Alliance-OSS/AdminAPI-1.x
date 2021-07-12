# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

import-module -force "$PSScriptRoot/Install-EdFiOdsAdminApp.psm1"

<#
Review and edit the following connection information for your database server

.EXAMPLE
Installs and connects the applications to the database using SQL Authentication

    $dbConnectionInfo = @{
        Server = "(local)"
        Engine = "SqlServer"
        UseIntegratedSecurity = $false
        Username = "exampleAdmin"
        Password = "examplePassword"
    }
#>

$dbConnectionInfo = @{
    Server = "(local)"
    Engine = "SqlServer"
    UseIntegratedSecurity=$true
}

<#
Review and edit the following application settings and connection information for Admin App

.EXAMPLE
Configure Admin App to manage an API with url "https://localhost:54746"

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "https://localhost:54746"
        PackageVersion = '2.2.1'
    }

.EXAMPLE
Deploy Admin App for use with a "District Specific" ODS API

    $adminAppFeatures = @{
        ApiMode = "districtspecifc"
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://web-api.example.com/WebApi"
        PackageVersion = '2.2.1'
        AdminAppFeatures = $adminAppFeatures
    }
#>

$adminAppFeatures = @{
    ApiMode = "sharedInstance"
}

$p = @{
    ToolsPath = "C:/temp/tools"
    DbConnectionInfo = $dbConnectionInfo
    OdsApiUrl = ""
    PackageVersion = '2.2.1'
    AdminAppFeatures = $adminAppFeatures
}

if ([string]::IsNullOrWhiteSpace($p.OdsApiUrl)) {
    Write-Error "ODS API URL has not been configured. Edit install.ps1 to pass in a valid url for the ODS API."
}
else {
    Install-EdFiOdsAdminApp @p
}
