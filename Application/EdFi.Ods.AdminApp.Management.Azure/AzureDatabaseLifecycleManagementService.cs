// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Setup;
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

            using (var connection = _rawSqlConnectionService.GetDatabaseConnectionWithAdminCredentials(sqlConfiguration, copyToDatabaseName))
            {
                performanceLevel = _azureDatabaseManagementService.GetDatabasePerformanceLevel(connection, copyToDatabaseName);
            }

            using (var connection = _rawSqlConnectionService.GetDatabaseConnectionWithAdminCredentials(sqlConfiguration, masterDatabaseName))
            {
                var now = DateTime.UtcNow;
                var tempDbName = $"{copyToDatabaseName}_temp_{now:yyyyMMddHHmmss}";
                var oldDbName = $"{copyToDatabaseName}_old_{now:yyyyMMddHHmmss}";

                _ddlSqlWorkflowManager = new DdlSqlWorkflowManager(connection, cancellationToken);

                _ddlSqlWorkflowManager.JobStarted += () => OnStatusUpdated(GetStatus());
                _ddlSqlWorkflowManager.JobCompleted += () => OnStatusUpdated(GetStatus());
                _ddlSqlWorkflowManager.StepStarted += () => OnStatusUpdated(GetStatus());
                _ddlSqlWorkflowManager.StepCompleted += () => OnStatusUpdated(GetStatus());

                _ddlSqlWorkflowManager
                    .SetWorkflowName($"Copy Azure database {templateDatabaseName} to {copyToDatabaseName}")
                    .StartWith(
                        new DdlSqlWorkflowStep
                        {
                            ExecuteAction =
                                conn =>
                                {
                                    _azureDatabaseManagementService.CopyDatabase(conn, templateDatabaseName, tempDbName);
                                    _azureDatabaseManagementService.EnsureCopyDatabaseCompleted(conn, tempDbName);
                                },
                            RollBackAction = conn => _azureDatabaseManagementService.DropDatabase(conn, tempDbName), //delete the extra database copy
                            StatusMessage = "Copying template database",
                            RollbackStatusMessage = "Removing copy of template database",
                            FailureMessage = $"Error trying to copy {templateDatabase.DisplayName} to new database",
                            RollbackFailureMessage = $"Error trying to remove temporary database '{tempDbName}'",
                        }
                    ).ContinueWith(
                        new DdlSqlWorkflowStep
                        {
                            ExecuteAction = conn => _azureDatabaseManagementService.RenameDatabase(conn, copyToDatabaseName, oldDbName),
                            RollBackAction = conn => _azureDatabaseManagementService.RenameDatabase(conn, oldDbName, copyToDatabaseName),
                            StatusMessage = "Renaming old database",
                            RollbackStatusMessage = "Renaming old database",
                            FailureMessage =
                                $"Error trying to rename old {copyToDatabase.DisplayName} database from '{copyToDatabaseName}' to '{oldDbName}' prior to removal",
                            RollbackFailureMessage =
                                $"Error trying to rename old {copyToDatabase.DisplayName} database '{oldDbName}' back to '{copyToDatabaseName}'"
                        }
                    ).ContinueWith(
                        new DdlSqlWorkflowStep
                        {
                            ExecuteAction = conn => _azureDatabaseManagementService.RenameDatabase(conn, tempDbName, copyToDatabaseName),
                            RollBackAction = conn => { },
                            //worst case there's an extra DB online, but the old database is still online.  nothing to cleanup in this step as we'll delete the temp db further up the chain
                            StatusMessage = "Renaming copied database",
                            RollbackStatusMessage = "",
                            FailureMessage =
                                $"Error trying to rename new {copyToDatabase.DisplayName} database from '{tempDbName}' to '{copyToDatabaseName}'",
                            RollbackFailureMessage = ""
                        }
                    ).ContinueWith(
                        new DdlSqlWorkflowStep
                        {
                            ExecuteAction =
                                conn =>
                                {
                                    var securityConfiguration =
                                        _cloudOdsDatabaseSqlServerSecurityConfiguration.GetRuntimeConfiguration(sqlConfiguration)
                                        .Where(d => d.TargetDatabase == copyToDatabase);

                                    _cloudOdsSqlConfigurator.ApplyConfiguration(sqlConfiguration, securityConfiguration);
                                },
                            RollBackAction = conn => { },
                            StatusMessage = $"Resetting security on {copyToDatabaseName}",
                            RollbackStatusMessage = "",
                            FailureMessage = $"Error trying to re-create database logins in {copyToDatabaseName}",
                            RollbackFailureMessage = "",
                        }
                    ).ContinueWith(
                        new DdlSqlWorkflowStep
                        {
                            ExecuteAction = conn => _azureDatabaseManagementService.UpdateDatabasePerformanceLevel(conn, copyToDatabaseName, performanceLevel),
                            RollBackAction = conn => { },
                            //scale up request failed, nothing to rollback
                            StatusMessage = "Updating database performance level",
                            RollbackStatusMessage = "",
                            FailureMessage =
                                $"Error trying to scale {copyToDatabase.DisplayName} database to correct performance level",
                            RollbackFailureMessage = ""
                        }
                    ).ContinueWith(
                        new DdlSqlWorkflowStep
                        {
                            ExecuteAction = conn => _azureDatabaseManagementService.DropDatabase(conn, oldDbName),
                            RollBackAction = conn => { },
                            FailureMessage =
                                $"Error trying to remove database {oldDbName}.  You should remove this database manually in the Azure Portal to avoid incurring extra charges.",
                            StatusMessage = "Deleting old database",
                            RollbackStatusMessage = "",
                            RollbackFailureMessage = "",
                            RollbackPreviousSteps = false
                            //worst case there's an extra DB online, but the system is still operational, so there's nothing to rollback here
                        }
                    );

                return _ddlSqlWorkflowManager.Execute();
            }
        }
        
        public WorkflowStatus GetStatus()
        {
            if (_ddlSqlWorkflowManager == null)
            {
                return new WorkflowStatus
                {
                    StatusMessage = "Operation not yet started.",
                    Complete = false,
                    CurrentStep = 0,
                    Error = false,
                    TotalSteps = 0
                };
            }

            return _ddlSqlWorkflowManager.Status;
        }

        protected virtual void OnStatusUpdated(WorkflowStatus status)
        {
            StatusUpdated?.Invoke(status);
        }
    }
}
