// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureProductionLifecycleManagementService : ICloudOdsProductionLifecycleManagementService
    {
        private readonly AzureDatabaseLifecycleManagementService _azureDatabaseLifecycleManagementService;
        public event WorkflowStatusUpdated StatusUpdated;

        public AzureProductionLifecycleManagementService(AzureDatabaseLifecycleManagementService azureDatabaseLifecycleManagementService)
        {
            _azureDatabaseLifecycleManagementService = azureDatabaseLifecycleManagementService;
            _azureDatabaseLifecycleManagementService.StatusUpdated += OnStatusUpdated;
        }
        
        public Task<WorkflowResult> ResetToMinimal(OdsSqlConfiguration sqlConfiguration, CancellationToken cancellationToken)
        {
            return Task.FromResult(_azureDatabaseLifecycleManagementService.ResetByCopyingTemplate(sqlConfiguration, CloudOdsDatabases.MinimalTemplate, CloudOdsDatabases.ProductionOds, cancellationToken));
        }

        private void OnStatusUpdated(WorkflowStatus status)
        {
            StatusUpdated?.Invoke(status);
        }
    }
}
