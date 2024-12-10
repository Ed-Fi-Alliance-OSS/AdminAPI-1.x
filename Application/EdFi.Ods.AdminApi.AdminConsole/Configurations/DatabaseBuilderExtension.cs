// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsoleMsSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsolePgSql;
using EdFi.Ods.AdminApi.Common.Infrastructure.Context;
using EdFi.Ods.AdminApi.Common.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Common.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class DatabaseBuilderExtension
{
    public static void ConfigureAdminConsoleDatabase(this WebApplicationBuilder webApplicationBuilder)
    {
        IConfiguration config = webApplicationBuilder.Configuration;

        var databaseEngine = DbProviders.Parse(config.GetValue<string>("AppSettings:DatabaseEngine")!);

        switch (databaseEngine)
        {
            case DbProviders.SqlServer:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsoleMsSqlContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(AdminConnectionString(sp, config));
                });
                break;
            case DbProviders.PostgreSql:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsolePgSqlContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(AdminConnectionString(sp, config));
                });
                break;
            default:
                throw new ArgumentException($"Unexpected DB setup error. Engine '{databaseEngine}' was parsed as valid but is not configured for startup.");
        }
    }

    public static string AdminConnectionString(IServiceProvider serviceProvider, IConfiguration config)
    {
        var multiTenancyEnabled = config.Get("AppSettings:MultiTenancy", false);

        var adminConnectionString = string.Empty;

        if (multiTenancyEnabled)
        {
            var tenantContextProvider = serviceProvider.GetRequiredService<IContextProvider<TenantConfiguration>>();
            var tenantConfigurationProvider = serviceProvider.GetRequiredService<ITenantConfigurationProvider>();

            var tenant = tenantContextProvider.Get();
            if (tenant != null && !string.IsNullOrEmpty(tenant.AdminConnectionString))
            {
                adminConnectionString = tenant.AdminConnectionString;
            }
            else
            {
                var tenantSection = serviceProvider.GetRequiredService<IOptionsMonitor<TenantsSection>>();
                var tenants = tenantSection.CurrentValue.Tenants;

                if (tenants != null)
                {
                    var firstTenant = tenants.FirstOrDefault();
                    if (tenantConfigurationProvider.Get().TryGetValue(firstTenant.Key, out var tenantConfiguration))
                    {
                        tenantContextProvider.Set(tenantConfiguration);
                    }

                    adminConnectionString = tenantContextProvider.Get()!.AdminConnectionString;
                }
                else
                {
                    throw new ArgumentException($"Section Tenants not found");
                }
            }
        }
        else
        {
            adminConnectionString = config.GetConnectionStringByName("EdFi_Admin");
        }

        return adminConnectionString!;
    }

}
