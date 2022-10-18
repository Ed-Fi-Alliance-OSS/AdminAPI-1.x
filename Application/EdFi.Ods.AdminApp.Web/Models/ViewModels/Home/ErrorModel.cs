// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Home
{
    public class ErrorModel: PageModel
    {
        public string Message { get; }
        public string StackTrace { get; }
        public new HttpStatusCode? StatusCode { get; }

        public bool IsStackTraceRelevant { get; }
        public bool AllowFeedback { get; }

        public ErrorModel(Exception exception, bool isProductImprovementEnabled)
        {
            StackTrace = exception.StackTrace;
            Message = exception.Message;

            if (exception is IAdminAppException adminAppException)
            {
                StatusCode = adminAppException.StatusCode;
                IsStackTraceRelevant = adminAppException.IsStackTraceRelevant;
                AllowFeedback = adminAppException.AllowFeedback;
            }
            else
            {
                StatusCode = HttpStatusCode.InternalServerError;
                IsStackTraceRelevant = true;
                AllowFeedback = true;
            }

            if (!isProductImprovementEnabled)
            {
                AllowFeedback = false;
            }
        }
    }
}
