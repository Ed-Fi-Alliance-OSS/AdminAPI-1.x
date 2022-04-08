# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

[cmdletbinding(SupportsShouldProcess)]
param( 

    [Parameter(Mandatory = $true)]
    $PackagesURL,

    [Parameter(Mandatory = $true)]
    $Username,

    [Parameter(Mandatory = $true)]
    [SecureString] $Password,

    [Parameter(Mandatory = $true)]
    $View,

    [Parameter(Mandatory = $true)]       
    $Packages
)

$ErrorActionPreference = 'Stop'

$body = @{
    data      = @{
        viewId = $View
    }
    operation = 0
    packages  = @()
}

foreach ($key in $Packages.Keys) {
    $body.packages += @{
        id           = $key
        version      = $Packages[$key]
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

if ($PSCmdlet.ShouldProcess($PackagesURL)) {
    $response = Invoke-WebRequest @parameters
    $response | ConvertTo-Json -Depth 10 | Out-Host
}

Write-TeamCityBuildStatus "Packages promoted: $($Packages.Count)"