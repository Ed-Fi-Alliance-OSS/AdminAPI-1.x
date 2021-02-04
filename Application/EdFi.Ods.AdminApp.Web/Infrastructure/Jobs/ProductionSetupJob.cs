// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Workflow;
using EdFi.Ods.AdminApp.Web.Hubs;
using Hangfire;
using log4net;
using Microsoft.AspNetCore.SignalR;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Jobs
{
    public interface IProductionSetupJob : IWorkflowJob<int>
    {
    }

    [Obsolete("This job is no longer intended to be reached, and should be phased out.")]
    public class ProductionSetupJob : WorkflowJob<int, ProductionSetupHub>, IProductionSetupJob
    {
        private const string WorkflowJobName = "Production Setup";
        private readonly IGetOdsSqlConfigurationQuery _getOdsSqlConfigurationQuery;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProductionSetupJob));

        public ProductionSetupJob(
            IGetOdsSqlConfigurationQuery getOdsSqlConfigurationQuery, IBackgroundJobClient backgroundJobClient, IHubContext<ProductionSetupHub> productionSetupHubContext)
            : base(backgroundJobClient, WorkflowJobName, productionSetupHubContext)
        {
            _getOdsSqlConfigurationQuery = getOdsSqlConfigurationQuery;
        }

        /// <summary>
        /// Logic to run the production reset task
        /// </summary>
        /// <param name="jobContext">ProductionSetupOperation enumeration value.  Passed as an int because Hangfire does not support deserialization of the Enumeration class</param>
        /// <param name="jobCancellationToken"></param>
        /// <returns></returns>
        protected override async Task<WorkflowResult> RunJob(int jobContext, IJobCancellationToken jobCancellationToken)
        {
            var sqlConfig = await _getOdsSqlConfigurationQuery.Execute();

            _logger.Info($"User requested {WorkflowJobName} operation 'Interactive SetUp'");

            return new WorkflowResult();
        }
    }
}
