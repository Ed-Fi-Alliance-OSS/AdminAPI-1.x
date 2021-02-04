// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureDatabaseLifecycleManagementService
    {
        private readonly AzureDatabaseManagementService _azureDatabaseManagementService;
        private readonly IAzureSqlSecurityConfigurator _cloudOdsSqlConfigurator;
        private readonly ICloudOdsDatabaseSqlServerSecurityConfiguration _cloudOdsDatabaseSqlServerSecurityConfiguration;
        private DdlSqlWorkflowManager _ddlSqlWorkflowManager = null;
        private readonly IRawSqlConnectionService _rawSqlConnectionService;
        private readonly ICloudOdsDatabaseNameProvider _cloudOdsDatabaseNameProvider;

        public event WorkflowStatusUpdated StatusUpdated;

        public AzureDatabaseLifecycleManagementService(
            AzureDatabaseManagementService azureDatabaseManagementService,
            IAzureSqlSecurityConfigurator cloudOdsSqlConfigurator,
            ICloudOdsDatabaseSqlServerSecurityConfiguration cloudOdsDatabaseSqlServerSecurityConfiguration,
            IRawSqlConnectionService rawSqlConnectionService,
            ICloudOdsDatabaseNameProvider cloudOdsDatabaseNameProvider)
        {
            _azureDatabaseManagementService = azureDatabaseManagementService;
            _cloudOdsSqlConfigurator = cloudOdsSqlConfigurator;
            _cloudOdsDatabaseSqlServerSecurityConfiguration = cloudOdsDatabaseSqlServerSecurityConfiguration;
            _rawSqlConnectionService = rawSqlConnectionService;
            _cloudOdsDatabaseNameProvider = cloudOdsDatabaseNameProvider;
        }

        [Obsolete("This operation is no longer intended to be reached, and should be phased out.")]
        public WorkflowResult ResetByCopyingTemplate(OdsSqlConfiguration sqlConfiguration, CloudOdsDatabases templateDatabase, CloudOdsDatabases copyToDatabase, CancellationToken cancellationToken)
        {
            AzurePerformanceLevel performanceLevel;
            var templateDatabaseName = _cloudOdsDatabaseNameProvider.GetDatabaseName(templateDatabase);
            var copyToDatabaseName = _cloudOdsDatabaseNameProvider.GetDatabaseName(copyToDatabase);
            var masterDatabaseName = _cloudOdsDatabaseNameProvider.GetDatabaseName(CloudOdsDatabases.Master);

            {
                var now = DateTime.UtcNow;
                var tempDbName = $"{copyToDatabaseName}_temp_{now:yyyyMMddHHmmss}";
                var oldDbName = $"{copyToDatabaseName}_old_{now:yyyyMMddHHmmss}";

                _ddlSqlWorkflowManager = new DdlSqlWorkflowManager(null, cancellationToken);

                _ddlSqlWorkflowManager
                    .SetWorkflowName($"Copy Azure database {templateDatabaseName} to {copyToDatabaseName}");

                return _ddlSqlWorkflowManager.Execute();
            }
        }
    }
}
