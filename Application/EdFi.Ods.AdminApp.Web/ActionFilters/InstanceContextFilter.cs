// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
#if NET48
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
#else
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endif
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class InstanceContextFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (SkipFilter(filterContext)) return;

            OdsInstanceRegistration instance;
            if (CloudOdsAdminAppSettings.Instance.Mode.SupportsSingleInstance)
            {
                if (!TryQueryInstanceRegistration(out instance))
                {
                    filterContext.Result = new RedirectResult("~/Error/MultiInstanceError");
                    return;
                }
            }
            else
            {
                var unsafeInstanceId = filterContext.HttpContext.Request.Cookies.Get("Instance")?.Value;
                var userId = filterContext.HttpContext.User.Identity.GetUserId();

                if (!TryQueryInstanceRegistration(userId, unsafeInstanceId, out instance))
                {
                    filterContext.Result = new RedirectResult("~/OdsInstances");
                    return;
                }
            }

            var instanceContext = DependencyResolver.Current.GetService<InstanceContext>();

            instanceContext.Id = instance.Id;
            instanceContext.Name = instance.Name;
            instanceContext.Description = instance.Description;
        }

        private static bool TryQueryInstanceRegistration(out OdsInstanceRegistration instanceRegistration)
        {
            OdsInstanceRegistration singleInstanceLookup;
            using (var dbContext = new AdminAppDbContext())
            {
                singleInstanceLookup = dbContext.OdsInstanceRegistrations.FirstOrDefault(x =>
                    x.Name.Equals(CloudOdsAdminAppSettings.Instance.OdsInstanceName,
                        StringComparison.InvariantCultureIgnoreCase));
            }
            instanceRegistration = singleInstanceLookup;
            return singleInstanceLookup != null;
        }

        private static bool TryQueryInstanceRegistration(string userId, string unsafeInstanceId, out OdsInstanceRegistration instanceRegistration)
        {
            if (int.TryParse(unsafeInstanceId, out var safeInstanceId))
            {
                var isAuthorized = IsUserAuthorizedForInstance(safeInstanceId, userId);

                OdsInstanceRegistration instanceLookup;
                using (var dbContext = new AdminAppDbContext())
                {
                    instanceLookup = dbContext.OdsInstanceRegistrations.Find(safeInstanceId);
                }

                if (isAuthorized && instanceLookup != null)
                {
                    instanceRegistration = instanceLookup;
                    return true;
                }
            }

            instanceRegistration = null;
            return false;
        }

        private static bool SkipFilter(ControllerContext filterContext)
        {
            var hasBypassAttr = filterContext.Controller.GetType().GetAttribute<BypassInstanceContextFilter>() != null;
            return hasBypassAttr;
        }

        private static bool IsUserAuthorizedForInstance(int instanceId, string userId)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                return database.UserOdsInstanceRegistrations.Any(x =>
                    x.OdsInstanceRegistrationId == instanceId
                    && x.UserId == userId);
            }
        }
    }
}
