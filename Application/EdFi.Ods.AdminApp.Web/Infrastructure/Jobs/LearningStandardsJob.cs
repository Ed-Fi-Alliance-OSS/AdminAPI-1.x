// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Admin.LearningStandards.Core;
using EdFi.Admin.LearningStandards.Core.Configuration;
using EdFi.Admin.LearningStandards.Core.Services;
using EdFi.Admin.LearningStandards.Core.Services.Interfaces;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Workflow;
using EdFi.Ods.AdminApp.Web.Hubs;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Jobs
{
    public class LearningStandardsJobContext
    {
        public string ApiUrl { get; set; }

        public int? SchoolYear { get; set; }

        public int OdsInstanceId { get; set; }
    }

    public interface IProductionLearningStandardsJob : IWorkflowJob<LearningStandardsJobContext>
    {
    }

    public class ProductionLearningStandardsJob : LearningStandardsJob<ProductionLearningStandardsHub>,
        IProductionLearningStandardsJob
    {
        public ProductionLearningStandardsJob(
            IBackgroundJobClient backgroundJobClient,
            ILearningStandardsCorePluginConnector learningStandardsPlugin,
            IOdsSecretConfigurationProvider odsSecretConfigurationProvider,
            IHubContext<ProductionLearningStandardsHub> productionLearningStandardsHubContext
            )
            : base(backgroundJobClient, learningStandardsPlugin, odsSecretConfigurationProvider, productionLearningStandardsHubContext) { }
    }

    public abstract class LearningStandardsJob<THub> : WorkflowJob<LearningStandardsJobContext, THub>
        where THub : EdfiOdsHub<THub>
    {
        private const string LearningStandardsJobName = "Enable Learning Standards";
        private const string TaskName = "LearningStandardsJob";
        private readonly ILearningStandardsCorePluginConnector _learningStandardsPlugin;
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;

        protected LearningStandardsJob(
            IBackgroundJobClient backgroundJobClient,
            ILearningStandardsCorePluginConnector learningStandardsPlugin,
            IOdsSecretConfigurationProvider odsSecretConfigurationProvider
            , IHubContext<THub> hubContext)
            : base(backgroundJobClient, LearningStandardsJobName, hubContext)
        {
            _learningStandardsPlugin = learningStandardsPlugin;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
        }

        protected override async Task<WorkflowResult> RunJob(LearningStandardsJobContext jobContext,
            IJobCancellationToken jobCancellationToken)
        {
            var secureCredentials = await GetSecureCredentials(jobContext.OdsInstanceId);
            var edFiOdsApiConfiguration = GetEdFiOdsApiConfig(jobContext, secureCredentials);
            var academicBenchmarkApiAuthConfig = GetAcademicBenchmarkApiAuthConfig(secureCredentials);

            var validationResult =
                await ValidateConfiguration(edFiOdsApiConfiguration, academicBenchmarkApiAuthConfig);

            if (!validationResult.IsSuccess)
            {
                OnStatusComplete(
                    new LearningStandardsSynchronizerProgressInfo(
                        TaskName, 100,
                        new Exception(validationResult.ErrorMessage)));

                return new WorkflowResult
                {
                    ErrorMessage = validationResult.ErrorMessage,
                    Error = true
                };
            }

            var response = await _learningStandardsPlugin.LearningStandardsSynchronizer.SynchronizeAsync(
                edFiOdsApiConfiguration,
                academicBenchmarkApiAuthConfig,
                new LearningStandardsSynchronizationOptions(),
                jobCancellationToken.ShutdownToken,
                new Progress<LearningStandardsSynchronizerProgressInfo>(OnStatusUpdate));

            if (response.IsSuccess)
            {
                await SaveSuccessfulSync(jobContext.OdsInstanceId);
                OnStatusComplete(new LearningStandardsSynchronizerProgressInfo(TaskName, "Completed Successfully", 100));
            }
            else
            {
                var innerResponses = response.InnerResponses;

                if (innerResponses != null && innerResponses.Any(x => x.IsSuccess))
                {
                    const string link =
                        "<a href=\"https://techdocs.ed-fi.org/display/ADMIN/Known+Issues\">Admin App Known Issues</a>";

                    var failedResponses = innerResponses.Where(x => !x.IsSuccess);
                    await SaveSuccessfulSync(jobContext.OdsInstanceId);

                    var taskState =
                        $"Warning: {failedResponses.Count()} learning standards may not have synchronized correctly to this ODS/API instance. " +
                        "This occasionally occurs due to internal sequencing of learning standards items for your locality. " +
                        "The easiest way to fix is to re-run this synchronization process by clicking on " +
                        "the 'Update Now' button after clicking 'Reload' below. " +
                        $"More information on this issue can be found in TechDocs for {link}.";

                    OnStatusComplete(new LearningStandardsSynchronizerProgressInfo(TaskName, taskState, 100));
                }
                else
                {
                    OnStatusComplete(
                        new LearningStandardsSynchronizerProgressInfo(TaskName, 100, new Exception(response.ErrorMessage)));
                }
            }

            return new WorkflowResult
            {
                Error = !response.IsSuccess,
                ErrorMessage = response.ErrorMessage
            };
        }

        private async Task SaveSuccessfulSync(int odsInstanceId)
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(odsInstanceId);

            config.LearningStandardsCredential.SynchronizationWasSuccessful = true;
            config.LearningStandardsCredential.LastUpdatedDate = DateTime.Now;

            await _odsSecretConfigurationProvider.SetSecretConfiguration(config, odsInstanceId);
        }

        private async Task<IResponse> ValidateConfiguration(IEdFiOdsApiConfiguration apiConfig,
            IAuthenticationConfiguration abConfig)
        {
            var abresult = await _learningStandardsPlugin.LearningStandardsConfigurationValidator
                .ValidateLearningStandardProviderConfigurationAsync(abConfig);

            if (!abresult.IsSuccess)
            {
                return abresult;
            }

            return await _learningStandardsPlugin.LearningStandardsConfigurationValidator
                .ValidateEdFiOdsApiConfigurationAsync(
                    apiConfig);
        }

        private static AuthenticationConfiguration GetAcademicBenchmarkApiAuthConfig(SecureCredentials secureCredentials)
        {
            return new AuthenticationConfiguration(
                secureCredentials.AcademicBenchmarkApiKey,
                secureCredentials.AcademicBenchmarkApiSecret);
        }

        private static EdFiOdsApiConfiguration GetEdFiOdsApiConfig(LearningStandardsJobContext context,
            SecureCredentials credentials)
        {
            var oAuthAuthenticationConfiguration = new AuthenticationConfiguration(
                credentials.ProductionApiKey,
                credentials.ProductionApiSecret);

            return new EdFiOdsApiConfiguration(
                context.ApiUrl, EdFiOdsApiCompatibilityVersion.v3,
                oAuthAuthenticationConfiguration, context.SchoolYear);
        }

        private void OnStatusUpdate(LearningStandardsSynchronizerProgressInfo value)
        {
            var workflowStatus = MapWorkflowStatus(value);
            OperationStatusUpdated(workflowStatus);
        }

        private void OnStatusComplete(LearningStandardsSynchronizerProgressInfo value)
        {
            var workflowStatus = MapWorkflowStatus(value, true);
            OperationStatusUpdated(workflowStatus);
        }

        private static WorkflowStatus MapWorkflowStatus(LearningStandardsSynchronizerProgressInfo status,
            bool jobCompleted = false)
        {
            var hasError = status.TaskState.Contains("Error");
            var hasWarning = status.TaskState.Contains("Warning");

            return new WorkflowStatus
            {
                Error = hasError,
                ErrorMessage = hasError
                    ? status.TaskState
                    : null,
                TotalSteps = 100,
                CurrentStep = status.CompletedPercentage,
                Complete = status.CompletedPercentage >= 100 && jobCompleted,
                StatusMessage = status.TaskState,
                Warning = hasWarning
            };
        }

        private async Task<SecureCredentials> GetSecureCredentials(int odsInstanceId)
        {
            var configuration = await _odsSecretConfigurationProvider.GetSecretConfiguration(odsInstanceId);

            return new SecureCredentials
            {
                AcademicBenchmarkApiKey = configuration.LearningStandardsCredential.ApiKey,
                AcademicBenchmarkApiSecret = configuration.LearningStandardsCredential.ApiSecret,
                ProductionApiKey = configuration.ProductionAcademicBenchmarkApiClientKeyAndSecret?.ApiKey ?? string.Empty,
                ProductionApiSecret = configuration.ProductionAcademicBenchmarkApiClientKeyAndSecret?.ApiSecret ?? string.Empty
            };
        }

        private class SecureCredentials
        {
            public string AcademicBenchmarkApiSecret { get; set; }

            public string AcademicBenchmarkApiKey { get; set; }

            public string ProductionApiKey { get; set; }

            public string ProductionApiSecret { get; set; }
        }
    }
}
