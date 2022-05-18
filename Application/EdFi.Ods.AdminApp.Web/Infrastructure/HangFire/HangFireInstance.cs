// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Hangfire.Storage;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.HangFire
{
    public static class HangFireInstance
    {
        public static void EnableWithoutSchemaMigration()
        {
            Start(false);
        }

        private static void Start(bool enableSchemaMigration)
        {
            var schemaName = "adminapp_hangfire";

            var connectionString = Startup.ConfigurationConnectionStrings.Admin;
            var isPostgreSql = "PostgreSQL".Equals(
                Startup.ConfigurationAppSettings.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase);

            if (isPostgreSql)
            {
                var options = new PostgreSqlStorageOptions
                {
                    SchemaName = schemaName
                };

                GlobalConfiguration.Configuration.UsePostgreSqlStorage(connectionString, options);
            }
            else
            {
                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = enableSchemaMigration,
                    SchemaName = schemaName
                };

                GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString, options);
            }
        }

        public static void RemoveAllScheduledJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }
    }
}
