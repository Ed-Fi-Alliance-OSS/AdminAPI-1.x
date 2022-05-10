// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Workflow;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class BulkImportService
    {
        private readonly IFileUploadHandler _fileUploadHandler;
        private readonly IInferOdsApiVersion _inferOdsApiVersion;

        public BulkImportService(IFileUploadHandler fileUploadHandler, IInferOdsApiVersion inferOdsApiVersion)
        {
            _fileUploadHandler = fileUploadHandler;
            _inferOdsApiVersion = inferOdsApiVersion;
        }

        public event WorkflowStatusUpdated StatusUpdated;

        public WorkflowResult Execute(BulkUploadJobContext bulkUploadJobContext)
        {
            try
            {
                var globalBulkUploadFolder = CloudOdsAdminAppSettings.AppSettings.BulkUploadHashCache;

                var instanceBulkUploadFolder =
                    $"{globalBulkUploadFolder}\\{bulkUploadJobContext.OdsInstanceId}";

                var workingFolderPath =
                    _fileUploadHandler.GetWorkingDirectory(instanceBulkUploadFolder);

                var fileImportConfig = new FileImportConfiguration().SetConfiguration(
                    bulkUploadJobContext, workingFolderPath);

                var fileImportService = new FileImportService(
                    fileImportConfig, bulkUploadJobContext.OdsInstanceName, _inferOdsApiVersion);

                var workflowManager = SetupWorkflowManager(bulkUploadJobContext, fileImportService);
                return workflowManager.Execute();
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occured while initializing the file importer: {ex.Message}";
                var status = new WorkflowStatus
                {
                    Complete = true,
                    CurrentStep = 0,
                    TotalSteps = 0,
                    Error = true,
                    ErrorMessage = errorMessage,
                };
                StatusUpdated?.Invoke(status);

                return new WorkflowResult
                {
                    Error = true,
                    ErrorMessage = errorMessage,
                    TotalSteps = 0,
                    StepsCompletedSuccessfully = 0,
                };
            }
        }

        private BulkUploadWorkflowManager SetupWorkflowManager(BulkUploadJobContext bulkUploadJobContext, FileImportService fileImportService)
        {
            var workflowManager = new BulkUploadWorkflowManager(bulkUploadJobContext);
            workflowManager.JobStarted += () => OnStatusUpdated(GetStatus(bulkUploadJobContext, workflowManager));
            workflowManager.JobCompleted += () => OnStatusUpdated(GetStatus(bulkUploadJobContext, workflowManager));
            workflowManager.StepStarted += () => OnStatusUpdated(GetStatus(bulkUploadJobContext, workflowManager));
            workflowManager.StepCompleted += () => OnStatusUpdated(GetStatus(bulkUploadJobContext, workflowManager));

            workflowManager
                .StartWith(new BulkUploadWorkflowStep
                {
                    StatusMessage = "Authenticating",
                    FailureMessage = "Authentication failed. Please ensure your credentials are valid",
                    RollBackAction = CleanUp,
                    ExecuteAction = context => Authenticate(fileImportService),
                    RollbackFailureMessage = "An error occured while authenticating",
                    RollbackStatusMessage = "An error occured while authenticating"
                })

                .ContinueWith(new BulkUploadWorkflowStep
                {
                    StatusMessage = "Validating XML",
                    FailureMessage = "An error occured validating XML file content.",
                    RollBackAction = CleanUp,
                    ExecuteAction = context => ValidateBulkDataXml(context, fileImportService),
                    RollbackFailureMessage = "An error occured validating XML file content",
                    RollbackStatusMessage = "An error occured validating XML file content"
                })

                .ContinueWith(new BulkUploadWorkflowStep
                {
                    StatusMessage = "Importing data",
                    FailureMessage = "Data import failed. {0}",
                    RollBackAction = CleanUp,
                    ExecuteAction = context => RunImport(fileImportService),
                    RollbackFailureMessage = "Data import failed",
                    RollbackStatusMessage = "Data import failed"
                })
                .ContinueWith(new BulkUploadWorkflowStep
                {
                    StatusMessage = "Cleaning up temporary files",
                    FailureMessage = "Import completed, but there was an error removing the temporary files.  {0}",
                    RollBackAction = context => { },
                    ExecuteAction = CleanUp,
                    RollbackFailureMessage = "",
                    RollbackStatusMessage = ""
                }).ContinueWith(new BulkUploadWorkflowStep
                {
                    StatusMessage = "Import completed, please check Admin App logs to see the details of your import.",
                    FailureMessage = "",
                    RollBackAction = context => { },
                    ExecuteAction = context => { },
                    RollbackFailureMessage = "",
                    RollbackStatusMessage = ""
                });

            return workflowManager;
        }

        private static void RunImport(FileImportService fileImportService)
        {
            var result = fileImportService.Execute();
            if (result != 0)
            {
                throw new Exception($"Data import failed with status code {result}");
            }
        }

        private static void Authenticate(FileImportService fileImportService)
        {
            try
            {
                var result = fileImportService.Authenticate();
                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("Failed to retrieve authentication token");
                }
            }
            catch (Exception)
            {
                throw new Exception("Error while retrieving authentication token");
            }
        }

        public void CleanUp(BulkUploadJobContext jobContext)
        {
            var uploadFolder = jobContext?.DataDirectoryFullPath;

            if (!string.IsNullOrEmpty(uploadFolder) && Directory.Exists(uploadFolder))
            {
                Directory.Delete(uploadFolder, true);
            }
        }

        private void ValidateBulkDataXml(BulkUploadJobContext jobContext, FileImportService fileImportService)
        {
            try
            {
                jobContext.ValidationResult = fileImportService.Validate();

                if (jobContext.ValidationResult != null && !jobContext.ValidationResult.Valid)
                {
                    throw new Exception("Validation failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during file validation: {ex.Message}");
            }
        }

        public WorkflowStatus GetStatus(BulkUploadJobContext bulkUploadJobContext, BulkUploadWorkflowManager workflowManager)
        {
            if (workflowManager == null)
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

            if (workflowManager.Status.Complete && bulkUploadJobContext.ValidationResult != null && !bulkUploadJobContext.ValidationResult.Valid)
            {
                var errorMessage = $"XML Validation failed: {string.Join(Environment.NewLine, bulkUploadJobContext.ValidationResult.ValidationErrors)}";
                var status = workflowManager.Status;
                status.Error = true;
                status.ErrorMessage = errorMessage;
                return status;
            }

            return workflowManager.Status;
        }

        protected virtual void OnStatusUpdated(WorkflowStatus status)
        {
            StatusUpdated?.Invoke(status);
        }
    }

    public class BulkUploadWorkflowStep : WorkflowStep<BulkUploadJobContext>
    {
    }

    public class BulkUploadWorkflowManager :
        WorkflowManager<BulkUploadWorkflowStep, BulkUploadJobContext>
    {
        public BulkUploadWorkflowManager(BulkUploadJobContext context) : base(context)
        {
        }

        public BulkUploadWorkflowManager(BulkUploadJobContext context,
            CancellationToken cancellationToken) : base(context, cancellationToken)
        {
        }
    }
}
