// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Management.OnPrem
{
    public class OnPremProductionLifecycleManagementService : ICloudOdsProductionLifecycleManagementService
    {
        public Task<WorkflowResult> ResetToMinimal(OdsSqlConfiguration sqlConfiguration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

#pragma warning disable CS0067
        //Warning CS0067 claims that StatusUpdated is never used.
        //This is misleading. It implements an event to satisfy
        //an interface. At least one implementation of the interface
        //truly uses it. Attempts to throw NotImplementedException
        //to address the warning results in exceptions at runtime.
        public event WorkflowStatusUpdated StatusUpdated;
#pragma warning restore CS0067
    }
}