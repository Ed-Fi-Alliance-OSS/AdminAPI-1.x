// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.SqlClient;
using System.Net;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Filters;
#endif
using EdFi.Ods.AdminApp.Web.Controllers;
using log4net;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class HandleAjaxErrorAttribute : HandleErrorAttribute
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Global));

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest() || filterContext.IsChildAction)
            {
                _logger.Error(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                if (filterContext.Controller is ReportsController && filterContext.Exception is SqlException)
                    filterContext.HttpContext.Response.Write("An error occurred trying to access the SQL views for reports.");
                else
                    filterContext.HttpContext.Response.Write(filterContext.Exception.Message);
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}
