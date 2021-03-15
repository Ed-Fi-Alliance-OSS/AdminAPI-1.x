// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package installers.templates

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

object BuildAdminAppInstaller : BuildType {
    init {
        name = "Build Admin App Installer Package"

        artifactRules = "%project.directory%/*.nupkg"
        publishArtifacts = PublishMode.SUCCESSFUL
        buildNumberPattern = "%version%"

        option("shouldFailBuildOnAnyErrorMessage", "true")

        params {
            param("env.VSS_NUGET_EXTERNAL_FEED_ENDPOINTS", "{\"endpointCredentials\": [{\"endpoint\": \"%azureArtifacts.feed.nuget%\",\"username\": \"%azureArtifacts.edFiBuildAgent.userName%\",\"password\": \"%azureArtifacts.edFiBuildAgent.accessToken%\"}]}")
            param("project.directory", "Ed-Fi-ODS-Implementation/scripts/NuGet/%project.name%")
            param("project.name", "%system.teamcity.buildConfName%")
            param("project.shouldPublishPreRelease", "true")
        }

        steps {
        powerShell {
            name = "Build Pre-release and release, publish pre-release package"
            workingDir = "%project.directory%"
            scriptMode = script {
                content = """
                    ${'$'}parameters = @{
                        SemanticVersion = "%version%"
                        BuildCounter = "%build.counter%"
                        PreReleaseLabel = "%version.preReleaseLabel%"
                        Publish = [System.Convert]::ToBoolean("%project.shouldPublishPreRelease%")
                        NuGetFeed = "%azureArtifacts.feed.nuget%"
                        NuGetApiKey = "%azureArtifacts.edFiBuildAgent.accessToken%"
                    }
                    .\build-package.ps1 @parameters
                """.trimIndent()
            }
        }
    }
    }
}
