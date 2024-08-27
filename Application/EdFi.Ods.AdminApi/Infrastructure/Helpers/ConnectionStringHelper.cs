// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using log4net;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace EdFi.Ods.AdminApi.Infrastructure.Helpers;

public static class ConnectionStringHelper
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(ConnectionStringHelper));
    public static bool ValidateConnectionString(string databaseEngine, string? connectionString)
    {
        bool result = true;
        if (databaseEngine.ToLowerInvariant() == DatabaseEngineEnum.SqlServer.ToLowerInvariant())
        {
            try
            {
                _ = new SqlConnectionStringBuilder(connectionString);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException ||
                    ex is FormatException ||
                    ex is KeyNotFoundException)
                {
                    result = false;
                    _log.Error(ex);
                }
            }
        }
        else if (databaseEngine.ToLowerInvariant() == DatabaseEngineEnum.PostgreSql.ToLowerInvariant())
        {
            try
            {
                _ = new NpgsqlConnectionStringBuilder(connectionString);
            }
            catch (ArgumentException ex)
            {
                result = false;
                _log.Error(ex);
            }
        }
        return result;
    }
}
