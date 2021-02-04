// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.SqlClient;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureDatabaseManagementService
    {
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
    }
}
