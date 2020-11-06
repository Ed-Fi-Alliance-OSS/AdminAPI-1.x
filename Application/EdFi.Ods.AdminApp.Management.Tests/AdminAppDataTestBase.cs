// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using NUnit.Framework;
using Respawn;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
#if NET48
using EdFi.Ods.AdminApp.Management.Helpers;
#else
using EdFi.Ods.AdminApp.Web;
#endif


namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public abstract class AdminAppDataTestBase
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

        private string ConnectionString
        {
            get
            {
                #if NET48
                return ConfigurationHelper.GetConnectionStrings().Admin;
                #else
                return Startup.ConfigurationConnectionStrings.Admin;
                #endif
            }
        }

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

        protected void DeleteAll<TEntity>() where TEntity : class
        {
            Transaction(database =>
            {
                foreach (var entity in database.Set<TEntity>())
                    database.Set<TEntity>().Remove(entity);
            });
        }

        protected int Count<TEntity>() where TEntity : class
        {
            return Transaction(database => database.Set<TEntity>().Count());
        }

        protected void Transaction(Action<AdminAppDbContext> action)
        {
            Scoped<AdminAppDbContext>(database =>
            {
                using (var transaction = database.Database.BeginTransaction())
                {
                    action(database);
                    database.SaveChanges();
                    transaction.Commit();
                }
            });
        }

        protected TResult Transaction<TResult>(Func<AdminAppDbContext, TResult> query)
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
