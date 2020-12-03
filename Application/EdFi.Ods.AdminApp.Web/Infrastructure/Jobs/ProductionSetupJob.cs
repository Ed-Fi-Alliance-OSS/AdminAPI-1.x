// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Services;
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

    public class ProductionSetupJob : WorkflowJob<int, ProductionSetupHub>, IProductionSetupJob
    {
        private const string WorkflowJobName = "Production Setup";
        private readonly IGetOdsSqlConfigurationQuery _getOdsSqlConfigurationQuery;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProductionSetupJob));
        private readonly ICloudOdsProductionLifecycleManagementService _productionDatabaseLifecycleManagementService;

        public ProductionSetupJob(
            ICloudOdsProductionLifecycleManagementService productionDatabaseLifecycleManagementService,
            IGetOdsSqlConfigurationQuery getOdsSqlConfigurationQuery, IBackgroundJobClient backgroundJobClient,
            ProductionSetupHub productionSetupHub
            , IHubContext<ProductionSetupHub> productionSetupHubContext)
            : base(backgroundJobClient, productionSetupHub, WorkflowJobName, productionSetupHubContext)
        {
            _productionDatabaseLifecycleManagementService = productionDatabaseLifecycleManagementService;
            _getOdsSqlConfigurationQuery = getOdsSqlConfigurationQuery;

            _productionDatabaseLifecycleManagementService.StatusUpdated += OperationStatusUpdated;
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

            var result = await _productionDatabaseLifecycleManagementService.ResetToMinimal(
                sqlConfig, jobCancellationToken?.ShutdownToken ?? CancellationToken.None);

            return result;
        }
    }
}
