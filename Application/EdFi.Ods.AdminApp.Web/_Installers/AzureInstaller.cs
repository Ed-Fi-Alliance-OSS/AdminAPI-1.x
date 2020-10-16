// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Ods.AdminApp.Management.Azure;
using EdFi.Ods.AdminApp.Management;
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
            services.AddSingleton(CloudOdsAzureActiveDirectoryClientInfo.GetActiveDirectoryClientInfoForUser());

            services.Register(Component.For<IGetCloudOdsHostedComponentsQuery, IGetAzureCloudOdsHostedComponentsQuery, GetAzureCloudOdsHostedComponentsQuery>()
                .ImplementedBy<GetAzureCloudOdsHostedComponentsQuery>()
                .LifestyleTransient());

            services.Register(Component.For<IGetAzureCloudOdsWebsitePerformanceLevelQuery, GetAzureCloudOdsWebsitePerformanceLevelQuery>()
                .ImplementedBy<GetAzureCloudOdsWebsitePerformanceLevelQuery>()
                .LifestyleTransient());

            services.AddTransient<IGetCloudOdsApiWebsiteSettingsQuery, GetAzureCloudOdsApiWebsiteSettingsQuery>();
            services.AddTransient<AzureDatabaseManagementService>();
            services.AddTransient<IAzureSqlSecurityConfigurator, AzureSqlSecurityConfigurator>();
            services.AddTransient<AzureDatabaseLifecycleManagementService>();
            services.AddTransient<GetAzureCloudOdsHostedInstanceQuery>();
            services.AddTransient<ICompleteOdsPostUpdateSetupCommand, CompleteAzureOdsPostUpdateSetupCommand>();
            services.AddTransient<IRestartAppServicesCommand, RestartAzureAppServicesCommand>();
            services.AddTransient<IUpdateCloudOdsApiWebsiteSettingsCommand, UpdateAzureCloudOdsApiWebsiteSettingsCommand>();
            services.AddTransient<IGetProductionApiProvisioningWarningsQuery, GetAzureProductionApiProvisioningWarningsQuery>();
            services.AddTransient<ICloudOdsProductionLifecycleManagementService, AzureProductionLifecycleManagementService>();
            services.AddTransient<IGetCloudOdsInstanceQuery, GetAzureCloudOdsInstanceQuery>();
            services.AddTransient<ICloudOdsDatabaseSqlServerSecurityConfiguration, AzureCloudOdsDatabaseSqlServerSecurityConfiguration>();
            services.AddTransient<IFirstTimeSetupService, AzureFirstTimeSetupService>();
            services.AddTransient<ICloudOdsDatabaseNameProvider, AzureCloudOdsDatabaseNameProvider>();
            services.AddTransient<ITabDisplayService, AzureTabDisplayService>();
            services.AddTransient<IHomeScreenDisplayService, AzureHomeScreenDisplayService>();
            services.AddTransient<ICompleteOdsFirstTimeSetupCommand, CompleteAzureFirstTimeSetupCommand>();
        }
    }
}
