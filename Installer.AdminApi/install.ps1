# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

import-module -force "$PSScriptRoot/Install-AdminApi.psm1"

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

Installs and connects the applications to the database using PostgreSQL Authentication

    $dbConnectionInfo = @{
        Server = "localhost"
        Engine = "PostgreSQL"
        UseIntegratedSecurity = $false
        Username = "postgres"
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
Configure Admin Api to manage an ODS API with url "https://localhost:54746"

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "https://localhost:54746"
        PackageVersion = '1.1.0'
    }

.EXAMPLE
Deploy Admin Api for use with a "District Specific" ODS API

    $adminApiFeatures = @{
        ApiMode = "districtspecifc"
    }

    $p = @{
        ToolsPath = "C:/temp/tools"
        DbConnectionInfo = $dbConnectionInfo
        OdsApiUrl = "http://web-api.example.com/WebApi"
        PackageVersion = '1.1.0'
        AdminApiFeatures = $adminApiFeatures
    }
#>

$adminApiFeatures = @{
    ApiMode = "sharedinstance"
}

# Authentication Settings
# Authentication:SigningKey must be a Base64-encoded string
# Authentication:Authority and Authentication:IssuerUrl should be the same URL as your application
# Changing Authentication:AllowRegistration to true allows unrestricted registration of new Admin API clients. This is not recommended for production. 
$authenticationSettings = @{
    Authority = ""
    IssuerUrl = ""
    SigningKey = ""
    AllowRegistration = $false
}

$packageSource = Split-Path $PSScriptRoot -Parent
$adminApiSource = "$packageSource/AdminApi"

$p = @{
    ToolsPath = "C:/temp/tools"
    DbConnectionInfo = $dbConnectionInfo
    OdsApiUrl = ""
    PackageVersion = '1.1.0.0'
    PackageSource = $adminApiSource
    AuthenticationSettings = $authenticationSettings
    AdminApiFeatures = $adminApiFeatures
}

if ([string]::IsNullOrWhiteSpace($p.OdsApiUrl)) {
    Write-Error "ODS API URL has not been configured. Edit install.ps1 to pass in a valid url for the ODS API."
}
elseif ([string]::IsNullOrWhiteSpace($p.AuthenticationSettings.Authority) -or [string]::IsNullOrWhiteSpace($p.AuthenticationSettings.IssuerUrl) -or [string]::IsNullOrWhiteSpace($p.AuthenticationSettings.SigningKey) -or $p.AuthenticationSettings.AllowRegistration -isnot [bool]) {
    Write-Error "Authentication Settings have not been configured correctly. Edit install.ps1 to pass in valid authentication settings for Admin Api."
}
else {
    Install-EdFiOdsAdminApi @p
}
