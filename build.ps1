# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

[CmdLetBinding()]
<#
    .SYNOPSIS
        Automation script for running build operations from the command line.

    .DESCRIPTION
        Provides automation of the following tasks:

        * Clean: runs `dotnet clean`
        * Build: runs `dotnet build` with several implicit steps
          (clean, restore, inject version information).
        * UnitTest: executes NUnit tests in projects named `*.UnitTests`, which
          do not connect to a database.
        * IntegrationTest: executes NUnit tests in projects named `*.Test`,
          which connect to a database. Includes drop and deploy operations for
          installing fresh test databases.
        * BuildAndTest: executes the Build, UnitTest, and IntegrationTest
          commands.
        * Package: builds pre-release and release NuGet packages for the Admin
          App web application.
        * Push: uploads a NuGet package to the NuGet feed.
        * BuildAndDeployToAdminApiDockerContainer: runs the build operation, update the appsettings.json with provided
          DockerEnvValues and copy over the latest files to existing AdminApi docker container for testing.

    .EXAMPLE
        .\build.ps1 build -Configuration Release -Version "2.0" -BuildCounter 45

        Overrides the default build configuration (Debug) to build in release
        mode with assembly version 2.0.45.

    .EXAMPLE
        .\build.ps1 unittest

        Output: test results displayed in the console and saved to XML files.

    .EXAMPLE
        .\build.ps1 integrationtest

        Output: test results displayed in the console and saved to XML files.

    .EXAMPLE
        .\build.ps1 push -NuGetApiKey $env:nuget_key

    .EXAMPLE
       $p = @{
            ProductionApiUrl = "http://api"
            ApiExternalUrl = "https://localhost:5001"
            AppStartup = "OnPrem"
            XsdFolder = "/app/Schema"
            ApiStartupType = "SharedInstance"
            DatabaseEngine = "PostgreSql"
            BulkUploadHashCache = "/app/BulkUploadHashCache/"
            EncryptionKey = "<Generated encryption key>"
            AdminDB = "host=db-admin;port=5432;username=username;password=password;database=EdFi_Admin;Application Name=EdFi.Ods.AdminApi;"
            SecurityDB = "host=db-admin;port=5432;username=username;password=password;database=EdFi_Security;Application Name=EdFi.Ods.AdminApi;"
            ProductionOdsDB = "host=db-ods;port=5432;username=username;password=password;database=EdFi_{0};Application Name=EdFi.Ods.AdminApi;"
        }

        .\build.ps1 -Version "2.1" -Configuration Release -DockerEnvValues $p -Command BuildAndDeployToAdminApiDockerContainer
