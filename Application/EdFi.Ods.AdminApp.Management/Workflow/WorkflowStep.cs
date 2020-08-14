// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management.Workflow
{
    public class WorkflowStep<TContext>
    {
        public Action<TContext> ExecuteAction { get; set; }
        public Action<TContext> RollBackAction { get; set; }

        public bool RollbackPreviousSteps { get; set; } = true;

        public string StatusMessage { get; set; }
        public string RollbackStatusMessage { get; set; }
        public string FailureMessage { get; set; }
        public string RollbackFailureMessage { get; set; }

        public Func<string, WorkflowErrorContext, string> ErrorMessageFormatter { get; set; } = (format, ctx) => string.Format(format, ctx.Exception, ctx.Exception.InnerException);

        public string GetFormattedFailureMessage(Exception e)
        {
            return ErrorMessageFormatter?.Invoke(FailureMessage, new WorkflowErrorContext(e)) ?? FailureMessage;
        }

        public string GetFormattedRollbackFailureMessage(Exception e)
        {
            return ErrorMessageFormatter?.Invoke(RollbackFailureMessage, new WorkflowErrorContext(e)) ?? RollbackFailureMessage;
        }
    }
}
