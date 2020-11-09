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