#>
[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', '', Justification = 'False positive')]
param(
    # Command to execute, defaults to "Build".
    [string]
    [ValidateSet("Clean", "Build", "BuildAndPublish", "UnitTest", "IntegrationTest",  "PackageApi"
    , "Push", "BuildAndTest", "BuildAndDeployToAdminApiDockerContainer"
    , "BuildAndRunAdminApiDevDocker", "RunAdminApiDevDockerContainer", "RunAdminApiDevDockerCompose", "Run")]
    $Command = "Build",

    # Assembly and package version number for Admin API. The current package number is
    # configured in the build automation tool and passed to this script.
    [string]
    $APIVersion = "0.1",

    # .NET project build configuration, defaults to "Debug". Options are: Debug, Release.
    [string]
    [ValidateSet("Debug", "Release")]
    $Configuration = "Debug",

    # Ed-Fi's official NuGet package feed for package download and distribution.
    [string]
    $EdFiNuGetFeed = "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json",

    # API key for accessing the feed above. Only required with with the Push
    # command.
    [string]
    $NuGetApiKey,

    # Full path of a package file to push to the NuGet feed. Optional, only
    # applies with the Push command. If not set, then the script looks for a
    # NuGet package corresponding to the provided $APIVersion and $BuildCounter.
    [string]
    $PackageFile,

    # Environment values for updating the appsettings on existing AdminApi docker container.

    # Only required with the BuildAndDeployToAdminApiDockerContainer or BuildAndDeployToAdminApiDockerContainer commands.
    [hashtable]
    $DockerEnvValues,

    # Only required with the Run command.
    [string]
    [ValidateSet("EdFi.Ods.AdminApi (Dev)", "EdFi.Ods.AdminApi (Prod)", "EdFi.Ods.AdminApi (Docker)", "IIS Express")]
    $LaunchProfile,

    # Only required with local builds and testing.
    [switch]
    $IsLocalBuild
)

$Env:MSBUILDDISABLENODEREUSE = "1"

$solutionRoot = "$PSScriptRoot/Application"

$supportedApiVersions = @(
    @{
        OdsPackageName = "EdFi.RestApi.Databases.EFA"
        OdsVersion = "3.4.0"
        Prerelease = $false
    },
    @{
        OdsPackageName = "EdFi.Suite3.RestApi.Databases"
        OdsVersion = "5.3.659"
        Prerelease = $false
    }
)
$maintainers = "Ed-Fi Alliance, LLC and contributors"

$appCommonPackageName = "EdFi.Installer.AppCommon"
$appCommonPackageVersion = "3.0.0"

Import-Module -Name "$PSScriptRoot/eng/build-helpers.psm1" -Force
Import-Module -Name "$PSScriptRoot/eng/package-manager.psm1" -Force
Import-Module -Name "$PSScriptRoot/eng/database-manager.psm1" -Force
function DotNetClean {
    Invoke-Execute { dotnet clean $solutionRoot -c $Configuration --nologo -v minimal }
}
function InitializeNuGet {
    Invoke-Execute { $script:nugetExe = Install-NugetCli }
}

function Restore {
    Invoke-Execute { dotnet restore $solutionRoot }
}

function SetAdminApiAssemblyInfo {
    Invoke-Execute {
        $assembly_version = $APIVersion

        Invoke-RegenerateFile "$solutionRoot/EdFi.Ods.AdminApi/Directory.Build.props" @"
<Project>
    <!-- This file is generated by the build script. -->
    <PropertyGroup>
        <Product>Ed-Fi ODS Admin API</Product>
        <Authors>$maintainers</Authors>
        <Company>$maintainers</Company>
        <Copyright>Copyright © ${(Get-Date).year)} Ed-Fi Alliance</Copyright>
        <VersionPrefix>$assembly_version</VersionPrefix>
        <VersionSuffix></VersionSuffix>
    </PropertyGroup>
</Project>
"@
    }
}

function Compile {
    Invoke-Execute {
        dotnet build $solutionRoot -c $Configuration --nologo --no-restore
    }
}

function PublishAdminApi {
    Invoke-Execute {
        $project = "$solutionRoot/EdFi.Ods.AdminApi/"
        $outputPath = "$project/publish"
        dotnet publish $project -c $Configuration /p:EnvironmentName=Production -o $outputPath --no-build --nologo
    }
}

function RunTests {
    param (
        # File search filter
        [string]
        $Filter
    )

    $testAssemblyPath = "$solutionRoot/$Filter/bin/$Configuration/"
    $testAssemblies = Get-ChildItem -Path $testAssemblyPath -Filter "$Filter.dll" -Recurse

    if ($testAssemblies.Length -eq 0) {
        Write-Output "no test assemblies found in $testAssemblyPath"
    }

    $testAssemblies | ForEach-Object {
        Write-Output "Executing: dotnet test $($_)"
        Invoke-Execute {
            dotnet test $_ `
                --logger "trx;LogFileName=$($_).trx" `
                --nologo
        }
    }
}

function UnitTests {
    Invoke-Execute { RunTests -Filter "*.UnitTests" }
}

function ResetTestDatabases {
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments', 'unused', Justification = 'False positive')]
    param (
        [string]
        $OdsPackageName,

        [string]
        $OdsVersion,

        [switch]
        $Prerelease
    )

    Invoke-Execute {
        $arguments = @{
            RestApiPackageVersion = $OdsVersion
            RestApiPackageName = $OdsPackageName
            UseIntegratedSecurity = $true
            RestApiPackagePrerelease = $Prerelease
            NuGetFeed = $EdFiNuGetFeed
        }

        Invoke-PrepareDatabasesForTesting @arguments
    }
}

function IntegrationTests {
    Invoke-Execute { RunTests -Filter "*.DBTests" }
}

