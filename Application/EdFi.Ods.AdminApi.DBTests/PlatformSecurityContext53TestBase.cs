// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
extern alias Compatability;

using System;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using Compatability::EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using NUnit.Framework;
using Respawn;
using Microsoft.EntityFrameworkCore;
using Respawn.Graph;

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

    private Respawner _checkpoint;

    protected virtual string ConnectionString => TestContext.Database.GetConnectionString();

    protected virtual void AdditionalFixtureSetup()
    {
    }

    protected virtual async void CreateCheckpoint()
    {
        _checkpoint = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = new Table[]
        {
            "__MigrationHistory", "DeployJournal", "AdminApiDeployJournal"
        },
            SchemasToExclude = Array.Empty<string>()
        });
    }

    protected abstract SqlServerSecurityContext CreateDbContext();

    [OneTimeSetUp]
    public virtual async Task FixtureSetup()
    {
        CreateCheckpoint();
        TestContext = CreateDbContext();
        SetupContext = CreateDbContext();

        if (CheckpointPolicy == CheckpointPolicyOptions.BeforeAnyTest)
        {
            await _checkpoint.ResetAsync(ConnectionString);
        }

        AdditionalFixtureSetup();
    }

    [OneTimeTearDown]
    public async Task FixtureTearDown()
    {
        await _checkpoint.ResetAsync(ConnectionString);
        TestContext.Dispose();
        SetupContext.Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        CreateCheckpoint();
        TestContext = CreateDbContext();
        SetupContext = CreateDbContext();

        if (CheckpointPolicy == CheckpointPolicyOptions.BeforeEachTest)
        {
            await _checkpoint.ResetAsync(ConnectionString);
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
            TestContext.Add(entity);
        }

        TestContext.SaveChanges();
    }

    protected void UsersTransaction(Action<IUsersContext> action)
    {
        using var usersContext = new SqlServerUsersContext(Testing.GetDbContextOptions(Testing.AdminConnectionString));
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
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer(Testing.SecurityV53ConnectionString);

        using var usersContext = new SqlServerSecurityContext(optionsBuilder.Options);
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
