# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# This refers to function Update-AppSettingsToAddGoogleAnalyticsMeasurementId
[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseShouldProcessForStateChangingFunctions', '', Justification = 'Rule is irrelevant in this context', Scope = "Function")]

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
        * BuildAndDeployToAdminAppDockerContainer: runs the build operation, update the appsettings.json with provided
          DockerEnvValues and copy over the latest files to existing AdminApp docker container for testing.
        * BuildAndDeployToAdminApiDockerContainer: runs the build operation, update the appsettings.json with provided
          DockerEnvValues and copy over the latest files to existing AdminApi docker container for testing.
        * PopulateGoogleAnalyticsAppSettings: update the appsettings.json with provided GoogleAnalyticsMeasurementId.

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
            AdminDB = "host=db-admin;port=5432;username=username;password=password;database=EdFi_Admin;Application Name=EdFi.Ods.AdminApp;"
            SecurityDB = "host=db-admin;port=5432;username=username;password=password;database=EdFi_Security;Application Name=EdFi.Ods.AdminApp;"
            ProductionOdsDB = "host=db-ods;port=5432;username=username;password=password;database=EdFi_{0};Application Name=EdFi.Ods.AdminApp;"
        }

        .\build.ps1 -Version "2.1" -Configuration Release -DockerEnvValues $p -Command BuildAndDeployToAdminAppDockerContainer
