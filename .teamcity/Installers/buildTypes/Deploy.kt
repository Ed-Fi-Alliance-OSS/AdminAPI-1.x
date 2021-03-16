// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object Deploy : BuildType ({
    name = "Deploy"
    description = "Creates a release in Octopus Deploy and triggers its deployment to the test server"
    type = BuildTypeSettings.Type.DEPLOYMENT

    params {
        param("octopus.package", "placeholder")
        param("octopus.environment", "Integration")
        param("nuGet.packageFile", "placeholder")
        param("octopus.channel", "Suite3")
        param("octopus.release", "placeholder")
        param("octopus.project", "Admin App")
    }

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Lookup Package Name and Version"
            formatStderrAsError = true
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            scriptMode = file {
                path = """eng\teamcity-get-package-version.ps1"""
            }
        }
        step {
            name = "Publish Package to Octopus"
            type = "octopus.push.package"
            param("octopus_host", "%octopus.server%")
            param("octopus_packagepaths", "%octopus.package%")
            param("octopus_forcepush", "true")
            param("secure:octopus_apikey", "%octopus.apiKey%")
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
        }
        step {
            name = "Create and Deploy Release"
            type = "octopus.create.release"
            param("octopus_additionalcommandlinearguments", """--variable="ForceFirstTimeSetup:false" --packageVersion=%octopus.release% --deploymenttimeout=%octopus.deployTimeout%""")
            param("octopus_channel_name", "%octopus.channel%")
            param("octopus_version", "3.0+")
            param("octopus_host", "%octopus.server%")
            param("octopus_project_name", "%octopus.project%")
            param("octopus_deployto", "%octopus.environment%")
            param("secure:octopus_apikey", "%octopus.apiKey%")
            param("octopus_releasenumber", "%octopus.release%")
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
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
