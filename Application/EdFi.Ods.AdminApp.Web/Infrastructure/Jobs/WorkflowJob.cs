// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Workflow;
using Hangfire;
using log4net;
using Microsoft.AspNetCore.SignalR;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Jobs
{
    public abstract class WorkflowJob<TContext, THub>: IWorkflowJob<TContext>
        where THub: Hubs.EdfiOdsHub<THub>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(WorkflowJob<,>));
        private readonly IHubContext<THub> _hubContext;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private static int _runningJobCount = 0;
        private static long _runningJobId = 0;
        public string JobName { get; }

        protected WorkflowJob(IBackgroundJobClient backgroundJobClient, string jobName, IHubContext<THub> hubContext)
        {
            _backgroundJobClient = backgroundJobClient;
            _hubContext = hubContext;
            JobName = jobName;
        }

        protected abstract Task<WorkflowResult> RunJob(TContext jobContext, IJobCancellationToken jobCancellationToken);
        
        /// <summary>
        /// Logic to run the job task
        /// </summary>
        /// <param name="jobContext">Context value needed to run the job</param>
        /// <param name="jobCancellationToken"></param>
        //Disable automatic retries so our "runningJobCount" logic will work properly
        [AutomaticRetry(Attempts = 0)]
        public async Task<WorkflowResult> Execute(TContext jobContext, IJobCancellationToken jobCancellationToken)
        {
            _logger.Info($"Starting {JobName} job");
            jobCancellationToken?.ThrowIfCancellationRequested();

            try
            {
                var result = await RunJob(jobContext, jobCancellationToken);

                if (result.Error)
                {
                    _logger.Error($"Error during {JobName} job: {result.ErrorMessage}");
                }
                else
                {
                    _logger.Info($"{JobName} job completed");
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.Error($"Unhandled error during {JobName} job", e);

                var errorMessage = $"An error occured while executing the {JobName} job: {e.Message}";
                var status = new WorkflowStatus
                {
                    Complete = true,
                    CurrentStep = 0,
                    TotalSteps = 0,
                    Error = true,
                    ErrorMessage = errorMessage,
                };
                OperationStatusUpdated(status);

                throw;
            }
            finally
            {
                Interlocked.Exchange(ref _runningJobCount, 0);
                Interlocked.Exchange(ref _runningJobId, 0);
            }
        }

        public bool EnqueueJob(TContext jobContext)
        {
            if (Interlocked.CompareExchange(ref _runningJobCount, 1, 0) != 0)
            {
                _logger.Warn($"Ignoring {JobName} request - job is already running");

                var message = $"The {JobName} job is already running. Wait for the response.";
                var status = new WorkflowStatus
                {
                    StatusMessage = message,
                    Complete = false,
                    CurrentStep = 1,
                    TotalSteps = 3,
                    Error = false
                };
                OperationStatusUpdated(status);

                return false;
            }

            _logger.Info($"Queueing {JobName} job");
            var jobId = _backgroundJobClient.Enqueue<WorkflowJob<TContext, THub>>(x => x.Execute(jobContext, JobCancellationToken.Null));
            Interlocked.Exchange(ref _runningJobId, long.Parse(jobId));
            return true;
        }

        public bool IsJobRunning()
        {
            return Interlocked.CompareExchange(ref _runningJobCount, _runningJobCount, 0) != 0;
        }

        public bool IsSameOdsInstance(int odsInstanceId, Type contextType)
        {
            var jobId = Interlocked.Read(ref _runningJobId);
            if (jobId <= 0) return false;
            using (var connection = JobStorage.Current.GetConnection())
            {
                var jobData = connection.GetJobData(jobId.ToString());
                if (jobData != null && (jobData.State.Equals("Processing") || jobData.State.Equals("Enqueued")))
                {
                    var jobDetails = jobData.Job?.Args[0];
                    var type = jobDetails?.GetType();
                    var prop = type?.GetProperty("OdsInstanceId");
                    var runningJobOdsInstanceId = prop?.GetValue(jobDetails);
                    return runningJobOdsInstanceId != null && runningJobOdsInstanceId.Equals(odsInstanceId) &&
                           type == contextType;
                }
            }

            return false;
        }

        protected void OperationStatusUpdated(WorkflowStatus operationStatus)
        {
            SendStatusUpdate(operationStatus);
        }

        private async Task SendStatusUpdate(WorkflowStatus operationStatus)
        {
            await _hubContext.Clients.Group(typeof(THub).ToString()).SendAsync("UpdateStatus", operationStatus);
        }
    }
}
