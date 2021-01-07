// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object PublishPackagesToAzureArtifacts : BuildType ({
    name = "Publish Packages to Azure Artifacts"
    description = "Publishes NuGet packages to the Azure Artifacts"

    publishArtifacts = PublishMode.SUCCESSFUL

    option("shouldFailBuildOnAnyErrorMessage", "true")

    params {
        param("nuGet.packageFile", "placeholder")
        param("nuGet.packageVersion", "placeholder")
        param("env.VSS_NUGET_EXTERNAL_FEED_ENDPOINTS", "{\"endpointCredentials\": [{\"endpoint\": \"%azureArtifacts.feed.nuget%\",\"username\": \"%azureArtifacts.edFiBuildAgent.userName%\",\"password\": \"%azureArtifacts.edFiBuildAgent.accessToken%\"}]}")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Lookup Application Package Name and Version"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = file {
                path = """eng\teamcity-get-package-version.ps1"""
            }
        }
        powerShell {
            name = "Publish Application package to Azure Artifacts"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = script {
                content = """
                    ${'$'}arguments = @{
                    	PackageFile = "%nuGet.packageFile%"
                        EdFiNuGetFeed = "%azureArtifacts.feed.nuget%"
                        NuGetApiKey = "%azureArtifacts.edFiBuildAgent.accessToken%"
                    }
                    ./build.ps1 Push @arguments
                """.trimIndent()
            }
        }
        powerShell {
            name = "Lookup Database Package Name and Version"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = script {
                content = """eng\teamcity-get-package-version.ps1 -packageType Database""".trimIndent()
            }
        }
        powerShell {
            name = "Publish Database package to Azure Artifacts"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = script {
                content = """
                    ${'$'}arguments = @{
                    	PackageFile = "%nuGet.packageFile%"
                        EdFiNuGetFeed = "%azureArtifacts.feed.nuget%"
                        NuGetApiKey = "%azureArtifacts.edFiBuildAgent.accessToken%"
                    }
                    ./build.ps1 Push @arguments
                """.trimIndent()
            }
        }
    }
  triggers {
        finishBuildTrigger {
            buildType = "${BuildBranch.id}"
        }
    }

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
                    +:*pre*.nupkg =>
                """.trimIndent()
            }
        }
    }
})
