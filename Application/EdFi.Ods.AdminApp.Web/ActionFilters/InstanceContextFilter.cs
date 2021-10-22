// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class InstanceContextFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        private readonly InstanceContext _instanceContext;
        private readonly AdminAppDbContext _adminAppDbContext;
        private readonly AdminAppIdentityDbContext _adminAppIdentityDbContext;

        public InstanceContextFilter(InstanceContext instanceContext, AdminAppDbContext adminAppDbContext,
            AdminAppIdentityDbContext adminAppIdentityDbContext)
        {
            _instanceContext = instanceContext;
            _adminAppDbContext = adminAppDbContext;
            _adminAppIdentityDbContext = adminAppIdentityDbContext;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (SkipFilter(filterContext.ActionDescriptor as ControllerActionDescriptor)) return;

            OdsInstanceRegistration instance;
            if (CloudOdsAdminAppSettings.Instance.Mode.SupportsSingleInstance)
            {
                instance = GetQueryInstanceRegistration();

                if (instance == null)
                {
                    var errorMessage = "Invalid configuration:" +
                       " A previous version of Admin App was already setup and this version cannot continue to function." +
                       " See the Known Issues documentation page to rerun Admin App setup in this mode," +
                       " or switch back to using the previous mode.";

                    throw new AdminAppException(errorMessage) { AllowFeedback = false };
                }
            }
            else
            {
                var unsafeInstanceId = filterContext.HttpContext.Request.Cookies["Instance"];
                var userId = filterContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!TryQueryInstanceRegistration(userId, unsafeInstanceId, out instance))
                {
                    filterContext.Result = new RedirectResult("~/OdsInstances");
                    return;
                }
            }

            _instanceContext.Id = instance.Id;
            _instanceContext.Name = instance.Name;
            _instanceContext.Description = instance.Description;
        }

        private OdsInstanceRegistration GetQueryInstanceRegistration()
        {
            var singleInstanceLookup = _adminAppDbContext.OdsInstanceRegistrations.FirstOrDefault(x =>
                x.Name == CloudOdsAdminAppSettings.Instance.OdsInstanceName);

            return singleInstanceLookup;
        }

        private bool TryQueryInstanceRegistration(string userId, string unsafeInstanceId, out OdsInstanceRegistration instanceRegistration)
        {
            if (int.TryParse(unsafeInstanceId, out var safeInstanceId))
            {
                var isAuthorized = IsUserAuthorizedForInstance(safeInstanceId, userId);

                var instanceLookup = _adminAppDbContext.OdsInstanceRegistrations.Find(safeInstanceId);

                if (isAuthorized && instanceLookup != null)
                {
                    instanceRegistration = instanceLookup;
                    return true;
                }
            }

            instanceRegistration = null;
            return false;
        }

        private static bool SkipFilter(ControllerActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
                return false;

            var declaringType = actionDescriptor.MethodInfo.DeclaringType;
            return declaringType != null && declaringType.CustomAttributes.Any(ca => ca.AttributeType == typeof(BypassInstanceContextFilter));
        }

        private bool IsUserAuthorizedForInstance(int instanceId, string userId)
        {
            return _adminAppIdentityDbContext.UserOdsInstanceRegistrations.Any(
                x =>
                    x.OdsInstanceRegistrationId == instanceId
                    && x.UserId == userId);
        }
    }
}
