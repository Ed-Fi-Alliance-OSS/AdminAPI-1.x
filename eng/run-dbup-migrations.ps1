﻿# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5

Param(
    $config =
    @{
        "engine" = "SqlServer"
        "databaseServer" = "(local)"
        "databasePort" = ""
        "databaseUser" = ""
        "databasePassword" = ""
        "useIntegratedSecurity" = $true
        "adminDatabaseName" = "EdFi_Admin"
    }
)

$ErrorActionPreference = "Stop"

Import-Module -Name "$PSScriptRoot/database-manager.psm1" -Force

$arguments = @{
    ToolsPath = ".tools"
    ForPostgreSQL = "postgresql" -eq $config.engine.ToLower()
    Server = $config.databaseServer
    DatabaseName = $config.adminDatabaseName
    Port = $config.databasePort
    UseIntegratedSecurity = $config.useIntegratedSecurity
    Username = $config.databaseUser
    Password = $config.databasePassword
    NuGetFeed = "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json"
}

Write-Output "Installing the Admin API tables to $($arguments.DatabaseName)"
Install-AdminApiTables @arguments

$arguments.DatabaseName = "EdFi_Admin_Test"
Write-Output "Installing the Admin API tables to $($arguments.DatabaseName)"
Install-AdminApiTables @arguments
