// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Options;
using System;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Common.Database;
using EdFi.Common.Configuration;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public interface IConnectionStringBuilderAdapterFactory
    {
        IDbConnectionStringBuilderAdapter Get();
    }

    public class ConnectionStringBuilderAdapterFactory : IConnectionStringBuilderAdapterFactory
    {
        private readonly IOptions<AppSettings> _appSettings;

        public ConnectionStringBuilderAdapterFactory(IOptions<AppSettings> appSettings) => _appSettings = appSettings;

        public IDbConnectionStringBuilderAdapter Get()
        {
            var isPostgreSql = ApiConfigurationConstants.PostgreSQL.Equals(_appSettings.Value.DatabaseEngine,
            StringComparison.InvariantCultureIgnoreCase);

            if (isPostgreSql)
            {
                return new NpgsqlConnectionStringBuilderAdapter();
            }
            return new SqlConnectionStringBuilderAdapter();
        }
    }
}
