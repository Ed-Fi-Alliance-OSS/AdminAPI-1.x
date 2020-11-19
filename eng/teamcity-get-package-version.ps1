# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# This script helps TeamCity lookup the package version from an artifact that
# was copied over from an upstream dependency.

# There should only be one file here - the pre-release NuGet package from the upstream build artifacts
$pkg = Get-ChildItem -Filter "*Core*pre*.nupkg"

if (-not $pkg) {
    throw "No pre-release package found."
}

# Extract the version number
$result = $pkg -match "Web.Core\.(\d)\.(\d)\.(\d)\-pre(\d+)\.nupkg"

if (-not $result) {
    throw "Package name does not match the expected naming convention."
}

$release = $matches[1]+"."+$matches[2]+"."+$matches[3]+"-pre"+$matches[4]

Write-Host "##teamcity[setParameter name='nuGet.packageFile' value='$pkg']"
Write-Host "##teamcity[setParameter name='nuGet.packageVersion' value='$release']"
Write-Host "##teamcity[setParameter name='octopus.release' value='$release']"
Write-Host "##teamcity[setParameter name='octopus.package' value='$pkg']"