// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck;
using FeaturesTenant = EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
using EdFi.Ods.AdminApi.AdminConsole.Features.Instances;
using EdFi.Ods.AdminApi.AdminConsole.Features.Permissions;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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


        serviceCollection.Configure<AdminConsoleSettings>(configuration.GetSection("AdminConsoleSettings"));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        serviceCollection.AddTransient<IEncryptionKeySettings>(sp => sp.GetService<IOptions<AdminConsoleSettings>>().Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        serviceCollection.AddTransient<IEncryptionKeyResolver, OptionsEncryptionKeyResolver>();
        serviceCollection.AddScoped<IEncryptionService, EncryptionService>();
    }

    private static void RegisterRepositories(IServiceCollection serviceCollection)
    {
        #region Healthcheck
        serviceCollection.AddScoped<ICommandRepository<HealthCheck>, CommandRepository<HealthCheck>>();
        serviceCollection.AddScoped<IQueriesRepository<HealthCheck>, QueriesRepository<HealthCheck>>();
        #endregion


        #region Tenant
        serviceCollection.AddScoped<ICommandRepository<Tenant>, CommandRepository<Tenant>>();
        serviceCollection.AddScoped<IQueriesRepository<Tenant>, QueriesRepository<Tenant>>();
        #endregion

        #region Instance
        serviceCollection.AddScoped<ICommandRepository<Instance>, CommandRepository<Instance>>();
        serviceCollection.AddScoped<IQueriesRepository<Instance>, QueriesRepository<Instance>>();
        #endregion

        #region Permission
        serviceCollection.AddScoped<ICommandRepository<Permission>, CommandRepository<Permission>>();
        serviceCollection.AddScoped<IQueriesRepository<Permission>, QueriesRepository<Permission>>();
        #endregion
    }

    private static void RegisterServices(IServiceCollection serviceCollection)
    {
        #region Healthcheck
        serviceCollection.AddScoped<IAddHealthCheckCommand, AddHealthCheckCommand>();
        serviceCollection.AddScoped<IGetHealthCheckQuery, GetHealthCheckQuery>();
        serviceCollection.AddScoped<IGetHealthChecksQuery, GetHealthChecksQuery>();
        #endregion

        #region Tenant
        serviceCollection.AddScoped<IAddTenantCommand, AddTenantCommand>();
        serviceCollection.AddScoped<IGetTenantQuery, GetTenantQuery>();
        #endregion

        #region Instance
        serviceCollection.AddScoped<IAddInstanceCommand, AddInstanceCommand>();
        serviceCollection.AddScoped<IGetInstanceQuery, GetInstanceQuery>();
        serviceCollection.AddScoped<IGetInstancesQuery, GetInstancesQuery>();
        #endregion Instance

        #region Permission
        serviceCollection.AddScoped<IAddPermissionCommand, AddPermissionCommand>();
        serviceCollection.AddScoped<IGetPermissionQuery, GetPermissionQuery>();
        serviceCollection.AddScoped<IGetPermissionsQuery, GetPermissionsQuery>();
        #endregion Permission
    }

    private static void RegisterValidators(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AddHealthCheck.Validator>();
        serviceCollection.AddTransient<FeaturesTenant.AddTenant.Validator>();
        serviceCollection.AddTransient<AddInstance.Validator>();
        serviceCollection.AddTransient<AddPermission.Validator>();
    }
}
