// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Security.MsSql;
internal class AdminConsoleSecurityMsSqlContextFactory : IDesignTimeDbContextFactory<AdminConsoleSecurityMsSqlContext>
{
    public AdminConsoleSecurityMsSqlContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
        var connectionString = configuration.GetConnectionString("EdFi_Security");
        var optionsBuilder = new DbContextOptionsBuilder<AdminConsoleSecurityMsSqlContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new AdminConsoleSecurityMsSqlContext(optionsBuilder.Options);
    }
}