function RunNuGetPack {
    param (
        [string]
        $ProjectPath,

        [string]
        $PackageVersion,

        [string]
        $nuspecPath
    )

    # NU5100 is the warning about DLLs outside of a "lib" folder. We're
    # deliberately using that pattern, therefore we don't care about the
    # warning.
    dotnet pack $ProjectPath --output $PSScriptRoot -p:NuspecFile=$nuspecPath -p:NuspecProperties="version=$PackageVersion" /p:NoWarn=NU5100
}

function NewDevCertificate {
    Invoke-Command { dotnet dev-certs https -c }
    if ($lastexitcode) {
        Write-Output "Generating a new Dev Certificate"
        Invoke-Execute { dotnet dev-certs https --clean }
        Invoke-Execute { dotnet dev-certs https -t }
    }
    else {
        Write-Output "Dev Certificate already exists" 
    }
}

function AddAppCommonPackageForInstaller {
    $project = "EdFi.Ods.AdminApi"
    $mainPath = "$solutionRoot/$project"
    $destinationPath = "$mainPath/publish"

    $arguments = @{
        AppCommonPackageName = $appCommonPackageName
        AppCommonPackageVersion = $appCommonPackageVersion
        NuGetFeed = $EdFiNuGetFeed
        DestinationPath = $destinationPath
    }

    Add-AppCommon @arguments
}

function BuildApiPackage {
    $project = "EdFi.Ods.AdminApi"
    $mainPath = "$solutionRoot/$project"
    $projectPath = "$mainPath/$project.csproj"
    $nugetSpecPath = "$mainPath/publish/$project.nuspec"

    RunNuGetPack -ProjectPath $projectPath -PackageVersion $APIVersion $nugetSpecPath
}

function Invoke-Build {
    Invoke-Step { DotNetClean }
    Invoke-Step { Restore }
    Invoke-Step { Compile }
}

function Invoke-SetAssemblyInfo {
    Write-Output "Setting Assembly Information"

    Invoke-Step { SetAdminApiAssemblyInfo }
}

function Invoke-Publish {
    Write-Output "Building Version AdminApi ($APIVersion)"

    Invoke-Step { PublishAdminApi }
}

function Invoke-Run {
    Write-Output "Running Admin API"

    Invoke-Step { NewDevCertificate }

    $projectFilePath = "$solutionRoot/EdFi.Ods.AdminApi"

    if ([string]::IsNullOrEmpty($LaunchProfile)) {
        Write-Error "LaunchProfile parameter is required for running Admin Api. Please " +
                    "specify the LaunchProfile parameter. Valid values include ""EdFi.Ods.AdminApi (Dev)""" +
                    ", ""EdFi.Ods.AdminApi (Prod)"", ""EdFi.Ods.AdminApi (Docker)"", and ""IIS Express"""
    }
    else {
        Invoke-Execute { dotnet run --project $projectFilePath --launch-profile $LaunchProfile }
    }
}

function Invoke-Clean {
    Invoke-Step { DotNetClean }
}

function Invoke-UnitTestSuite {
    Invoke-Step { UnitTests }
}

function Invoke-IntegrationTestSuite {
    Invoke-Step { InitializeNuGet }

    $supportedApiVersions | ForEach-Object {
        Write-Output "Running Integration Tests for ODS Version" $_.OdsVersion

        Invoke-Step {
            $arguments = @{
                OdsVersion = $_.OdsVersion
                OdsPackageName = $_.OdsPackageName
                Prerelease = $_.Prerelease
            }
            ResetTestDatabases @arguments
        }
        Invoke-Step {
            IntegrationTests
        }
    }
}

function Invoke-BuildApiPackage {
    Invoke-Step { AddAppCommonPackageForInstaller }
    Invoke-Step { BuildApiPackage }
}

function Invoke-BuildDatabasePackage {
    Invoke-Step { BuildDatabasePackage }
}

