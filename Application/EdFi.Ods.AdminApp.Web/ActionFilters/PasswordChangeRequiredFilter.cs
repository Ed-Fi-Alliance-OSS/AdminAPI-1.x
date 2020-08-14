// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class PasswordChangeRequiredFilter : ActionFilterAttribute
    {
        private ApplicationUserManager _userManager;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ShouldBypassThisFilter(filterContext))
                return;

            _userManager = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (IsPasswordChangeRequired())
            {
                filterContext.Result =
                    RouteHelpers.RedirectToActionRoute<IdentityController>(x => x.ChangePassword());
            }
        }

        private bool ShouldBypassThisFilter(ActionExecutingContext filterContext)
        {
            return filterContext.Controller.GetType().GetAttribute<BypassPasswordChangeRequiredFilterAttribute>() != null;
        }

        private bool IsPasswordChangeRequired()
        {
            var user = _userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
            return user != null && user.RequirePasswordChange;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BypassPasswordChangeRequiredFilterAttribute : Attribute
    {
    }
}