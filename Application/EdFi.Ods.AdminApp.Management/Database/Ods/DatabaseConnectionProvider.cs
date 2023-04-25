// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Common.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.Data.SqlClient;
using EdFi.Ods.AdminApp.Management.Services;
using Npgsql;
using ApiMode = EdFi.Ods.AdminApp.Management.Instances.ApiMode;

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

        private readonly IOptions<AppSettings> _appSettings;

        public DatabaseConnectionProvider(IConnectionStringService connectionStringService
            , IOptions<AppSettings> appSettings
            )
        {
            _connectionStringService = connectionStringService;
            _appSettings = appSettings;
        }

        public IDbConnection CreateNewConnection(int odsInstanceNumericSuffix, ApiMode apiMode)
        {
            return CreateNewConnection(odsInstanceNumericSuffix.ToString(), apiMode);
        }

        public IDbConnection CreateNewConnection(string odsInstanceName, ApiMode apiMode)
        {
            var connectionString = _connectionStringService.GetConnectionString(odsInstanceName, apiMode);

            var isPostgreSql = ApiConfigurationConstants.PostgreSQL.Equals(_appSettings.Value.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase);

            if (isPostgreSql)
            {
                return new NpgsqlConnection(connectionString);
            }

            return new SqlConnection(connectionString);
        }
    }
}
