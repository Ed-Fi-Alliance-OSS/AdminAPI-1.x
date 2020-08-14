// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace EdFi.Ods.AdminApp.Management.OnPrem
{
    public class OnPremOdsDatabaseNameProvider : BaseCloudOdsDatabaseNameProvider, ICloudOdsDatabaseNameProvider
    {
        private static readonly Dictionary<string, string> CachedDatabaseNames = new Dictionary<string, string>();

        public string GetDatabaseName(CloudOdsDatabases cloudOdsDatabase)
        {
            if (cloudOdsDatabase == null)
            {
                throw new ArgumentNullException(nameof(cloudOdsDatabase));
            }

            var switchDict = new Dictionary<CloudOdsDatabases, Func<string>>
            {
                { CloudOdsDatabases.Admin, () => RetrieveDatabaseNameFromConfigConnectionString(CloudOdsDatabaseNames.Admin) },
                { CloudOdsDatabases.Security, () => RetrieveDatabaseNameFromConfigConnectionString(CloudOdsDatabaseNames.Security) },
                { CloudOdsDatabases.ProductionOds, () => RetrieveDatabaseNameFromConfigConnectionString(CloudOdsDatabaseNames.ProductionOds)  },
                { CloudOdsDatabases.Master, GetMasterDatabaseName },
                { CloudOdsDatabases.MinimalTemplate, GetMinimalTemplateDatabaseName },
                { CloudOdsDatabases.PopulatedTemplate, GetPopulatedTemplateDatabaseName }
            };

            return switchDict.ContainsKey(cloudOdsDatabase) ? switchDict[cloudOdsDatabase]() : string.Empty;

            string RetrieveDatabaseNameFromConfigConnectionString(string connectionStringName)
            {
                if (CachedDatabaseNames.ContainsKey(connectionStringName))
                {
                    return CachedDatabaseNames[connectionStringName];
                }

                var connectionString = ConfigurationManager
                                            .ConnectionStrings[connectionStringName]?
                                            .ConnectionString;

                var builder = new SqlConnectionStringBuilder(connectionString);

                CachedDatabaseNames.Add(connectionStringName, builder.InitialCatalog);

                return CachedDatabaseNames[connectionStringName];
            }
        }
    }
}