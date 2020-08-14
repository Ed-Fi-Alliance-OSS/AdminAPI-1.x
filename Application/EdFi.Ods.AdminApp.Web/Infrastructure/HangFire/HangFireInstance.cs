// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Hangfire.Storage;
using EdFi.Ods.AdminApp.Management.Database;

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

            if (DatabaseProviderHelper.PgSqlProvider)
            {
                var options = new PostgreSqlStorageOptions
                {
                    SchemaName = schemaName
                };

                GlobalConfiguration.Configuration.UsePostgreSqlStorage("EdFi_Admin", options);
            }
            else
            {
                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = enableSchemaMigration,
                    SchemaName = schemaName
                };

                GlobalConfiguration.Configuration.UseSqlServerStorage("EdFi_Admin", options);
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