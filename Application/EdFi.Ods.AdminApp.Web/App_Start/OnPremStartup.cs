// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.Windsor;
using EdFi.Ods.AdminApp.Web._Installers;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup("OnPrem", typeof(EdFi.Ods.AdminApp.Web.OnPremStartup))]
namespace EdFi.Ods.AdminApp.Web
{
    public class OnPremStartup : StartupBase
    {
        protected override void InstallConfigurationSpecificInstaller(IWindsorContainer container)
        {
            if (AppSettings.AppStartup == "OnPrem")
                container.Install(new OnPremInstaller());
        }

        protected override void ConfigureLogging(IAppBuilder app)
        {
            if (AppSettings.AppStartup == "OnPrem")
                log4net.Config.XmlConfigurator.Configure();
        }
    }
}
