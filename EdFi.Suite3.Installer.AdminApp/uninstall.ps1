# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

import-module -force "$PSScriptRoot/Install-EdFiOdsAdminApp.psm1"

<#
This script will uninstall your existing Admin App installation.

.EXAMPLE
    $p = @{
        ToolsPath = "C:/temp/tools"
        WebSiteName="Ed-Fi-3"
        WebApplicationPath="d:/edfi/applications/staging/AdminApp-3"
        WebApplicationName = "AdminApp"
    }
#>

$p = @{
    ToolsPath = "C:/temp/tools"
}

Uninstall-EdFiOdsAdminApp @p

