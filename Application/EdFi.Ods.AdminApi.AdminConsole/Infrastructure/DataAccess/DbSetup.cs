// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsolePg;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsoleSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;

public static class DbSetup
{
    public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var databaseProvider = DbProviders.Parse(configuration.GetValue<string>("AppSettings:DatabaseEngine")!);
        var connectionString = configuration.GetConnectionString("EdFi_Admin");
        switch (databaseProvider)
        {
            case DbProviders.SqlServer:
                services.AddDbContext<IDbContext, AdminConsoleSqlContext>(options =>
                           options.UseSqlServer(connectionString));
                break;
            case DbProviders.PostgreSql:
                services.AddDbContext<IDbContext, AdminConsolePgContext>(options =>
                           options.UseNpgsql(connectionString));
                break;
            default:
                throw new InvalidOperationException($"Invalid database provider specified. {databaseProvider}.");
        }
    }

    public static void ApplyMigrations(IServiceCollection services, IConfiguration configuration)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var databaseProvider = DbProviders.Parse(configuration.GetValue<string>("AppSettings:DatabaseEngine")!);
        DbContext dbContext = databaseProvider switch
        {
            DbProviders.SqlServer => scope.ServiceProvider.GetRequiredService<AdminConsoleSqlContext>(),
            DbProviders.PostgreSql => scope.ServiceProvider.GetRequiredService<AdminConsolePgContext>(),
            _ => throw new InvalidOperationException("Invalid database provider.")
        };
        dbContext.Database.Migrate();
    }
}
