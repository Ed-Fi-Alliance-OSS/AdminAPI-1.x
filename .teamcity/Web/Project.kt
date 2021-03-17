// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package web

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object AdminAppWebProject : Project({
    name = "Admin App Web"
    id = RelativeId("AdminAppWeb")
    description = "ODS Admin App Build Configurations"

    params {
        param("adminApp.version", "2.2.0")
    }

    template(_self.templates.BuildAndTestTemplate)

    buildType(_self.buildTypes.BuildBranch)
    buildType(_self.buildTypes.BuildPullRequests)
    buildType(_self.buildTypes.PublishPackagesToAzureArtifacts)
    buildType(_self.buildTypes.PublishReleasePackagesToAzureArtifacts)
})
