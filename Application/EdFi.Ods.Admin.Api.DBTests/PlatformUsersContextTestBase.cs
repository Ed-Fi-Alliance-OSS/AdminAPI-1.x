// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using NUnit.Framework;
using Respawn;
using static EdFi.Ods.Admin.Api.DBTests.Testing;

namespace EdFi.Ods.Admin.Api.DBTests;

[TestFixture]
public abstract class PlatformUsersContextTestBase
{
    private readonly Checkpoint _checkpoint = new()
    {
        TablesToIgnore = new[]
        {
            "__MigrationHistory", "DeployJournal", "AdminAppDeployJournal"
        },
        SchemasToExclude = new[]
        {
            "HangFire", "adminapp_HangFire"
        }
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
        Transaction(usersContext =>
        {
            foreach (var entity in entities)
                ((SqlServerUsersContext)usersContext).Set(entity.GetType()).Add(entity);
        });
    }

    protected static void Transaction(Action<IUsersContext> action)
    {
        using var usersContext = new SqlServerUsersContext(ConnectionString);
        using var transaction = (usersContext).Database.BeginTransaction();
        action(usersContext);
        usersContext.SaveChanges();
        transaction.Commit();
    }

    protected static TResult Transaction<TResult>(Func<IUsersContext, TResult> query)
    {
        var result = default(TResult);

        Transaction(database =>
        {
            result = query(database);
        });

        return result;
    }
}
