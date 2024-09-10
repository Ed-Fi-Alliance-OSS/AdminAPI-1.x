// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
extern alias Compatability;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.DBTests;

public static class Testing
{
    public static void EnsureInitialized()
    {
        _ = new SecurityTestDatabaseSetup();
        SecurityTestDatabaseSetup.EnsureSecurityDatabase(@"C:\\temp");
    }

    private static IConfigurationRoot _config;

    public static IConfiguration Configuration()
    {
        _config ??= new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        return _config;
    }

    public static string AdminConnectionString { get { return Configuration().GetConnectionString("Admin"); } }

    public static string SecurityConnectionString { get { return Configuration().GetConnectionString("Security"); } }

    public static string SecurityV53ConnectionString { get { return Configuration().GetConnectionString("SecurityV53"); } }

    public static int DefaultPageSizeOffset => (int)Configuration().GetValue(typeof(int), "DefaultPageSizeOffset");

    public static int DefaultPageSizeLimit => (int)Configuration().GetValue(typeof(int), "DefaultPageSizeLimit");

    public static DbContextOptions GetDbContextOptions(string connectionString)
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseSqlServer(connectionString);
        return builder.Options;
    }
}
