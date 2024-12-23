// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Security.MsSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Security.PgSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants;
using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Infrastructure.Context;
using EdFi.Ods.AdminApi.Common.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class AdminConsoleExtension
{
    public static void UseCorsForAdminConsole(this WebApplication app)
    {
        var adminConsoleSettings = app.Services.GetService<IOptions<AdminConsoleSettings>>();

        if (adminConsoleSettings != null && adminConsoleSettings.Value.CorsSettings.EnableCors)
            app.UseCors(AdminConsoleConstants.CorsPolicyName);
    }

    public static async Task InitAdminConsoleData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        IAdminConsoleTenantsService? adminConsoleTenantsService = scope.ServiceProvider.GetService<IAdminConsoleTenantsService>();
        if (adminConsoleTenantsService != null)
        {
            //Get tenants
            await adminConsoleTenantsService.InitializeTenantsAsync();
            //Populate intances
            var options = scope.ServiceProvider.GetService<IOptionsSnapshot<AppSettingsFile>>();
            var tenantId = 1;
            if (options!.Value.AppSettings.MultiTenancy)
            {
                foreach (var item in options!.Value.Tenants)
                {
                    using (var scopeInstances = app.Services.CreateScope())
                    {
                        var tenantConfigurationProvider = scopeInstances.ServiceProvider.GetService<ITenantConfigurationProvider>();
                        var tenantConfigurationContextProvider = scopeInstances.ServiceProvider.GetService<IContextProvider<TenantConfiguration>>();
                        if (tenantConfigurationProvider!.Get().TryGetValue(item.Key, out var tenantConfiguration))
                        {
                            //assign connection string to the dbcontext when is multitenant
                            tenantConfigurationContextProvider!.Set(tenantConfiguration);
                            IAdminConsoleInstancesService? adminConsoleInstancesService = scopeInstances.ServiceProvider.GetService<IAdminConsoleInstancesService>();

                            if (adminConsoleInstancesService != null)
                                await adminConsoleInstancesService.InitializeIntancesAsync(tenantId);
                            tenantId++;
                        }
                    }
                }
            }
            else
            {
                //single tenant
                //get instances in adminconsole
                IAdminConsoleInstancesService? adminConsoleInstancesService = scope.ServiceProvider.GetService<IAdminConsoleInstancesService>();
                if (adminConsoleInstancesService != null)
                    await adminConsoleInstancesService.InitializeIntancesAsync(tenantId);
            }
        }
    }

    public static void MigrateSecurityDbContext(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            DbContext dbContext;
            var databaseEngine = DbProviders.Parse(app.Configuration.GetValue<string>("AppSettings:DatabaseEngine")!);

            switch (databaseEngine)
            {
                case DbProviders.SqlServer:
                    dbContext = scope.ServiceProvider.GetRequiredService<AdminConsoleSecurityMsSqlContext>();
                    dbContext.Database.Migrate();
                    break;
                case DbProviders.PostgreSql:
                    dbContext = scope.ServiceProvider.GetRequiredService<AdminConsoleSecurityPgSqlContext>();
                    dbContext.Database.Migrate();
                    break;
            }
        }

    }
}
