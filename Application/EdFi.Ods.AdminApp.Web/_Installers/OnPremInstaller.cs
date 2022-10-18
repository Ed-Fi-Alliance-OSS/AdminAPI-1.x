// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.OnPrem;
using EdFi.Ods.AdminApp.Web.Display.DisplayService;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web._Installers
{
    public class OnPremInstaller : CommonConfigurationInstaller
    {
        protected override void InstallHostingSpecificClasses(IServiceCollection services)
        {
            services.AddTransient<IGetCloudOdsInstanceQuery, GetOnPremOdsInstanceQuery>();
            services.AddTransient<IGetCloudOdsApiWebsiteSettingsQuery, GetOnPremOdsApiWebsiteSettingsQuery>();
            services.AddTransient<IUpdateCloudOdsApiWebsiteSettingsCommand, UpdateOnPremOdsApiWebsiteSettingsCommand>();
            services.AddTransient<ICompleteOdsPostUpdateSetupCommand, CompleteOnPremOdsPostUpdateSetupCommand>();
            services.AddTransient<IRestartAppServicesCommand, RestartOnPremAppServicesCommand>();
            services.AddTransient<IFirstTimeSetupService, OnPremFirstTimeSetupService>();
            services.AddTransient<ICloudOdsDatabaseSqlServerSecurityConfiguration,OnPremOdsDatabaseSqlServerSecurityConfiguration>();
            services.AddTransient<ITabDisplayService, OnPremTabDisplayService>();
            services.AddTransient<ICompleteOdsFirstTimeSetupCommand, CompleteOnPremFirstTimeSetupCommand>();
        }
    }
}
