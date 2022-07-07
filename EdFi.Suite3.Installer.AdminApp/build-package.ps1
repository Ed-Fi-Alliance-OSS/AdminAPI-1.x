# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5
param (
    [string]
    [Parameter(Mandatory=$true)]
    $SemanticVersion,

    [string]
    [Parameter(Mandatory=$true)]
    $BuildCounter,

    [string]
    $NuGetFeed,

    [string]
    $NuGetApiKey
)

$ErrorActionPreference = "Stop"
$OutputDirectory = Resolve-Path $PSScriptRoot
$PackageDefinitionFile = Resolve-Path "$PSScriptRoot/EdFi.Suite3.Installer.AdminApp.nuspec"
$Downloads = "$PSScriptRoot/downloads"
$Version = "$SemanticVersion.$BuildCounter"

Invoke-Expression "$PSScriptRoot/prep-installer-package.ps1 $PSScriptRoot"

function Build-Package {

    $parameters = @(
        "pack", $PackageDefinitionFile,
        "-Version", $Version,
        "-OutputDirectory", $OutputDirectory,
        "-Verbosity", "detailed"
    )

    Write-Host @parameters -ForegroundColor Magenta
    nuget @parameters
}

Write-Host "Building package"
Build-Package