#>
[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', '', Justification = 'False positive')]
param(
    # Command to execute, defaults to "Build".
    [string]
    [ValidateSet("Clean", "Build", "BuildAndPublish", "UnitTest", "IntegrationTest", "Package", "PackageApi"
    , "PackageDatabase", "Push", "BuildAndTest", "BuildAndDeployToAdminAppDockerContainer", "BuildAndDeployToAdminApiDockerContainer"
    , "BuildAndRunAdminApiDevDocker", "RunAdminApiDevDockerContainer", "RunAdminApiDevDockerCompose", "PopulateGoogleAnalyticsAppSettings", "Run")]
    $Command = "Build",

    # Assembly and package version number for AdminApp Web. The current package number is
    # configured in the build automation tool and passed to this script.
    [string]
    $Version = "0.1",

    # Assembly and package version number for Admin API. The current package number is
    # configured in the build automation tool and passed to this script.
    [string]
    $APIVersion = $Version,

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
    # NuGet package corresponding to the provided $Version and $BuildCounter.
    [string]
    $PackageFile,

    # Environment values for updating the appsettings on existing AdminApp docker container.

    # Only required with the BuildAndDeployToAdminAppDockerContainer or BuildAndDeployToAdminApiDockerContainer commands.
    [hashtable]
    $DockerEnvValues,

    # Only required with the PopulateGoogleAnalyticsAppSettings command.
    [string]
    $GoogleAnalyticsMeasurementId,

    # Only required with the Run command.
    [string]
    [ValidateSet("mssql-district", "mssql-shared", "mssql-year", "pg-district", "pg-shared", "pg-year")]
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
function Clean {
    Invoke-Execute { dotnet clean $solutionRoot -c $Configuration --nologo -v minimal }
}
function InitializeNuGet {
    Invoke-Execute { $script:nugetExe = Install-NugetCli }
}

function Restore {
    Invoke-Execute { dotnet restore $solutionRoot }
}

function SetAdminAppAssemblyInfo {
    Invoke-Execute {
        $assembly_version = $Version

        Invoke-RegenerateFile "$solutionRoot/Directory.Build.props" @"
<Project>
    <!-- This file is generated by the build script. -->
    <PropertyGroup>
        <Product>Ed-Fi ODS Admin App</Product>
        <Authors>$maintainers</Authors>
        <Company>$maintainers</Company>
        <Copyright>Copyright © 2016 Ed-Fi Alliance</Copyright>
        <VersionPrefix>$assembly_version</VersionPrefix>
        <VersionSuffix></VersionSuffix>
    </PropertyGroup>
</Project>

"@
    }
}

function SetAdminApiAssemblyInfo {
    Invoke-Execute {
        $assembly_version = $APIVersion

        Invoke-RegenerateFile "$solutionRoot/EdFi.Ods.Admin.Api/Directory.Build.props" @"
<Project>
    <!-- This file is generated by the build script. -->
    <PropertyGroup>
        <Product>Ed-Fi ODS Admin API</Product>
        <Authors>$maintainers</Authors>
        <Company>$maintainers</Company>
        <Copyright>Copyright © 2022 Ed-Fi Alliance</Copyright>
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

function PublishAdminApp {
    Invoke-Execute {
        $project = "$solutionRoot/EdFi.Ods.AdminApp.Web/"
        $outputPath = "$project/publish"
        dotnet publish $project -c $Configuration /p:EnvironmentName=Production -o $outputPath --no-build --nologo
    }
}

function PublishAdminApi {
    Invoke-Execute {
        $project = "$solutionRoot/EdFi.Ods.Admin.Api/"
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
    Invoke-Execute { RunTests -Filter "*.Tests" }
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
        Write-Output "Generating a new Dev Certificate" -ForegroundColor Magenta
        Invoke-Execute { dotnet dev-certs https --clean }
        Invoke-Execute { dotnet dev-certs https -t }
    }
    else {
        Write-Output "Dev Certificate already exists" -ForegroundColor Magenta
    }
}

function AddAppCommonPackageForInstaller {
    $project = "EdFi.Ods.Admin.Api"
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

function BuildDatabasePackage {
    $project = "EdFi.Ods.AdminApp.Web"
    $mainPath = "$solutionRoot/$project"
    $projectPath = "$mainPath/$project.csproj"
    $nugetSpecPath = "$mainPath/publish/EdFi.Ods.AdminApp.Database.nuspec"

    RunNuGetPack -ProjectPath $projectPath -PackageVersion $Version $nugetSpecPath
}

function BuildAdminAppPackage {
    $project = "EdFi.Ods.AdminApp.Web"
    $mainPath = "$solutionRoot/$project"
    $projectPath = "$mainPath/$project.csproj"
    $nugetSpecPath = "$mainPath/publish/$project.nuspec"

    RunNuGetPack -ProjectPath $projectPath -PackageVersion $Version $nugetSpecPath
}

function BuildApiPackage {
    $project = "EdFi.Ods.Admin.Api"
    $mainPath = "$solutionRoot/$project"
    $projectPath = "$mainPath/$project.csproj"
    $nugetSpecPath = "$mainPath/publish/$project.nuspec"

    RunNuGetPack -ProjectPath $projectPath -PackageVersion $APIVersion $nugetSpecPath
}

function PushPackage {
    if (-not $NuGetApiKey) {
        throw "Cannot push a NuGet package without providing an API key in the `NuGetApiKey` argument."
    }

    if (-not $PackageFile) {
        $PackageFile = "$PSScriptRoot/EdFi.Ods.AdminApp.Web.$(GetPackageVersion).nupkg"
    }

    $arguments = @{
        PackageFile = $PackageFile
        NuGetApiKey = $NuGetApiKey
        NuGetFeed = $EdFiNuGetFeed
    }

    Invoke-Execute { Push-Package @arguments }
}

function Invoke-Build {
    Invoke-Step { Clean }
    Invoke-Step { Restore }
    Invoke-Step { Compile }
}

function Invoke-SetAssemblyInfo {
    Write-Output "Setting Assembly Information" -ForegroundColor Cyan

    Invoke-Step { SetAdminAppAssemblyInfo }
    Invoke-Step { SetAdminApiAssemblyInfo }
}

function Invoke-Publish {
    Write-Output "Building Version AdminApp ($Version) and AdminApi ($APIVersion)" -ForegroundColor Cyan

    Invoke-Step { PublishAdminApp }
    Invoke-Step { PublishAdminApi }
}

function Invoke-Run {
    Write-Output "Running Admin App" -ForegroundColor Cyan

    Invoke-Step { NewDevCertificate }

    $projectFilePath = "$solutionRoot/EdFi.Ods.AdminApp.Web"

    if ([string]::IsNullOrEmpty($LaunchProfile)) {
        Write-Output "LaunchProfile parameter is required for running Admin App. Please specify the LaunchProfile parameter. Valid values include 'mssql-district', 'mssql-shared', 'mssql-year', 'pg-district', 'pg-shared' and 'pg-year'" -ForegroundColor Red
    }
    else {
        Invoke-Execute { dotnet run --project $projectFilePath --launch-profile $LaunchProfile }
    }
}

function Invoke-Clean {
    Invoke-Step { Clean }
}

function Invoke-UnitTestSuite {
    Invoke-Step { UnitTests }
}

function Invoke-IntegrationTestSuite {
    Invoke-Step { InitializeNuGet }

    $supportedApiVersions | ForEach-Object {
        Write-Output "Running Integration Tests for ODS Version" $_.OdsVersion -ForegroundColor Cyan

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

function Invoke-BuildPackage {
    Invoke-Step { BuildAdminAppPackage }
}

function Invoke-BuildApiPackage {
    Invoke-Step { AddAppCommonPackageForInstaller }
    Invoke-Step { BuildApiPackage }
}

function Invoke-BuildDatabasePackage {
    Invoke-Step { BuildDatabasePackage }
}

function Invoke-PushPackage {
    Invoke-Step { PushPackage }
}

function Update-AppSettingsToAddGoogleAnalyticsMeasurementId {
    $filePath = "$solutionRoot/EdFi.Ods.AdminApp.Web/publish/appsettings.json"
    $json = (Get-Content -Path $filePath) | ConvertFrom-Json
    $json.AppSettings.GoogleAnalyticsMeasurementId = $GoogleAnalyticsMeasurementId
    $json | ConvertTo-Json | Set-Content $filePath
}

function UpdateAppSettingsForAdminAppDocker {
    $filePath = "$solutionRoot/EdFi.Ods.AdminApp.Web/publish/appsettings.json"
    $json = (Get-Content -Path $filePath) | ConvertFrom-Json
    $json.AppSettings.ProductionApiUrl = $DockerEnvValues["ProductionApiUrl"]
    $json.AppSettings.ApiExternalUrl = $DockerEnvValues["ApiExternalUrl"]
    $json.AppSettings.AppStartup = $DockerEnvValues["AppStartup"]
    $json.AppSettings.ApiStartupType = $DockerEnvValues["ApiStartupType"]
    $json.AppSettings.XsdFolder = $DockerEnvValues["XsdFolder"]
    $json.AppSettings.DatabaseEngine = $DockerEnvValues["DatabaseEngine"]
    $json.AppSettings.BulkUploadHashCache = $DockerEnvValues["BulkUploadHashCache"]
    $json.AppSettings.PathBase = $DockerEnvValues["PathBase"]

    if ($null -eq $json.AppSettings.EncryptionKey) {
        $json.AppSettings | Add-Member -NotePropertyName EncryptionKey -NotePropertyValue $DockerEnvValues["EncryptionKey"]
    }
    else
    {
        $json.AppSettings.EncryptionKey = $DockerEnvValues["EncryptionKey"]
    }

    $json.ConnectionStrings.Admin = $DockerEnvValues["AdminDB"]
    $json.ConnectionStrings.Security = $DockerEnvValues["SecurityDB"]
    $json.ConnectionStrings.ProductionOds = $DockerEnvValues["ProductionOdsDB"]
    $json.Log4NetCore.Log4NetConfigFileName =  "./log4net.config"
    $json | ConvertTo-Json | Set-Content $filePath
}

function UpdateAppSettingsForAdminApiDocker {
    $filePath = "$solutionRoot/EdFi.Ods.Admin.Api/publish/appsettings.json"
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

function CopyLatestFilesToAdminAppContainer {
    $source = "$solutionRoot/EdFi.Ods.AdminApp.Web/publish/."
    docker cp $source ed-fi-ods-adminapp:/app
}

function CopyLatestFilesToAdminApiContainer {
    $source = "$solutionRoot/EdFi.Ods.Admin.Api/publish/."
    &docker cp $source adminapi:/app/AdminApi
}

function RestartAdminAppContainer {
    &docker restart ed-fi-ods-adminapp
}

function RestartAdminApiContainer {
    &docker restart adminapi
}

function BuildAdminApiDevDockerImage {
    &docker build -t adminapi-dev --no-cache -f "$solutionRoot/EdFi.Ods.Admin.Api/dev.Dockerfile" .
}

function RunAdminApiDevDockerContainer {
    &docker run --env-file "$solutionRoot/EdFi.Ods.Admin.Api/.env" -p 80:80 -v "$solutionRoot/EdFi.Ods.Admin.Api/Docker/ssl:/ssl/" adminapi-dev
}

function RunAdminApiDevDockerCompose {
    &docker compose -f "$solutionRoot/EdFi.Ods.Admin.Api/Compose/pgsql/compose-build-dev.yml" --env-file "$solutionRoot/EdFi.Ods.Admin.Api/E2E Tests/gh-action-setup/.automation.env" -p "ods_admin_api" up -d
}

function Invoke-AdminAppDockerDeploy {
   Invoke-Step { UpdateAppSettingsForAdminAppDocker }
   Invoke-Step { CopyLatestFilesToAdminAppContainer }
   Invoke-Step { RestartAdminAppContainer }
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
        PackageDatabase { Invoke-BuildDatabasePackage }
        Push { Invoke-PushPackage }
        BuildAndDeployToAdminAppDockerContainer {
            Invoke-Build
            Invoke-AdminAppDockerDeploy
        }
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
