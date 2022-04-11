// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object PromotePackagesToReleaseOnAzureArtifacts : BuildType ({
    name = "Promote Packages to Release on Azure Artifacts"
    description = "Promote Packages to Release on Azure Artifacts"

    publishArtifacts = PublishMode.SUCCESSFUL

    option("shouldFailBuildOnAnyErrorMessage", "true")

    params {
        param("promote.packages.view", "placeholder")
        param("env.VSS_NUGET_EXTERNAL_FEED_ENDPOINTS", "{\"endpointCredentials\": [{\"endpoint\": \"%azureArtifacts.feed.nuget%\",\"username\": \"%azureArtifacts.edFiBuildAgent.userName%\",\"password\": \"%azureArtifacts.edFiBuildAgent.accessToken%\"}]}")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Promote Packages"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = script {
                content = """
                    ${'$'}Version = "%adminApp.version%.%build.counter%"
                    ${'$'}Packages =  @{ }
                    ${'$'}Packages.add("EdFi.Suite3.ODS.AdminApp.Database".ToLower().Trim(), ${'$'}Version)
                    ${'$'}Packages.add("EdFi.Suite3.ODS.AdminApp.Web".ToLower().Trim(), ${'$'}Version)

                    ${'$'}arguments = @{
                    	 PackagesURL = "%azureArtifacts.api.packaging%"
                         Username    = "%azureArtifacts.edFiBuildAgent.userName%"
                         Password    = (ConvertTo-SecureString -String "%azureArtifacts.edFiBuildAgent.accessToken%" -AsPlainText -Force)
                         View        = "%promote.packages.view%"
                         Packages    = ${'$'}Packages
                    }
                    eng\promote-packages.ps1 @arguments
                """.trimIndent()
            }
        }
    }


    // This deliberately does not have a trigger - should only be run manually.
})
