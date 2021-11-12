# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

import-module -force "$PSScriptRoot/Install-EdFiOdsAdminApp.psm1"

<#
This script will take your existing Admin App installation and upgrade it to the version indicated by PackageVersion below.
Your existing appsettings.json config values and connection strings will be copied forward to the new version.

.EXAMPLE
    $p = @{
    ToolsPath = "C:/temp/tools"
    PackageVersion = '2.3.1' }
#>

$p = @{
    ToolsPath = "C:/temp/tools"
    PackageVersion = '2.3.1'
}

Update-EdFiOdsAdminApp @p

