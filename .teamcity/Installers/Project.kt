// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package installers

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object AdminAppInstallerProject : Project({
    name = "Admin App Installer"
    id = RelativeId("AdminAppInstaller")
    description = "ODS Admin App Installer Build Configurations"

    params {
        param("project.name", "EdFi.Suite3.Installer.AdminApp")
        param("adminAppInstaller.version", "2.2.0")
        param("adminAppWeb.version", "2.2.0-pre0008")
    }

    buildType(_self.buildTypes.BuildAdminAppInstaller)
    buildType(_self.buildTypes.Deploy)
})
