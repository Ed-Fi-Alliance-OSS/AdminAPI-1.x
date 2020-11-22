// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class HandleAjaxErrorAttribute : ExceptionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(HandleAjaxErrorAttribute));

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                _logger.Error(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var responseText = IsReportsController(filterContext.ActionDescriptor as ControllerActionDescriptor) && filterContext.Exception is SqlException
                    ? "An error occurred trying to access the SQL views for reports."
                    : filterContext.Exception.Message;
                filterContext.HttpContext.Response.WriteAsync(responseText);
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        private static bool IsReportsController(ControllerActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
                return false;

            return actionDescriptor.ControllerTypeInfo == typeof(ReportsController);
        }
    }
}
