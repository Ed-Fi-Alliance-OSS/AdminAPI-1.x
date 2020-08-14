// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Aws;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Display.DisplayService;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web._Installers
{
    public class AwsInstaller : CommonConfigurationInstaller
    {
        protected override void InstallHostingSpecificClasses(IWindsorContainer container)
        {
            InstallEnterpriseSpecificServices(container);
        }

        private void InstallEnterpriseSpecificServices(IWindsorContainer container)
        {
            container.Register(Component.For<IGetCloudOdsInstanceQuery>()
                .ImplementedBy<GetAwsCloudOdsInstanceQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IGetCloudOdsApiWebsiteSettingsQuery>()
                .ImplementedBy<GetAwsCloudOdsApiWebsiteSettingsQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IUpdateCloudOdsApiWebsiteSettingsCommand>()
                .ImplementedBy<UpdateAwsCloudOdsApiWebsiteSettingsCommand>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsProductionLifecycleManagementService>()
                .ImplementedBy<AwsProductionLifecycleManagementService>()
                .LifestyleTransient());

            container.Register(Component.For<IGetProductionApiProvisioningWarningsQuery>()
                .ImplementedBy<GetAwsProductionApiProvisionWarningQuery>()
                .LifestyleTransient());

            container.Register(Component.For<ICompleteOdsPostUpdateSetupCommand>()
                .ImplementedBy<CompleteAwsOdsPostUpdateSetupCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IRestartAppServicesCommand>()
                .ImplementedBy<RestartAwsAppServicesCommand>()
                .LifestyleTransient());

            container.Register(Component.For<IGetCloudOdsHostedComponentsQuery>()
                .ImplementedBy<LocalFileBasedGetCloudOdsHostedComponentsQuery>()
                .LifestyleTransient());

            container.Register(Component.For<IFirstTimeSetupService>()
                .ImplementedBy<AwsFirstTimeSetupService>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseSqlServerSecurityConfiguration>()
                .ImplementedBy<AwsCloudOdsDatabaseSqlServerSecurityConfiguration>()
                .LifestyleTransient());

            container.Register(Component.For<ICloudOdsDatabaseNameProvider>()
                .ImplementedBy<AwsCloudOdsDatabaseNameProvider>()
                .LifestyleTransient());

            container.Register(Component.For<IStringEncryptorService>()
                .ImplementedBy<StringEncryptorService>()
                .LifestyleTransient());

            container.Register(Component.For<ITabDisplayService>()
                .ImplementedBy<AwsTabDisplayService>()
                .LifestyleTransient());

            container.Register(Component.For<IHomeScreenDisplayService>()
                .ImplementedBy<AwsHomeScreenDisplayService>()
                .LifestyleTransient());

            container.Register(
                Component.For<ICompleteOdsFirstTimeSetupCommand>()
                    .ImplementedBy<CompleteOdsFirstTimeSetupCommand>()
                    .LifestyleTransient());
        }
    }
}