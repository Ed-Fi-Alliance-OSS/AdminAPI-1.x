// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell

object PublishInstallerPackageToAzureArtifacts : BuildType({
    name = "Publish Installer Package To Azure Artifacts"

    enablePersonalBuilds = false
    type = BuildTypeSettings.Type.DEPLOYMENT
    maxRunningBuilds = 1
    params {
        param("env.VSS_NUGET_EXTERNAL_FEED_ENDPOINTS", """{"endpointCredentials": [{"endpoint": "%azureArtifacts.feed.nuget%","username": "%azureArtifacts.edFiBuildAgent.userName%","password": "%azureArtifacts.edFiBuildAgent.accessToken%"}]}""")
    }
    steps {
        powerShell {
            name = "Publish to Azure Artifacts"
            workingDir = "%project.directory%"
            scriptMode = script {
                content = "nuget push -source %azureArtifacts.feed.nuget% -apikey az *.nupkg"
            }
        }
    }

    dependencies {
        artifacts(BuildAdminAppInstaller) {
            buildRule = lastSuccessful()
            artifactRules = """
                +:**/*.nupkg => .
                -:**/*-pre*.nupkg
            """.trimIndent()
        }
    }
})
