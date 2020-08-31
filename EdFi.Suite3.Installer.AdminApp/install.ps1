# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

$dbConnectionInfo = @{
    Server = "(local)"
    Engine = "SqlServer"
    UseIntegratedSecurity=$true
}

$p = @{
    ToolsPath = "C:/temp/tools"
    DbConnectionInfo = $dbConnectionInfo
    OdsApiUrl = "https://localhost:54746"
    PackageVersion = '2.0.0-pre0040'
}

Install-EdFiOdsAdminApp @p