// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if !NET48
using System;
using EdFi.Common.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
#endif
using EdFi.Ods.AdminApp.Management.Services;
using ApiMode = EdFi.Ods.AdminApp.Management.Instances.ApiMode;

namespace EdFi.Ods.AdminApp.Management.Database.Ods.Reports
{
    public class ReportsConfig
    {
        public string ConnectionString { get; set; }

        public string ScriptFolder { get; set; }
    }

    public interface IReportsConfigProvider
    {
        ReportsConfig Create(string odsInstanceName, ApiMode apiMode);
    }

    public class ReportsConfigProvider : IReportsConfigProvider
    {
        private readonly IConnectionStringService _connectionStringService;

        #if !NET48
            private readonly IOptions<AppSettings> _appSettings;
        #endif

        public ReportsConfigProvider(IConnectionStringService connectionStringService
            #if !NET48
            , IOptions<AppSettings> appSettings
            #endif
            )
        {
            _connectionStringService = connectionStringService;
            #if !NET48
                _appSettings = appSettings;
            #endif
        }

        public ReportsConfig Create(string odsInstanceName, ApiMode apiMode)
        {
            #if NET48
                var isPostgreSql = DatabaseProviderHelper.PgSqlProvider;
            #else
                var isPostgreSql = ApiConfigurationConstants.PostgreSQL.Equals(_appSettings.Value.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase);
            #endif

            return new ReportsConfig
            {
                ConnectionString = _connectionStringService.GetConnectionString(odsInstanceName, apiMode),
                ScriptFolder = isPostgreSql ? "Reports.PgSql" : "Reports.Sql"
            };
        }
    }
}
