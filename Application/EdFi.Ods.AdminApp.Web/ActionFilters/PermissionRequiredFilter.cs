// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class PermissionRequiredFilter : IAuthorizationFilter
    {
        private readonly Permission _permission;
        private readonly AdminAppUserContext _userContext;

        public PermissionRequiredFilter(Permission permission, AdminAppUserContext userContext)
        {
            _permission = permission;
            _userContext = userContext;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_userContext.Has(_permission))
                context.Result = new ForbidResult();
        }
    }
}
