// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell

object RepackageAdminAppInstallerAndPushToAzureBlobStorage : BuildType({
    name = "Repackage Admin App Installer and Push to Azure Blob Storage"

    params {
        param("project.directory", """Ed-Fi-ODS-AdminApp\%project.name%""")
    }

    steps {
        powerShell {
            name = "Repackage and Push"
            formatStderrAsError = true
            workingDir = "%project.directory%"
            scriptMode = script {
                content = """
                    ${'$'}pkg = Resolve-Path "%project.name%*"
                    ${'$'}zip = "temp.zip"
                    mv ${'$'}pkg ${'$'}zip
                    Expand-Archive ${'$'}zip
                    @("temp\package", "temp\_rels", "temp\*.nuspec", "temp\*.xml") | % {rm -r ${'$'}_}
                    ${'$'}version = ${'$'}pkg | Select-String '(\d\.\d.\d)' | % { ${'$'}_.Matches[0].Groups[1].Value }
                    ${'$'}name = "AdminAppInstaller.${'$'}(${'$'}version).zip"
                    Compress-Archive -Path ./temp/* -DestinationPath ${'$'}name
                    azcopy copy (Resolve-Path ${'$'}name) "https://odsassets.blob.core.windows.net/public/adminapp/${'$'}name"
                    rm -r temp
                """.trimIndent()
            }
        }
    }

    features {
        swabra {
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
