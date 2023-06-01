# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', '', Justification = 'False positive')]
param(

    [Parameter(Mandatory = $true)]
    $FeedsURL,

    [Parameter(Mandatory = $true)]
    $PackagesURL,

    [Parameter(Mandatory = $true)]
    $Username,

    [Parameter(Mandatory = $true)]
    [SecureString] $Password,

    [Parameter(Mandatory = $true)]
    $View,

    # Git ref (short) for the release tag
    [Parameter(Mandatory = $true)]
    $ReleaseRef
)

function Get-PackagesFromAzure {

    $uri = "$FeedsURL/packages?api-version=6.0-preview.1"
    $result = @{ }

    foreach ($packageName in $Packages) {
        $packageQueryUrl = "$uri&packageNameQuery=$packageName"
        $packagesResponse = (Invoke-WebRequest -Uri $packageQueryUrl -UseBasicParsing).Content | ConvertFrom-Json
        $latestPackageVersion = ($packagesResponse.value.versions | Where-Object { $_.isLatest -eq $True } | Select-Object -ExpandProperty version)

        Write-Output "Package Name: $packageName"
        Write-Output "Package Version: $latestPackageVersion"

        $result.add(
            $packageName.ToLower().Trim(),
            $latestPackageVersion
        )
    }
    return $result
}

$ErrorActionPreference = 'Stop'

# example: "AdminApi-v2.5.1"
# output: "AdminApi"
$packageName = ($ReleaseRef -split "-v")[0]

switch ($packageName) {
    "AdminApi" {
        $packages = @( "EdFi.Suite3.ODS.AdminApi" )
    }
    "AdminApi.Installer" {
        $packages = @( "EdFi.Suite3.Installer.AdminApi" )
    }
}

$body = @{
    data      = @{
        viewId = $View
    }
    operation = 0
    packages  = @()
}

$latestPackages = Get-PackagesFromAzure

foreach ($key in $latestPackages.Keys) {
    $body.packages += @{
        id           = $key
        version      = $latestPackages[$key]
        protocolType = "NuGet"
    }
}

$parameters = @{
    Method      = "POST"
    ContentType = "application/json"
    Credential  = New-Object -TypeName PSCredential -ArgumentList $Username, $Password
    URI         = "$PackagesURL/nuget/packagesBatch?api-version=5.0-preview.1"
    Body        = ConvertTo-Json $Body -Depth 10
}

$parameters | Out-Host
$parameters.URI | Out-Host
$parameters.Body | Out-Host

$response = Invoke-WebRequest @parameters -UseBasicParsing
$response | ConvertTo-Json -Depth 10 | Out-Host
