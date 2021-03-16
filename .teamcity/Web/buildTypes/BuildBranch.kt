// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.VcsTrigger
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

object BuildBranch : BuildType ({
    name = "Branch Build and Test"
    description = "Runs build and test operations on the default or other branches"
    templates(_self.templates.BuildAndTestTemplate)

    publishArtifacts = PublishMode.SUCCESSFUL
    artifactRules = "+:*.nupkg"

    triggers {
        vcs {
            id ="vcsTrigger"
            quietPeriodMode = VcsTrigger.QuietPeriodMode.USE_CUSTOM
            quietPeriod = 120
            branchFilter = "+:<default>"
        }
    }
})
