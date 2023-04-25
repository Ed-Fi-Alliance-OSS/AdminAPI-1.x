// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public interface IRawSqlConnectionService
    {
        string GetConnectionStringWithAdminCredentials(OdsSqlConfiguration configuration, string databaseName);
        Task ExecuteDdlAsync(SqlConnection connection, string sql, int commandTimeout = 300);
        void ExecuteDdl(SqlConnection connection, string sql, int commandTimeout = 300);
    }

    public class RawSqlConnectionService : IRawSqlConnectionService
    {
        public string GetConnectionStringWithAdminCredentials(OdsSqlConfiguration configuration, string databaseName)
        {
            return GetConnectionString(configuration.HostName, databaseName, configuration.AdminCredentials);
        }
        
        private string GetConnectionString(string hostName, string databaseName, OdsSqlAdminCredential sqlCredentials)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = hostName,
                UserID = sqlCredentials.UserName,
                Password = sqlCredentials.Password,
                InitialCatalog = databaseName,
                ConnectTimeout = 300
            };

            return connectionStringBuilder.ConnectionString;
        }

        public async Task ExecuteDdlAsync(SqlConnection connection, string sql, int commandTimeout = 300)
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = commandTimeout;
                await command.ExecuteNonQueryAsync();
            }
        }

        public void ExecuteDdl(SqlConnection connection, string sql, int commandTimeout = 300)
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = commandTimeout;
                command.ExecuteNonQuery();
            }
        }
    }
}
