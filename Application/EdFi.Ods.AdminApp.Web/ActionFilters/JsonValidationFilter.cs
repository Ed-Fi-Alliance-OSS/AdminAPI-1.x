// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Mvc;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class JsonValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod != "POST" || filterContext.Controller.ViewData.ModelState.IsValid)
                return;

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                var result = new ContentResult();
                var content = JsonConvert.SerializeObject(filterContext.Controller.ViewData.ModelState,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                result.Content = content;
                result.ContentType = "application/json";

                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.HttpContext.Response.StatusCode = 400;
                filterContext.Result = result;
            }
        }
    }
}