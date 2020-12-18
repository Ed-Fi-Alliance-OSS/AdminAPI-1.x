// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class UserContextFilter : IAuthorizationFilter
    {
        private readonly AdminAppUserContext _userContext;
        private readonly AdminAppIdentityDbContext _identity;

        public UserContextFilter(AdminAppUserContext userContext, AdminAppIdentityDbContext identity)
        {
            _userContext = userContext;
            _identity = identity;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var userId = filterContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _identity.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return;
            }

            _userContext.User = user;
            var userRoles = _identity.UserRoles.Where(x => x.UserId == user.Id).ToArray();
            _userContext.Permissions = PopulatePermissions(userRoles);
        }

        private static Permission[] PopulatePermissions(IEnumerable<IdentityUserRole<string>> userRoles)
        {
            IEnumerable<Permission> permissions = new Permission[] { };
            permissions = userRoles.Aggregate(permissions, (current, userRole) => current.Union(RolePermission.GetPermissions(userRole.RoleId)));
            return permissions.ToArray();
        }
    }
}
