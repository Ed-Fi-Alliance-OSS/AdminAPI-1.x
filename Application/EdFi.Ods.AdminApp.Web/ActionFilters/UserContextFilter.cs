// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
#if NET48
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
#else
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
#endif
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Infrastructure;


namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class UserContextFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var userId = filterContext.HttpContext.User.Identity.GetUserId();

            TryQueryUser(userId, out var user);

            var userContext = DependencyResolver.Current.GetService<AdminAppUserContext>();

            userContext.User = user;
            if (user != null)
            {
                userContext.Permissions = PopulatePermissions(user.Roles);
            }
        }

        private static bool TryQueryUser(string userId, out AdminAppUser user)
        {
            AdminAppUser userLookup;
            using (var dbContext = AdminAppIdentityDbContext.Create())
            {
                userLookup = dbContext.Users.Include(x => x.Roles).SingleOrDefault(x => x.Id == userId);
            }

            if (userLookup != null)
            {
                user = userLookup;
                return true;
            }

            user = null;
            return false;
        }

        private static Permission[] PopulatePermissions(IEnumerable<IdentityUserRole> userRoles)
        {
            IEnumerable<Permission> permissions = new Permission[] {};
            permissions = userRoles.Aggregate(permissions, (current, userRole) => current.Union(RolePermission.GetPermissions(userRole.RoleId)));
            return permissions.ToArray();
        }
    }
}
