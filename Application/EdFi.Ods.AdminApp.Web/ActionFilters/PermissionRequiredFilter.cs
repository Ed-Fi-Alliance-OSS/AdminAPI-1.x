// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class PermissionRequiredFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var userContext = DependencyResolver.Current.GetService<AdminAppUserContext>();
            var actionRequirements = ActionRequirements(filterContext);
            var controllerRequirements = ControllerRequirements(filterContext);

            foreach (var permission in actionRequirements.Union(controllerRequirements))
                if (!userContext.Has(permission))
                    Forbidden(filterContext);
        }

        private static IEnumerable<Permission> ActionRequirements(AuthorizationContext filterContext)
        {
            return ((PermissionRequiredAttribute[]) filterContext
                    .ActionDescriptor
                    .GetCustomAttributes(typeof(PermissionRequiredAttribute), true))
                .Select(x => x.RequiredPermission);
        }

        private static IEnumerable<Permission> ControllerRequirements(AuthorizationContext filterContext)
        {
            return ((PermissionRequiredAttribute[]) filterContext
                    .ActionDescriptor
                    .ControllerDescriptor
                    .GetCustomAttributes(typeof(PermissionRequiredAttribute), true))
                .Select(x => x.RequiredPermission);
        }

        private static void Forbidden(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionRequiredAttribute : Attribute
    {
        public Permission RequiredPermission { get; }

        public PermissionRequiredAttribute(Permission permission)
        {
            RequiredPermission = permission;
        }
    }
}