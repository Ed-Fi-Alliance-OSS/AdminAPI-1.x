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
    $NuGetFeed = "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json",

    [string]
    $NuGetApiKey,

    [switch]
    $IsLocalBuild
)

$ErrorActionPreference = "Stop"
$OutputDirectory = Resolve-Path $PSScriptRoot
$PackageDefinitionFile = Resolve-Path "$PSScriptRoot/EdFi.Suite3.Installer.AdminApp.nuspec"
$Downloads = "$PSScriptRoot/downloads"
$Version = "$SemanticVersion.$BuildCounter"

$AppCommonPackageName = "EdFi.Installer.AppCommon"
$AppCommonPackageVersion = "3.0.0"

function Add-AppCommon{

    if(-not(Test-Path -Path $Downloads )){
        mkdir $Downloads
    }

    $parameters = @(
        "install", $AppCommonPackageName,
        "-source", $NuGetFeed,
        "-outputDirectory", $Downloads
        "-version", $AppCommonPackageVersion
    )

    Write-Host "Downloading AppCommon"
    Write-Host -ForegroundColor Magenta "Executing nuget: $parameters"
    nuget $parameters

    $appCommonDirectory = Resolve-Path $Downloads/$AppCommonPackageName.$AppCommonPackageVersion* | Select-Object -Last 1

    Move-AppCommon $appCommonDirectory
}

function Add-AppCommonLocal{    

    Import-Module -Force "$PSScriptRoot/nuget-helper.psm1"

    $parameters = @{
        PackageName = $AppCommonPackageName
        PackageVersion = $AppCommonPackageVersion
        ToolsPath = "C:/temp/tools"
        PackageSource = $NuGetFeed
    }
    $appCommonDirectory = Get-NugetPackage @parameters

    Move-AppCommon $appCommonDirectory
}

function Move-AppCommon {
    param (
        [string]
        [Parameter(Mandatory=$true)]
        $AppCommonSourceDirectory
    )

    # Move AppCommon's modules to a local AppCommon directory
    @(
        "Application"
        "Environment"
        "IIS"
        "Utility"
    ) | ForEach-Object {
        $parameters = @{
            Recurse = $true
            Force = $true
            Path = "$AppCommonSourceDirectory/$_"
            Destination = "$PSScriptRoot/AppCommon/$_"
        }
        Copy-Item @parameters
    }
}

function New-Package {

    $parameters = @(
        "pack", $PackageDefinitionFile,
        "-Version", $Version,
        "-OutputDirectory", $OutputDirectory,
        "-Verbosity", "detailed"
    )

    Write-Host @parameters -ForegroundColor Magenta
    nuget @parameters
}

function New-PackageLocal {

    $appCommonUtilityDirectory = "$PSScriptRoot/AppCommon/Utility"

    Import-Module "$appCommonUtilityDirectory/create-package.psm1" -Force

    $parameters = @{
        PackageDefinitionFile = $PackageDefinitionFile
        Version = "$Version"
        OutputDirectory = $OutputDirectory
        Source = $NuGetFeed
        ApiKey = $NuGetApiKey
        ToolsPath = "C:/temp/tools"
    }
    Invoke-CreatePackage @parameters -Verbose:$verbose
}


if (-not $IsLocalBuild) {
    
    #Add AppCommon for Github Actions
    Add-AppCommon
    
    # Build package for Github Actions
    Write-Host "Building Package"
    New-Package
} else {
 
    #Add AppCommon locally
    Add-AppCommonLocal
    
    # Build package locally
    Write-Host "Building Package"
    New-PackageLocal
}