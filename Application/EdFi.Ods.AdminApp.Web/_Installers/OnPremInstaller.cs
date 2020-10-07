// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.OnPrem;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Display.DisplayService;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web._Installers
{
    public class OnPremInstaller : CommonConfigurationInstaller
    {
        protected override void InstallHostingSpecificClasses(IWindsorContainer container)
        {
            InstallEnterpriseSpecificServices(container);
        }

        private void InstallEnterpriseSpecificServices(IWindsorContainer container)
        {
            container.Register(Component.For<IGetCloudOdsInstanceQuery>()
                .ImplementedBy<GetOnPremOdsInstanceQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IGetCloudOdsApiWebsiteSettingsQuery>()
                .ImplementedBy<GetOnPremOdsApiWebsiteSettingsQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IUpdateCloudOdsApiWebsiteSettingsCommand>()
                .ImplementedBy<UpdateOnPremOdsApiWebsiteSettingsCommand>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsProductionLifecycleManagementService>()
                .ImplementedBy<OnPremProductionLifecycleManagementService>()
                .LifestyleTransient());

            container.Register(Component.For<IGetProductionApiProvisioningWarningsQuery>()
                .ImplementedBy<GetOnPremProductionApiProvisionWarningQuery>()
                .LifestyleTransient());

            container.Register(Component.For<ICompleteOdsPostUpdateSetupCommand>()
                .ImplementedBy<CompleteOnPremOdsPostUpdateSetupCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IRestartAppServicesCommand>()
                .ImplementedBy<RestartOnPremAppServicesCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IFirstTimeSetupService>()
                .ImplementedBy<OnPremFirstTimeSetupService>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseSqlServerSecurityConfiguration>()
                .ImplementedBy<OnPremOdsDatabaseSqlServerSecurityConfiguration>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseNameProvider>()
                .ImplementedBy<OnPremOdsDatabaseNameProvider>()
                .LifestyleTransient());

            container.Register(Component.For<IStringEncryptorService>()
                .ImplementedBy<StringEncryptorService>()
                .LifestyleTransient());

            container.Register(Component.For<ITabDisplayService>()
                .ImplementedBy<OnPremTabDisplayService>()
                .LifestyleTransient());

            container.Register(Component.For<IHomeScreenDisplayService>()
                .ImplementedBy<OnPremHomeScreenDisplayService>()
                .LifestyleTransient());

            container.Register(
                Component.For<ICompleteOdsFirstTimeSetupCommand>()
                    .ImplementedBy<CompleteOnPremFirstTimeSetupCommand>()
                    .LifestyleTransient());

        }
    }
}