// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data;
using System.Data.SqlClient;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.Services;
using Npgsql;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public interface IDatabaseConnectionProvider
    {
        IDbConnection CreateNewConnection(int odsInstanceNumericSuffix, ApiMode apiMode);
        IDbConnection CreateNewConnection(string odsInstanceName, ApiMode apiMode);
    }

    public class DatabaseConnectionProvider : IDatabaseConnectionProvider
    {
        private readonly IConnectionStringService _connectionStringService;

        public DatabaseConnectionProvider(IConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public IDbConnection CreateNewConnection(int odsInstanceNumericSuffix, ApiMode apiMode)
        {
            return CreateNewConnection(odsInstanceNumericSuffix.ToString(), apiMode);
        }

        public IDbConnection CreateNewConnection(string odsInstanceName, ApiMode apiMode)
        {
            var connectionString = _connectionStringService.GetConnectionString(odsInstanceName, apiMode);

            if (DatabaseProviderHelper.PgSqlProvider)
            {
                return new NpgsqlConnection(connectionString);
            }

            return new SqlConnection(connectionString);
        }
    }
}
