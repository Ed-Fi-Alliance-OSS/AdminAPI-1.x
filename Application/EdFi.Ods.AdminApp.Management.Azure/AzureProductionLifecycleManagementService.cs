// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    [Obsolete("This service is no longer intended to be reached, and should be phased out.")]
    public class AzureProductionLifecycleManagementService : ICloudOdsProductionLifecycleManagementService
    {
        public event WorkflowStatusUpdated StatusUpdated;

        public Task<WorkflowResult> ResetToMinimal(OdsSqlConfiguration sqlConfiguration, CancellationToken cancellationToken)
        {
            return Task.FromResult(new WorkflowResult());
        }
    }
}
