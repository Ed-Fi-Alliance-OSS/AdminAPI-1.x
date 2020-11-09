// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using NUnit.Framework;
using Respawn;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public abstract class PlatformUsersContextTestBase
    {
        protected SqlServerUsersContext SetupContext { get; private set; }

        protected enum CheckpointPolicyOptions
        {
            DoNotCheckpoint,
            BeforeEachTest,
            BeforeAnyTest
        }

        protected CheckpointPolicyOptions CheckpointPolicy { get; set; } = CheckpointPolicyOptions.BeforeEachTest;

        private readonly Checkpoint _checkpoint = new Checkpoint
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

        protected abstract string ConnectionString { get; }

        protected virtual void AdditionalFixtureSetup()
        {
        }

        protected abstract SqlServerUsersContext CreateDbContext();

        [OneTimeSetUp]
        public virtual async Task FixtureSetup()
        {
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
            SetupContext = CreateDbContext();

            if (CheckpointPolicy == CheckpointPolicyOptions.BeforeEachTest)
            {
                await _checkpoint.Reset(ConnectionString);
            }
        }

        [TearDown]
        public void TearDown()
        {
            SetupContext.Dispose();
        }
        
        protected void Save(params object[] entities)
        {
            Transaction(usersContext =>
            {
                foreach (var entity in entities)
                    ((SqlServerUsersContext) usersContext).Set(entity.GetType()).Add(entity);
            });
        }

        protected void Transaction(Action<IUsersContext> action)
        {
            Scoped<IUsersContext>(usersContext =>
            {
                using (var transaction = ((SqlServerUsersContext)usersContext).Database.BeginTransaction())
                {
                    action(usersContext);
                    usersContext.SaveChanges();
                    transaction.Commit();
                }
            });
        }

        protected TResult Transaction<TResult>(Func<IUsersContext, TResult> query)
        {
            var result = default(TResult);

            Transaction(database =>
            {
                result = query(database);
            });

            return result;
        }
    }
}
