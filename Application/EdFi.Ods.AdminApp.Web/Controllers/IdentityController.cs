// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Identity;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Identity;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter, BypassPasswordChangeRequiredFilter, BypassInstanceContextFilter]
    public class IdentityController : ControllerBase
    {
        private readonly ApplicationConfigurationService _applicationConfiguration;
        private readonly SignInManager<AdminAppUser> _signInManager;
        private readonly UserManager<AdminAppUser> _userManager;
        private readonly RegisterCommand _registerCommand;
        private readonly EditUserRoleCommand _editUserRoleCommand;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;
        private readonly IProductRegistration _productRegistration;
        private readonly AdminAppIdentityDbContext _identity;
        
        public IdentityController(ApplicationConfigurationService applicationConfiguration, RegisterCommand registerCommand, EditUserRoleCommand editUserRoleCommand, IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery,
            SignInManager<AdminAppUser> signInManager,
            UserManager<AdminAppUser> userManager,
            IProductRegistration productRegistration,
            AdminAppIdentityDbContext identity)
        {
            _applicationConfiguration = applicationConfiguration;
            _registerCommand = registerCommand;
            _editUserRoleCommand = editUserRoleCommand;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
            _signInManager = signInManager;
            _userManager = userManager;
            _productRegistration = productRegistration;
            _identity = identity;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

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
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _identity.Users.SingleOrDefaultAsync(x => x.UserName == model.Email);
                
                await _productRegistration.NotifyWhenEnabled(user);

                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                throw new NotSupportedException("Two-factor authentication is not supported.");
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
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
                var (adminAppUser, result) = await _registerCommand.Execute(model, _userManager);

                if (result.Succeeded)
                {
                    _editUserRoleCommand.Execute(new EditUserRoleModel
                    {
                        UserId = adminAppUser.Id,
                        RoleId = Role.SuperAdmin.Value.ToString()
                    });
                    await _signInManager.SignInAsync(adminAppUser, isPersistent: false);

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
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                user.RequirePasswordChange = false;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                TempData["PasswordChanged"] = true;

                if (ZeroOdsInstanceRegistrations())
                    return RedirectToAction("RegisterOdsInstance", "OdsInstances");

                return RedirectToAction("ChangePassword");
            }

            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
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
