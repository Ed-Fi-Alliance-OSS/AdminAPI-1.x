// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsoleSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Respawn;
using static EdFi.Ods.AdminConsole.DBTests.Testing;

namespace EdFi.Ods.AdminConsole.DBTests;

[TestFixture]
public abstract class PlatformUsersContextTestBase
{
    private readonly Checkpoint _checkpoint = new()
    {
        TablesToIgnore = new[]
        {
            "__MigrationHistory", "DeployJournal", "AdminConsoleDeployJournal"
        },
        SchemasToExclude = Array.Empty<string>()
    };

    protected static string ConnectionString => AdminConnectionString;

    [OneTimeTearDown]
    public async Task FixtureTearDown()
    {
        await _checkpoint.Reset(ConnectionString);
    }

    [SetUp]
    public async Task SetUp()
    {
        await _checkpoint.Reset(ConnectionString);
    }

    protected static void Save(params object[] entities)
    {
        Transaction(dbContext =>
        {
            foreach (var entity in entities)
            {
                ((AdminConsoleSqlContext)dbContext).Add(entity);
            }
        });
    }

    protected static async void Transaction(Action<IDbContext> action)
    {
        using var dbContext = new AdminConsoleSqlContext(GetDbContextOptions());
        using var transaction = (dbContext).Database.BeginTransaction();
        action(dbContext);
        dbContext.SaveChanges();
        transaction.Commit();
    }

    protected static TResult Transaction<TResult>(Func<IDbContext, TResult> query)
    {
        var result = default(TResult);

        Transaction(database =>
        {
            result = query(database);
        });

        return result;
    }

    protected static DbContextOptions<AdminConsoleSqlContext> GetDbContextOptions()
    {
        var builder = new DbContextOptionsBuilder<AdminConsoleSqlContext>();
        builder.UseSqlServer(ConnectionString);
        return builder.Options;
    }
}
