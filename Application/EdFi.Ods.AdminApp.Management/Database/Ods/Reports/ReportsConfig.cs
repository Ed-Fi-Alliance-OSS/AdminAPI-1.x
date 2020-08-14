// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.Services;

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
       
        public ReportsConfigProvider(IConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public ReportsConfig Create(string odsInstanceName, ApiMode apiMode)
        {
            return new ReportsConfig
            {
                ConnectionString = _connectionStringService.GetConnectionString(odsInstanceName, apiMode),
                ScriptFolder = DatabaseProviderHelper.PgSqlProvider ? "Reports.PgSql" : "Reports.Sql"
            };
        }
    }
}
