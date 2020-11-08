# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5

Param(
    $config =
    @{
        "engine" = "sql"
        "databaseServer" = "(local)"
        "databasePort" = ""
        "databaseUser" = ""
        "databasePassword" = ""
        "useIntegratedSecurity" = $true
        "adminDatabaseName" = "EdFi_Admin"
    },
	[Switch] $netCore
)

$ErrorActionPreference = "Stop"

Import-Module -Name "$PSScriptRoot/database-manager.psm1" -Force

$arguments = @{
    ToolsPath = ".tools"
    DbDeployVersion = "1.1.0"
    ForPostgreSQL = "postgresql" -eq $config.engine.ToLower()
    Server = $config.databaseServer
    DatabaseName = $config.adminDatabaseName
    Port = $config.databasePort
    UseIntegratedSecurity = $config.useIntegratedSecurity
    Username = $config.databaseUser
    Password = $config.databasePassword
    NuGetFeed = "https://www.myget.org/F/ed-fi/api/v3/index.json"
}
if($netCore)
{
	$arguments.FilePath  = "$PSScriptRoot/../Application/EdFi.Ods.AdminApp.Web.Core"
}

Write-Host "Installing the Admin App tables to $($arguments.DatabaseName)" -ForegroundColor Cyan
Install-AdminAppTables @arguments

$arguments.DatabaseName = "EdFi_Admin_Test"
Write-Host "Installing the Admin App tables to $($arguments.DatabaseName)" -ForegroundColor Cyan
Install-AdminAppTables @arguments