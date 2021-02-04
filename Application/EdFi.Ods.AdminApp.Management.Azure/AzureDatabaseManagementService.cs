// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.SqlClient;
using System.Threading;
using EdFi.Ods.AdminApp.Management.Database;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureDatabaseManagementService
    {
        private readonly IRawSqlConnectionService _rawSqlConnectionService;

        public AzureDatabaseManagementService(IRawSqlConnectionService rawSqlConnectionService)
        {
           _rawSqlConnectionService = rawSqlConnectionService;
        }

        public virtual AzurePerformanceLevel GetDatabasePerformanceLevel(SqlConnection connection, string databaseName)
        {
            var sql = $"SELECT DATABASEPROPERTYEX('{databaseName}', 'ServiceObjective') AS ServiceObjective, DATABASEPROPERTYEX('{databaseName}', 'Edition') AS Edition";
            using (var command = new SqlCommand(sql, connection))
            {
                var reader = command.ExecuteReader();
                reader.Read();

                var edition = reader["Edition"] as string;
                var serviceObjective = reader["ServiceObjective"] as string;

                return new AzurePerformanceLevel(edition, serviceObjective);
            }
        }

        public void RenameDatabase(SqlConnection connection, string oldName, string newName)
        {
            var sql = $"ALTER DATABASE [{oldName}] MODIFY NAME=[{newName}]";
            _rawSqlConnectionService.ExecuteDdl(connection, sql);
        }

        public void CopyDatabase(SqlConnection connection, string sourceDatabaseName, string destinationDatabaseName)
        {
            var sql = $"CREATE DATABASE [{destinationDatabaseName}] AS COPY OF [{sourceDatabaseName}]";
            _rawSqlConnectionService.ExecuteDdl(connection, sql);
        }

        public void UpdateDatabasePerformanceLevel(SqlConnection connection, string databaseName, AzurePerformanceLevel newPerformanceLevel)
        {
            var sql = $"ALTER DATABASE [{databaseName}] MODIFY (EDITION = '{newPerformanceLevel.Edition}', SERVICE_OBJECTIVE='{newPerformanceLevel.ServiceObjective}')";
            _rawSqlConnectionService.ExecuteDdl(connection, sql);
        }

        /// <summary>
        /// Waits for a *new* Azure database created via the CopyDatabase method to finish copying and be brought into an ONLINE state
        /// </summary>
        public void EnsureCopyDatabaseCompleted(SqlConnection connection, string databaseName, int maxWaitTimeInSeconds = 300)
        {
            var copyOperationSql = $"SELECT state_desc FROM sys.dm_operation_status WHERE major_resource_id = '{databaseName}'";
            var databaseStatusSql = $"SELECT state_desc FROM sys.databases WHERE name = '{databaseName}'";

            var processStart = DateTime.Now;
            var waitTimeInMilliseconds = 1000;

            while (DateTime.Now.Subtract(processStart).TotalSeconds < maxWaitTimeInSeconds)
            {
                using (var dbStatusCommand = new SqlCommand(databaseStatusSql, connection))
                {
                    var dbStatus = dbStatusCommand.ExecuteScalar() as string;

                    if (dbStatus != null && dbStatus.Equals("ONLINE", StringComparison.InvariantCultureIgnoreCase))
                        return;

                    //if we don't find the database in the sys.databases view, it may
                    //be that the copy operation is not started or has failed, so we'll check
                    //the operation status view instead
                    if (dbStatus == null)
                    {
                        using (var copyStatusCommand = new SqlCommand(copyOperationSql, connection))
                        {
                            var reader = copyStatusCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                var status = reader[0] as string;
                                if (status != null && status.Equals("FAILED", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    throw new Exception("Database copy operation failed");
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(waitTimeInMilliseconds);
                waitTimeInMilliseconds *= 2;
            }

            throw new TimeoutException($"Timeout while waiting for '{databaseName}' to finish copying");
        }
    }
}
