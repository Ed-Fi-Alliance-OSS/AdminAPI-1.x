// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        DbSetup.ConfigureDatabase(serviceCollection, configuration);
        DbSetup.ApplyMigrations(serviceCollection, configuration);

        #region Repositories
        RegisterRepositories(serviceCollection);
        #endregion

        #region Services
        RegisterServices(serviceCollection);
        #endregion

        #region Validators
        RegisterValidators(serviceCollection);
        #endregion

        serviceCollection.AddAutoMapper(typeof(AdminConsoleMappingProfile));
    }

    private static void RegisterRepositories(IServiceCollection serviceCollection)
    {
        #region Healthcheck
        serviceCollection.AddScoped<ICommandRepository<HealthCheck>, CommandRepository<HealthCheck>>();
        serviceCollection.AddScoped<IQueriesRepository<HealthCheck>, QueriesRepository<HealthCheck>>();
        #endregion
    }

    private static void RegisterServices(IServiceCollection serviceCollection)
    {
        #region Healthcheck
        serviceCollection.AddScoped<IAddHealthCheckCommand, AddHealthCheckCommand>();
        serviceCollection.AddScoped<IGetHealthCheckQuery, GetHealthCheckQuery>();
        serviceCollection.AddScoped<IGetHealthChecksQuery, GetHealthChecksQuery>();
        #endregion Healthcheck
    }

    private static void RegisterValidators(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AddHealthCheck.Validator>();
    }
}
