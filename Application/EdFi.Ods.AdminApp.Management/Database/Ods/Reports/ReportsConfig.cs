// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Common.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
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

        private readonly IOptions<AppSettings> _appSettings;

        public ReportsConfigProvider(IConnectionStringService connectionStringService
            , IOptions<AppSettings> appSettings
            )
        {
            _connectionStringService = connectionStringService;
                _appSettings = appSettings;
        }

        public ReportsConfig Create(string odsInstanceName, ApiMode apiMode)
        {
            var isPostgreSql = ApiConfigurationConstants.PostgreSQL.Equals(_appSettings.Value.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase);

            return new ReportsConfig
            {
                ConnectionString = _connectionStringService.GetConnectionString(odsInstanceName, apiMode),
                ScriptFolder = isPostgreSql ? "Reports.PgSql" : "Reports.Sql"
            };
        }
    }
}
