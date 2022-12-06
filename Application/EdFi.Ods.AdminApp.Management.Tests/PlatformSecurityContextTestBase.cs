// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

extern alias SecurityDataAccessLatest;

using System;
using System.Threading.Tasks;
using SecurityDataAccessLatest::EdFi.Security.DataAccess.Contexts;
using NUnit.Framework;
using Respawn;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public abstract class PlatformSecurityContextTestBase
    {
        protected SqlServerSecurityContext TestContext { get; private set; }
        protected SqlServerSecurityContext SetupContext { get; private set; }

        protected enum CheckpointPolicyOptions
        {
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
                SetupContext.Set(entity.GetType()).Add(entity);
            }

            SetupContext.SaveChanges();
        }

        protected void Transaction(Action<ISecurityContext> action)
        {
            Scoped<ISecurityContext>(securityContext =>
            {
                using (var transaction = ((SqlServerSecurityContext)securityContext).Database.BeginTransaction())
                {
                    action(securityContext);
                    securityContext.SaveChanges();
                    transaction.Commit();
                }
            });
        }

        protected TResult Transaction<TResult>(Func<ISecurityContext, TResult> query)
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
