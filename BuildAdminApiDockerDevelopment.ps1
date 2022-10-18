# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

<#
    .SYNOPSIS
        Script for running the build operation and copy over the latest files to an existing AdminApi docker container for testing.

    .DESCRIPTION
        Script for facilitating the local docker testing with latest changes. Developer can set the required appsettings values and trigger
        the build. Once the build done, the apsettings.json will be updated with values provided and
        latest files will be copied over to an existing AdminApi docker container folder.

    .EXAMPLE
        .\BuildAdminApiDockerDevelopment.ps1
#>

$p = @{
        ProductionApiUrl = "http://api"
        ApiExternalUrl = "https://localhost:5001"
        ApiStartupType = "SharedInstance"
        DatabaseEngine = "PostgreSql"
        PathBase = "adminapp"
        IssuerUrl = "https://localhost:8001"
        SigningKey = ""
        AdminDB = "host=db-admin;port=5432;username=username;password=root@321;database=EdFi_Admin;pooling=false"
        SecurityDB = "host=db-admin;port=5432;username=username;password=root@321;database=EdFi_Security;pooling=false"
        ProductionOdsDB = "host=db-ods;port=5432;username=username;password=password;database=EdFi_{0};pooling=false"
    }

.\build.ps1 -APIVersion 1.0.6 -Configuration Release -DockerEnvValues $p -Command BuildAndDeployToAdminApiDockerContainer