function UpdateAppSettingsForAdminApiDocker {
    $filePath = "$solutionRoot/EdFi.Ods.AdminApi/publish/appsettings.json"
    $json = (Get-Content -Path $filePath) | ConvertFrom-Json
    $json.AppSettings.ProductionApiUrl = $DockerEnvValues["ProductionApiUrl"]
    $json.AppSettings.ApiExternalUrl = $DockerEnvValues["ApiExternalUrl"]
    $json.AppSettings.ApiStartupType = $DockerEnvValues["ApiStartupType"]
    $json.AppSettings.DatabaseEngine = $DockerEnvValues["DatabaseEngine"]
    $json.AppSettings.PathBase = $DockerEnvValues["PathBase"]

    $json.Authentication.IssuerUrl = $DockerEnvValues["IssuerUrl"]
    $json.Authentication.SigningKey = $DockerEnvValues["SigningKey"]

    $json.ConnectionStrings.Admin = $DockerEnvValues["AdminDB"]
    $json.ConnectionStrings.Security = $DockerEnvValues["SecurityDB"]
    $json.ConnectionStrings.ProductionOds = $DockerEnvValues["ProductionOdsDB"]
    $json.Log4NetCore.Log4NetConfigFileName =  "./log4net.config"
    $json | ConvertTo-Json -Depth 10 | Set-Content $filePath
}

function CopyLatestFilesToAdminApiContainer {
    $source = "$solutionRoot/EdFi.Ods.AdminApi/publish/."
    &docker cp $source adminapi:/app/AdminApi
}

function RestartAdminApiContainer {
    &docker restart adminapi
}

function BuildAdminApiDevDockerImage {
    &docker build -t adminapi-dev --no-cache -f "$solutionRoot/EdFi.Ods.AdminApi/dev.Dockerfile" .
}

function RunAdminApiDevDockerContainer {
    &docker run --env-file "$solutionRoot/EdFi.Ods.AdminApi/.env" -p 80:80 -v "$solutionRoot/EdFi.Ods.AdminApi/Docker/ssl:/ssl/" adminapi-dev
}

function RunAdminApiDevDockerCompose {
    &docker compose -f "$solutionRoot/EdFi.Ods.AdminApi/Compose/pgsql/compose-build-dev.yml" --env-file "$solutionRoot/EdFi.Ods.AdminApi/E2E Tests/gh-action-setup/.automation.env" -p "ods_admin_api" up -d
}

function Invoke-AdminApiDockerDeploy {
   Invoke-Step { UpdateAppSettingsForAdminApiDocker }
   Invoke-Step { CopyLatestFilesToAdminApiContainer }
   Invoke-Step { RestartAdminApiContainer }
}

function Invoke-BuildAdminApiDevDockerImage {
   Invoke-Step { BuildAdminApiDevDockerImage }
}

function Invoke-RunAdminApiDevDockerContainer {
   Invoke-Step { RunAdminApiDevDockerContainer }
}

function Invoke-RunAdminApiDevDockerCompose {
   Invoke-Step { RunAdminApiDevDockerCompose }
}

Invoke-Main {
    if($IsLocalBuild)
    {
        $nugetExePath = Install-NugetCli       
        Set-Alias nuget $nugetExePath -Scope Global -Verbose
    }
    switch ($Command) {
        Clean { Invoke-Clean }
        Build { Invoke-Build }
        BuildAndPublish {
            Invoke-SetAssemblyInfo
            Invoke-Build
            Invoke-Publish
        }
        Run { Invoke-Run }
        UnitTest { Invoke-UnitTestSuite }
        IntegrationTest { Invoke-IntegrationTestSuite }
        BuildAndTest {
            Invoke-Build
            Invoke-UnitTestSuite
            Invoke-IntegrationTestSuite
        }
        Package { Invoke-BuildPackage }
        PackageApi { Invoke-BuildApiPackage }
        Push { Invoke-PushPackage }
        BuildAndDeployToAdminApiDockerContainer {
            Invoke-Build
            Invoke-AdminApiDockerDeploy
        }
        BuildAndRunAdminApiDevDocker{
            Invoke-BuildAdminApiDevDockerImage
            Invoke-RunAdminApiDevDockerContainer
        }
        RunAdminApiDevDockerContainer{
            Invoke-RunAdminApiDevDockerContainer
        }
        RunAdminApiDevDockerCompose{
            Invoke-RunAdminApiDevDockerCompose
        }
        PopulateGoogleAnalyticsAppSettings {
            Update-AppSettingsToAddGoogleAnalyticsMeasurementId
        }
        default { throw "Command '$Command' is not recognized" }
    }
}
