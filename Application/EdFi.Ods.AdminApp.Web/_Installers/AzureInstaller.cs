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
        protected override void InstallHostingSpecificClasses(IWindsorContainer container)
        {
            InstallAzureSpecificServices(container);
        }

        private void InstallAzureSpecificServices(IWindsorContainer container)
        {
            container.Register(Component.For<AzureActiveDirectoryClientInfo>()
                .Instance(CloudOdsAzureActiveDirectoryClientInfo.GetActiveDirectoryClientInfoForUser())
                .LifestyleSingleton());

            container.Register(Component.For<IGetCloudOdsHostedComponentsQuery, IGetAzureCloudOdsHostedComponentsQuery, GetAzureCloudOdsHostedComponentsQuery>()
                .ImplementedBy<GetAzureCloudOdsHostedComponentsQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IGetAzureCloudOdsWebsitePerformanceLevelQuery, GetAzureCloudOdsWebsitePerformanceLevelQuery>()
                .ImplementedBy<GetAzureCloudOdsWebsitePerformanceLevelQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IGetCloudOdsApiWebsiteSettingsQuery, GetAzureCloudOdsApiWebsiteSettingsQuery>()
                .ImplementedBy<GetAzureCloudOdsApiWebsiteSettingsQuery>()
                .LifestyleTransient());

            container.Register(Component.For<AzureDatabaseManagementService>()
                .ImplementedBy<AzureDatabaseManagementService>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseSecurityConfigurator, SqlServerCloudOdsDatabaseSecurityConfigurator>()
                .ImplementedBy<SqlServerCloudOdsDatabaseSecurityConfigurator>()
                .LifestyleTransient());

            container.Register(Component.For<AzureDatabaseLifecycleManagementService>()
                .ImplementedBy<AzureDatabaseLifecycleManagementService>()
                .LifestyleTransient());

            container.Register(Component.For<GetAzureCloudOdsHostedInstanceQuery>()
                .ImplementedBy<GetAzureCloudOdsHostedInstanceQuery>()
                .LifestyleTransient());

            container.Register(Component.For<ICompleteOdsPostUpdateSetupCommand>()
                .ImplementedBy<CompleteAzureOdsPostUpdateSetupCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IRestartAppServicesCommand>()
                .ImplementedBy<RestartAzureAppServicesCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IUpdateCloudOdsApiWebsiteSettingsCommand>()
                .ImplementedBy<UpdateAzureCloudOdsApiWebsiteSettingsCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IGetLatestPublishedOdsVersionQuery>()
                .ImplementedBy<GetLatestPublishedOdsVersionQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IGetProductionApiProvisioningWarningsQuery>()
                .ImplementedBy<GetAzureProductionApiProvisioningWarningsQuery>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsProductionLifecycleManagementService>()
                .ImplementedBy<AzureProductionLifecycleManagementService>()
                .LifestyleTransient());

            container.Register(Component.For<IGetCloudOdsInstanceQuery>()
                .ImplementedBy<GetAzureCloudOdsInstanceQuery>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseSqlServerSecurityConfiguration>()
                .ImplementedBy<AzureCloudOdsDatabaseSqlServerSecurityConfiguration>()
                .LifestyleTransient());

            container.Register(Component.For<IFirstTimeSetupService>()
                .ImplementedBy<AzureFirstTimeSetupService>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseNameProvider>()
                .ImplementedBy<AzureCloudOdsDatabaseNameProvider>()
                .LifestyleTransient());

            container.Register(Component.For<ITabDisplayService>()
                .ImplementedBy<AzureTabDisplayService>()
                .LifestyleTransient());

            container.Register(Component.For<IHomeScreenDisplayService>()
                .ImplementedBy<AzureHomeScreenDisplayService>()
                .LifestyleTransient());

            container.Register(
                Component.For<ICompleteOdsFirstTimeSetupCommand>()
                    .ImplementedBy<CompleteOdsFirstTimeSetupCommand>()
                    .LifestyleTransient());
        }
    }
}
