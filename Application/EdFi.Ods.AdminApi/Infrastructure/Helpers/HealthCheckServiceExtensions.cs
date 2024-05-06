// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;

namespace EdFi.Ods.AdminApi.Infrastructure;

public static class HealthCheckServiceExtensions
{
    public static IServiceCollection AddHealthCheck(
        this IServiceCollection services,
        IConfigurationRoot configuration
    )
    {
        Dictionary<string, string> connectionStrings;
        var databaseEngine = configuration.Get("AppSettings:DatabaseEngine", "SqlServer");
        var multiTenancyEnabled = configuration.Get("AppSettings:MultiTenancy", false);
        var connectionStringName = "EdFi_Admin";

        if (multiTenancyEnabled)
        {
            var tenantSettings =
                configuration.Get<TenantsSection>()
                ?? throw new AdminApiException("Unable to load tenant configuration from appSettings");

            connectionStrings = tenantSettings.Tenants.ToDictionary(
                x => x.Key,
                x => x.Value.ConnectionStrings[connectionStringName]
            );
        }
        else
        {
            connectionStrings = new()
            {
                { "SingleTenant", configuration.GetConnectionStringByName(connectionStringName) }
            };
        }

        if (!string.IsNullOrEmpty(databaseEngine))
        {
            var isSqlServer = DatabaseEngineEnum.Parse(databaseEngine).Equals(DatabaseEngineEnum.SqlServer);
            var hcBuilder = services.AddHealthChecks();

            foreach (var connectionString in connectionStrings)
            {
                if (isSqlServer)
                {
                    hcBuilder.AddSqlServer(connectionString.Value, name: connectionString.Key);
                }
                else
                {
                    hcBuilder.AddNpgSql(connectionString.Value, name: connectionString.Key);
                }
            }
        }

        return services;
    }
}
