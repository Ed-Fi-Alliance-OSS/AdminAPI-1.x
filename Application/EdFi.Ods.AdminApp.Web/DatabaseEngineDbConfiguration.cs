// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using Npgsql;

namespace EdFi.Ods.AdminApp.Web
{
    public class DatabaseEngineDbConfiguration : DbConfiguration
    {
        public DatabaseEngineDbConfiguration(string databaseEngine)
        {
            if ("SqlServer".Equals(databaseEngine, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            const string name = "Npgsql";

            SetProviderFactory(
                providerInvariantName: name,
                providerFactory: NpgsqlFactory.Instance);

            SetProviderServices(
                providerInvariantName: name,
                provider: NpgsqlServices.Instance);

            SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
        }
    }
}
