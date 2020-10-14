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
        protected override void InstallHostingSpecificClasses(IWindsorContainer services)
        {
            InstallEnterpriseSpecificServices(services);
        }

        private void InstallEnterpriseSpecificServices(IWindsorContainer services)
        {
            services.Register(Component.For<IGetCloudOdsInstanceQuery>()
                .ImplementedBy<GetOnPremOdsInstanceQuery>()
                .LifestyleTransient());

            services.Register(Component.For<IGetCloudOdsApiWebsiteSettingsQuery>()
                .ImplementedBy<GetOnPremOdsApiWebsiteSettingsQuery>()
                .LifestyleTransient());

            services.Register(Component.For<IUpdateCloudOdsApiWebsiteSettingsCommand>()
                .ImplementedBy<UpdateOnPremOdsApiWebsiteSettingsCommand>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsProductionLifecycleManagementService>()
                .ImplementedBy<OnPremProductionLifecycleManagementService>()
                .LifestyleTransient());

            services.Register(Component.For<IGetProductionApiProvisioningWarningsQuery>()
                .ImplementedBy<GetOnPremProductionApiProvisionWarningQuery>()
                .LifestyleTransient());

            services.Register(Component.For<ICompleteOdsPostUpdateSetupCommand>()
                .ImplementedBy<CompleteOnPremOdsPostUpdateSetupCommand>()
                .LifestyleTransient());

            services.Register(Component.For<IRestartAppServicesCommand>()
                .ImplementedBy<RestartOnPremAppServicesCommand>()
                .LifestyleTransient());

            services.Register(Component.For<IFirstTimeSetupService>()
                .ImplementedBy<OnPremFirstTimeSetupService>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsDatabaseSqlServerSecurityConfiguration>()
                .ImplementedBy<OnPremOdsDatabaseSqlServerSecurityConfiguration>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsDatabaseNameProvider>()
                .ImplementedBy<OnPremOdsDatabaseNameProvider>()
                .LifestyleTransient());

            services.Register(Component.For<ITabDisplayService>()
                .ImplementedBy<OnPremTabDisplayService>()
                .LifestyleTransient());

            services.Register(Component.For<IHomeScreenDisplayService>()
                .ImplementedBy<OnPremHomeScreenDisplayService>()
                .LifestyleTransient());

            services.Register(
                Component.For<ICompleteOdsFirstTimeSetupCommand>()
                    .ImplementedBy<CompleteOnPremFirstTimeSetupCommand>()
                    .LifestyleTransient());

        }
    }
}
