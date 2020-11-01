// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Security.Claims;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class PasswordChangeRequiredFilter : IAsyncActionFilter
    {
        private readonly UserManager<AdminAppUser> _userManager;

        public PasswordChangeRequiredFilter(UserManager<AdminAppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            if (!ShouldBypassThisFilter(filterContext))
            {
                if (await IsPasswordChangeRequired(filterContext))
                {
                    filterContext.Result =
                        RouteHelpers.RedirectToActionRoute<IdentityController>(x => x.ChangePassword());

                    return;
                }
            }

            await next();
        }

        private bool ShouldBypassThisFilter(ActionExecutingContext filterContext)
        {
            return filterContext.Controller.GetType().GetAttribute<BypassPasswordChangeRequiredFilterAttribute>() != null;
        }

        private async Task<bool> IsPasswordChangeRequired(ActionExecutingContext filterContext)
        {
            var userId = filterContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);
            return user != null && user.RequirePasswordChange;
        }
    }
}
