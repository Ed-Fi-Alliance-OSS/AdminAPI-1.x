// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object PublishReleasePackagesToAzureArtifacts : BuildType ({
    name = "Publish Release Packages to Azure Artifacts"
    description = "Publishes NuGet packages to the Azure Artifacts"

    publishArtifacts = PublishMode.SUCCESSFUL

    option("shouldFailBuildOnAnyErrorMessage", "true")

    params {
        param("nuGet.packageFile.database", "EdFi.Suite3.ODS.AdminApp.Database.%adminApp.version%.nupkg")
        param("nuGet.packageFile.web", "EdFi.Suite3.ODS.AdminApp.Web.%adminApp.version%.nupkg")
        param("env.VSS_NUGET_EXTERNAL_FEED_ENDPOINTS", "{\"endpointCredentials\": [{\"endpoint\": \"%azureArtifacts.feed.nuget%\",\"username\": \"%azureArtifacts.edFiBuildAgent.userName%\",\"password\": \"%azureArtifacts.edFiBuildAgent.accessToken%\"}]}")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Publish Application package to Azure Artifacts"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = script {
                content = """
                    ${'$'}arguments = @{
                    	PackageFile = "%nuGet.packageFile.web%"
                        EdFiNuGetFeed = "%azureArtifacts.feed.nuget%"
                        NuGetApiKey = "%azureArtifacts.edFiBuildAgent.accessToken%"
                    }
                    ./build.ps1 Push @arguments
                """.trimIndent()
            }
        }
        powerShell {
            name = "Publish Database package to Azure Artifacts"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = script {
                content = """
                    ${'$'}arguments = @{
                    	PackageFile = "%nuGet.packageFile.database%"
                        EdFiNuGetFeed = "%azureArtifacts.feed.nuget%"
                        NuGetApiKey = "%azureArtifacts.edFiBuildAgent.accessToken%"
                    }
                    ./build.ps1 Push @arguments
                """.trimIndent()
            }
        }
    }


    // This deliberately does not have a trigger - should only be run manually.

    features {
        swabra {
            forceCleanCheckout = true
        }
    }

    dependencies {
        dependency(BuildBranch) {
            snapshot {
            }

            artifacts {
                artifactRules = """
                    +:%nuGet.packageFile.database% =>
                    +:%nuGet.packageFile.web% =>
                """.trimIndent()
            }
        }
    }
})
