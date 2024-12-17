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
        var multiTenancyEnabled = config.Get("AppSettings:MultiTenancy", false);

        switch (databaseEngine)
        {
            case DbProviders.SqlServer:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsoleMsSqlContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(AdminConnectionString(sp));
                });
                break;
            case DbProviders.PostgreSql:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsolePgSqlContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(AdminConnectionString(sp));
                });
                break;
            default:
                throw new ArgumentException($"Unexpected DB setup error. Engine '{databaseEngine}' was parsed as valid but is not configured for startup.");
        }

        string AdminConnectionString(IServiceProvider serviceProvider)
        {
            var adminConnectionString = string.Empty;
            if (multiTenancyEnabled)
            {
                var tenant = serviceProvider.GetRequiredService<IContextProvider<TenantConfiguration>>().Get();
                if (tenant != null && !string.IsNullOrEmpty(tenant.AdminConnectionString))
                {
                    adminConnectionString = tenant.AdminConnectionString;
                }
                else
                {
                    throw new ArgumentException($"Admin database connection setup error. Tenant not configured correctly.");
                }
            }
            else
            {
                adminConnectionString = config.GetConnectionStringByName("EdFi_Admin");
            }

            return adminConnectionString;
        }
    }
}
