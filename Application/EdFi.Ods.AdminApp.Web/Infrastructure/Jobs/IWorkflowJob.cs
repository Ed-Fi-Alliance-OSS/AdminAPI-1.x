// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Workflow;
using Hangfire;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Jobs
{
    public interface IWorkflowJob<in TContext>
    {
        Task<WorkflowResult> Execute(TContext jobContext, IJobCancellationToken jobCancellationToken);

        bool EnqueueJob(TContext jobContext);

        bool IsJobRunning();

        bool IsSameOdsInstance(int odsInstanceId, Type contextType);
    }
}