# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

$ErrorActionPreference = "Stop"
<#
.DESCRIPTION
Promotes a package in Azure Artifacts to a view, e.g. pre-release or release.
#>
function Invoke-Promote {
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', '', Justification = 'False positive')]
    param(
        # NuGet package feed / source
        [Parameter(Mandatory = $true)]
        [String]
        $FeedsURL,

        # NuGet Packages API URL
        [Parameter(Mandatory = $true)]
        [String]
        $PackagesURL,

        # Azure Artifacts user name
        [Parameter(Mandatory = $true)]
        [String]
        $Username,

        # Azure Artifacts password
        [Parameter(Mandatory = $true)]
        [SecureString]
        $Password,

        # View to promote into
        [Parameter(Mandatory = $true)]
        [String]
        $View,

        # Git ref (short) for the release tag
        [Parameter(Mandatory = $true)]
        $ReleaseRef
    )

    $package = "EdFi.Suite3.ODS.AdminApi"
    $version = $ReleaseRef.substring(1)

    $body = @{
        views = @{
            op    = 'add'
            path  = '/views/-'
            value = 'Release'
        }
    }

    $uri = "$FeedsURL/packages/$package/versions/$version"

    Write-Output $uri

    $parameters = @{
        Method      = "PATCH"
        ContentType = "application/json-patch+json"
        Credential  = New-Object -TypeName PSCredential -ArgumentList $Username, $Password
        URI         = $uri + "?api-version=7.1-preview.1"
        Body        = ConvertTo-Json $Body -Depth 2
    }

    $parameters | Out-Host
    $parameters.URI | Out-Host
    $parameters.Body | Out-Host

    $response = Invoke-WebRequest @parameters -UseBasicParsing
    $response | ConvertTo-Json -Depth 10 | Out-Host
}

Export-ModuleMember -Function Invoke-Promote
