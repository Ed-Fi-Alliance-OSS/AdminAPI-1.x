// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
#endif
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Identity;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Identity;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter, BypassPasswordChangeRequiredFilter, BypassInstanceContextFilter]
    public class IdentityController : ControllerBase
    {
        private readonly ApplicationConfigurationService _applicationConfiguration;
        private ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        private readonly RegisterCommand _registerCommand;
        private readonly EditUserRoleCommand _editUserRoleCommand;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;

        public IdentityController(ApplicationConfigurationService applicationConfiguration, RegisterCommand registerCommand, EditUserRoleCommand editUserRoleCommand, IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery)
        {
            _applicationConfiguration = applicationConfiguration;
            _registerCommand = registerCommand;
            _editUserRoleCommand = editUserRoleCommand;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View(new LoginViewModel
            {
                AllowUserRegistration = _applicationConfiguration.AllowUserRegistration()
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    throw new NotSupportedException("Two-factor authentication is not supported.");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (!_applicationConfiguration.AllowUserRegistration())
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!_applicationConfiguration.AllowUserRegistration())
                return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                var (adminAppUser, result) = await _registerCommand.Execute(model, UserManager);

                if (result.Succeeded)
                {
                    _editUserRoleCommand.Execute(new EditUserRoleModel
                    {
                        UserId = adminAppUser.Id,
                        RoleId = Role.SuperAdmin.Value.ToString()
                    });
                    await SignInManager.SignInAsync(adminAppUser, isPersistent: false, rememberBrowser: false);

                    if (ZeroOdsInstanceRegistrations())
                        return RedirectToAction("RegisterOdsInstance", "OdsInstances");

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            return View(model);
        }

        public ActionResult ChangePassword()
        {
            ViewBag.PasswordChanged = TempData["PasswordChanged"];

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    user.RequirePasswordChange = false;
                    await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                TempData["PasswordChanged"] = true;

                if (ZeroOdsInstanceRegistrations())
                    return RedirectToAction("RegisterOdsInstance", "OdsInstances");

                return RedirectToAction("ChangePassword");
            }
            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (ZeroOdsInstanceRegistrations())
                return RedirectToAction("RegisterOdsInstance", "OdsInstances");

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        private bool ZeroOdsInstanceRegistrations()
        {
            return CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances &&
                   !_getOdsInstanceRegistrationsQuery.Execute().Any();
        }
    }
}
