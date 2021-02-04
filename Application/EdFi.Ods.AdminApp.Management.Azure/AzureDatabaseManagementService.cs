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

        public void UpdateDatabasePerformanceLevel(SqlConnection connection, string databaseName, AzurePerformanceLevel newPerformanceLevel)
        {
            var sql = $"ALTER DATABASE [{databaseName}] MODIFY (EDITION = '{newPerformanceLevel.Edition}', SERVICE_OBJECTIVE='{newPerformanceLevel.ServiceObjective}')";
            _rawSqlConnectionService.ExecuteDdl(connection, sql);
        }
    }
}
