// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsolePgSql;

public class AdminConsolePgSqlContextFactory : IDesignTimeDbContextFactory<AdminConsolePgSqlContext>
{
    public AdminConsolePgSqlContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

        var connectionString = configuration.GetConnectionString("EdFi_Admin");
        var optionsBuilder = new DbContextOptionsBuilder<AdminConsolePgSqlContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AdminConsolePgSqlContext(optionsBuilder.Options);
    }
}
