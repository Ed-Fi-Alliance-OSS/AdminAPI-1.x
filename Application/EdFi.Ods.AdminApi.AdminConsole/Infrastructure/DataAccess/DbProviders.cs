// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;

public static class DbProviders
{
    public const string SqlServer = nameof(SqlServer);
    public const string PostgreSql = nameof(PostgreSql);

    public static string Parse(string value)
    {
        if (value.Equals(SqlServer, StringComparison.InvariantCultureIgnoreCase))
        {
            return SqlServer;
        }

        if (value.Equals(PostgreSql, StringComparison.InvariantCultureIgnoreCase))
        {
            return PostgreSql;
        }

        throw new NotSupportedException("Not supported DatabaseEngine \"" + value + "\". Supported engines: SqlServer, and PostgreSql.");
    }
}
