// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Web.Hubs;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Management.Workflow;
using Hangfire;
using EdFi.LoadTools.BulkLoadClient;
using EdFi.Ods.AdminApp.Management.Instances;
using Microsoft.AspNetCore.SignalR;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Jobs
{
    public interface IBulkUploadJob : IWorkflowJob<BulkUploadJobContext>
    {
    }
    public class BulkUploadJob : WorkflowJob<BulkUploadJobContext, BulkUploadHub>, IBulkUploadJob
    {
        private const string WorkflowJobName = "Bulk Upload";
        private readonly BulkImportService _bulkImportService;

        public BulkUploadJob(BulkImportService bulkImportService, IBackgroundJobClient backgroundJobClient, IHubContext<BulkUploadHub> bulkUploadHubContext)
            : base(backgroundJobClient, WorkflowJobName, bulkUploadHubContext)
        {
            _bulkImportService = bulkImportService;

            _bulkImportService.StatusUpdated += OperationStatusUpdated;
        }
        
        protected override Task<WorkflowResult> RunJob(BulkUploadJobContext jobContext, IJobCancellationToken jobCancellationToken)
        {
            return Task.FromResult(_bulkImportService.Execute(jobContext));
        }
    }

    public class BulkUploadJobContext
    {
        public string ApiServerUrl { get; set; }
        public string DataDirectoryFullPath { get; set; }
        public BulkLoadValidationResult ValidationResult { get; set; }
        public int OdsInstanceId { get; set; }
        public string OdsInstanceName { get; set; }
        public int? SchoolYear {
            get
            {
                if (CloudOdsAdminAppSettings.Instance.Mode.Equals(ApiMode.YearSpecific))
                {
                    return OdsInstanceName.ExtractNumericInstanceSuffix();
                }

                return null;
            }
        }
        public string ApiBaseUrl { get; set; }
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public string OauthUrl { get; set; }
        public string MetadataUrl { get; set; }
        public string DependenciesUrl { get; set; }
        public string SchemaPath { get; set; }
        public int MaxSimultaneousRequests { get; set; }
    }
}

