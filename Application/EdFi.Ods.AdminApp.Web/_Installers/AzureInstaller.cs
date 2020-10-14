// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Ods.AdminApp.Management.Azure;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Setup;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Display.DisplayService;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web._Installers
{
    public class AzureInstaller : CommonConfigurationInstaller
    {
        protected override void InstallHostingSpecificClasses(IWindsorContainer services)
        {
            InstallAzureSpecificServices(services);
        }

        private void InstallAzureSpecificServices(IWindsorContainer services)
        {
            services.Register(Component.For<AzureActiveDirectoryClientInfo>()
                .Instance(CloudOdsAzureActiveDirectoryClientInfo.GetActiveDirectoryClientInfoForUser())
                .LifestyleSingleton());

            services.Register(Component.For<IGetCloudOdsHostedComponentsQuery, IGetAzureCloudOdsHostedComponentsQuery, GetAzureCloudOdsHostedComponentsQuery>()
                .ImplementedBy<GetAzureCloudOdsHostedComponentsQuery>()
                .LifestyleTransient());

            services.Register(Component.For<IGetAzureCloudOdsWebsitePerformanceLevelQuery, GetAzureCloudOdsWebsitePerformanceLevelQuery>()
                .ImplementedBy<GetAzureCloudOdsWebsitePerformanceLevelQuery>()
                .LifestyleTransient());

            services.Register(Component.For<IGetCloudOdsApiWebsiteSettingsQuery, GetAzureCloudOdsApiWebsiteSettingsQuery>()
                .ImplementedBy<GetAzureCloudOdsApiWebsiteSettingsQuery>()
                .LifestyleTransient());

            services.Register(Component.For<AzureDatabaseManagementService>()
                .ImplementedBy<AzureDatabaseManagementService>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsDatabaseSecurityConfigurator, SqlServerCloudOdsDatabaseSecurityConfigurator>()
                .ImplementedBy<SqlServerCloudOdsDatabaseSecurityConfigurator>()
                .LifestyleTransient());

            services.Register(Component.For<AzureDatabaseLifecycleManagementService>()
                .ImplementedBy<AzureDatabaseLifecycleManagementService>()
                .LifestyleTransient());

            services.Register(Component.For<GetAzureCloudOdsHostedInstanceQuery>()
                .ImplementedBy<GetAzureCloudOdsHostedInstanceQuery>()
                .LifestyleTransient());

            services.Register(Component.For<ICompleteOdsPostUpdateSetupCommand>()
                .ImplementedBy<CompleteAzureOdsPostUpdateSetupCommand>()
                .LifestyleTransient());

            services.Register(Component.For<IRestartAppServicesCommand>()
                .ImplementedBy<RestartAzureAppServicesCommand>()
                .LifestyleTransient());

            services.Register(Component.For<IUpdateCloudOdsApiWebsiteSettingsCommand>()
                .ImplementedBy<UpdateAzureCloudOdsApiWebsiteSettingsCommand>()
                .LifestyleTransient());

            services.Register(Component.For<IGetLatestPublishedOdsVersionQuery>()
                .ImplementedBy<GetLatestPublishedOdsVersionQuery>()
                .LifestyleTransient());

            services.Register(Component.For<IGetProductionApiProvisioningWarningsQuery>()
                .ImplementedBy<GetAzureProductionApiProvisioningWarningsQuery>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsProductionLifecycleManagementService>()
                .ImplementedBy<AzureProductionLifecycleManagementService>()
                .LifestyleTransient());

            services.Register(Component.For<IGetCloudOdsInstanceQuery>()
                .ImplementedBy<GetAzureCloudOdsInstanceQuery>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsDatabaseSqlServerSecurityConfiguration>()
                .ImplementedBy<AzureCloudOdsDatabaseSqlServerSecurityConfiguration>()
                .LifestyleTransient());

            services.Register(Component.For<IFirstTimeSetupService>()
                .ImplementedBy<AzureFirstTimeSetupService>()
                .LifestyleTransient());

            services.Register(Component.For<ICloudOdsDatabaseNameProvider>()
                .ImplementedBy<AzureCloudOdsDatabaseNameProvider>()
                .LifestyleTransient());

            services.Register(Component.For<ITabDisplayService>()
                .ImplementedBy<AzureTabDisplayService>()
                .LifestyleTransient());

            services.Register(Component.For<IHomeScreenDisplayService>()
                .ImplementedBy<AzureHomeScreenDisplayService>()
                .LifestyleTransient());

            services.Register(
                Component.For<ICompleteOdsFirstTimeSetupCommand>()
                    .ImplementedBy<CompleteOdsFirstTimeSetupCommand>()
                    .LifestyleTransient());
        }
    }
}
