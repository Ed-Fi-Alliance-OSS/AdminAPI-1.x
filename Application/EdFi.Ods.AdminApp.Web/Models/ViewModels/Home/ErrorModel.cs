// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EdFi.Ods.AdminApp.Web.Controllers;
using log4net;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Home
{
    public class ErrorModel: PageModel
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));

        public string Message { get; }
        public string StackTrace { get; }
        public HttpStatusCode? StatusCode { get; }

        public bool IsStackTraceRelevant { get; }
        public bool AllowFeedback { get; }

        public ErrorModel(Exception exception)
        {
            StackTrace = exception.StackTrace;
            Message = exception.Message;

            switch (exception)
            {
                case AdminAppException adminException:
                    StatusCode = adminException.StatusCode;
                    IsStackTraceRelevant = adminException.IsStackTraceRelevant;
                    AllowFeedback = adminException.AllowFeedback;
                    break;
                case OdsApiConnectionException odsApiConnectionException:
                    StatusCode = odsApiConnectionException.StatusCode;
                    IsStackTraceRelevant = odsApiConnectionException.IsStackTraceRelevant;
                    AllowFeedback = odsApiConnectionException.AllowFeedback;
                    break;
                default:
                    StatusCode = HttpStatusCode.InternalServerError;
                    IsStackTraceRelevant = true;
                    AllowFeedback = true;
                    break;
            }

            _logger.Error(exception);
        }
    }
}
