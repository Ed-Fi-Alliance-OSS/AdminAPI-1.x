// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EdFi.Ods.Common.Configuration;

namespace EdFi.Ods.AdminApp.Management.Database
{
    public static class DatabaseProviderHelper
    {
        private static readonly string Provider;

        static DatabaseProviderHelper()
        {
            Provider = GetProviderName();
        }

        public static bool PgSqlProvider => Provider.Equals(ApiConfigurationConstants.PostgresProviderName, StringComparison.InvariantCultureIgnoreCase);
        
        private static string GetProviderName()
        {
            var connectionStringProviderByName = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (ConnectionStringSettings connectionStringSettings in ConfigurationManager.ConnectionStrings)
            {
                connectionStringProviderByName.Add(connectionStringSettings.Name, connectionStringSettings.ProviderName);
            }

            var grouping = connectionStringProviderByName
                .GroupBy(x => x.Value)
                .Select(g => g.Key)
                .ToList();

            if (grouping.Count != 1)
            {
                throw new NotSupportedException($"Multiple database modes are not supported. Supported database providers: {ApiConfigurationConstants.PostgresProviderName}, or {ApiConfigurationConstants.SqlServerProviderName}.");
            }

            return grouping.Single();
        }
    }
}