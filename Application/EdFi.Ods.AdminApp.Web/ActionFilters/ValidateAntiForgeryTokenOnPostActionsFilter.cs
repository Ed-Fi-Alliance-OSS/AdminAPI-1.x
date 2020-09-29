// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Helpers;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Filters;
#endif

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class ValidateAntiForgeryTokenOnPostActionsFilter : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod == "POST")
                AntiForgery.Validate();
        }
    }
}
