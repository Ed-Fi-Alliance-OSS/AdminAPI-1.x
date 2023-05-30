// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using NUnit.Framework;
using Respawn;

namespace EdFi.Ods.AdminApi.DBTests;

[TestFixture]
public abstract class PlatformSecurityContextTestBase53
{
    protected SqlServerSecurityContext TestContext { get; private set; }
    protected SqlServerSecurityContext SetupContext { get; private set; }

    protected enum CheckpointPolicyOptions
    {
        BeforeEachTest,
        BeforeAnyTest
    }

    protected CheckpointPolicyOptions CheckpointPolicy { get; set; } = CheckpointPolicyOptions.BeforeEachTest;

    private readonly Checkpoint _checkpoint = new()
    {
        TablesToIgnore = new[]
        {
            "__MigrationHistory", "DeployJournal", "AdminApiDeployJournal"
        },
        SchemasToExclude = new[]
        {
            "HangFire", "adminapi_HangFire"
        }
    };

    protected virtual string ConnectionString => TestContext.Database.Connection.ConnectionString;

    protected virtual void AdditionalFixtureSetup()
    {
    }

    protected abstract SqlServerSecurityContext CreateDbContext();

    [OneTimeSetUp]
    public virtual async Task FixtureSetup()
    {
        TestContext = CreateDbContext();
        SetupContext = CreateDbContext();

        if (CheckpointPolicy == CheckpointPolicyOptions.BeforeAnyTest)
        {
            await _checkpoint.Reset(ConnectionString);
        }

        AdditionalFixtureSetup();
    }

    [OneTimeTearDown]
    public async Task FixtureTearDown()
    {
        await _checkpoint.Reset(ConnectionString);
    }

    [SetUp]
    public async Task SetUp()
    {
        TestContext = CreateDbContext();
        SetupContext = CreateDbContext();

        if (CheckpointPolicy == CheckpointPolicyOptions.BeforeEachTest)
        {
            await _checkpoint.Reset(ConnectionString);
        }
    }

    [TearDown]
    public void TearDown()
    {
        TestContext.Dispose();
        SetupContext.Dispose();
    }

    protected void Save(params object[] entities)
    {
        foreach (var entity in entities)
        {
            TestContext.Set(entity.GetType()).Add(entity);
        }

        TestContext.SaveChanges();
    }

    protected void UsersTransaction(Action<IUsersContext> action)
    {
        using var usersContext = new SqlServerUsersContext(Testing.AdminConnectionString);
        using var transaction = usersContext.Database.BeginTransaction();
        action(usersContext);
        TestContext.SaveChanges();
        transaction.Commit();
    }

    protected TResult UsersTransaction<TResult>(Func<IUsersContext, TResult> query)
    {
        var result = default(TResult);

        UsersTransaction(database =>
        {
            result = query(database);
        });

        return result;
    }

    protected void Transaction(Action<ISecurityContext> action)
    {
        using var usersContext = new SqlServerSecurityContext(Testing.SecurityV53ConnectionString);
        using var usersTransaction = usersContext.Database.BeginTransaction();
        action(usersContext);
        TestContext.SaveChanges();
        usersTransaction.Commit();
    }

    protected TResult Transaction<TResult>(Func<ISecurityContext, TResult> query)
    {
        var result = default(TResult);

        Transaction((usersContext) =>
        {
            result = query(usersContext);
        });

        return result;
    }
}
