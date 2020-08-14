// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.Windsor;
using EdFi.Ods.AdminApp.Web._Installers;
using Microsoft.Owin;

[assembly: OwinStartup("Aws", typeof(EdFi.Ods.AdminApp.Web.AwsStartup))]
namespace EdFi.Ods.AdminApp.Web
{
    public class AwsStartup : StartupBase
    {
        protected override void InstallConfigurationSpecificInstaller(IWindsorContainer container)
        {
            container.Install(new AwsInstaller());
        }
    }
}