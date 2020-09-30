// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Authorization;
#endif
using EdFi.Ods.AdminApp.Web.Helpers;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class AuthorizationFilter : AuthorizeAttribute
    {
        public const string AuthenticationRequiredHeaderName = "x-authentication-required";
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.AddHeader(AuthenticationRequiredHeaderName, "1");
                filterContext.Result = ResponseHelpers.JsonResult(new
                {
                    authentication_required = true
                });
                return;
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
