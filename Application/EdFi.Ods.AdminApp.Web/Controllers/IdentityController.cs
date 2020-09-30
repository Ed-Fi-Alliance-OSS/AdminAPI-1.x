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

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Identity", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

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

        #region Disabled 'Forgot Password' Flow

        //The ASP.NET Identity template includes these controller actions and associated views
        //for the purpose of a "Forgot Password" flow. The user would elect to receive an email
        //containing a specially-crafted link which would allow them to prove they are the real
        //owner of that email account and reset their password. The feature, however, relies
        //on enabling an email service. It was deemed out of scope for Admin App's initial release
        //of the Identity system, so it is disabled further here with [NonAction] attributes.

        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Identity", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Identity");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Identity");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Identity");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        [NonAction] //This ASP.NET Identity feature is not enabled.
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

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
