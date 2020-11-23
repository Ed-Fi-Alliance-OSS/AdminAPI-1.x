// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using EdFi.Common.Utils.Extensions;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassInstanceContextFilter]
    [PermissionRequired(Permission.AccessGlobalSettings)]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<AdminAppUser> SignInManager;
        private readonly UserManager<AdminAppUser> UserManager;

        private readonly AddUserCommand _addUserCommand;
        private readonly EditUserCommand _editUserCommand;
        private readonly DeleteUserCommand _deleteUserCommand;
        private readonly GetAdminAppUserByIdQuery _getAdminAppUserByIdQuery;
        private readonly EditOdsInstanceRegistrationForUserCommand _editOdsInstanceRegistrationForUserCommand;
        private readonly EditUserRoleCommand _editUserRoleCommand;
        private readonly GetRoleForUserQuery _getRoleForUserQuery;
        private readonly IGetOdsInstanceRegistrationsByUserIdQuery _getOdsInstanceRegistrationsByUserIdQuery;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;
        private readonly ITabDisplayService _tabDisplayService;

        public UserController(AddUserCommand addUserCommand
            , EditUserCommand editUserCommand
            , DeleteUserCommand deleteUserCommand
            , GetAdminAppUserByIdQuery getAdminAppUserByIdQuery
            , EditOdsInstanceRegistrationForUserCommand editOdsInstanceRegistrationForUserCommand
            , EditUserRoleCommand editUserRoleCommand
            , GetRoleForUserQuery getRoleForUserQuery
            , IGetOdsInstanceRegistrationsByUserIdQuery getOdsInstanceRegistrationsByUserIdQuery
            , IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery
            , ITabDisplayService tabDisplayService
            , SignInManager<AdminAppUser> signInManager
            , UserManager<AdminAppUser> userManager
            )
        {
            _addUserCommand = addUserCommand;
            _editUserCommand = editUserCommand;
            _deleteUserCommand = deleteUserCommand;
            _getAdminAppUserByIdQuery = getAdminAppUserByIdQuery;
            _editOdsInstanceRegistrationForUserCommand = editOdsInstanceRegistrationForUserCommand;
            _editUserRoleCommand = editUserRoleCommand;
            _getRoleForUserQuery = getRoleForUserQuery;
            _getOdsInstanceRegistrationsByUserIdQuery = getOdsInstanceRegistrationsByUserIdQuery;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
            _tabDisplayService = tabDisplayService;            

            SignInManager = signInManager;
            UserManager = userManager;
        }

        public ActionResult AddUser()
        {
            return View("AddUser", new AddUserModel
            {
                GlobalSettingsTabEnumerations = GetGlobalSettingsTabsWithUsersSelected()
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(AddUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return AddUser();
            }

            var (userId, result) = await _addUserCommand.Execute(model, UserManager);
            if (!result.Succeeded)
            {
                model.GlobalSettingsTabEnumerations = GetGlobalSettingsTabsWithUsersSelected();
                AddErrors(result);
                return View(model);
            }
            _editUserRoleCommand.Execute(new EditUserRoleModel
            {
                UserId = userId,
                RoleId = Role.Admin.Value.ToString()
            });

            SuccessToastMessage("New User added successfully. \n Please inform the user of the initial password.");

            return RedirectToAction("Users", "GlobalSettings");
        }

        public ActionResult EditUser(string userId)
        {
            var existingUser = _getAdminAppUserByIdQuery.Execute(userId);

            return PartialView("_EditUserModal", new EditUserModel
            {
                UserId = userId,
                Email = existingUser.Email,
                GlobalSettingsTabEnumerations = GetGlobalSettingsTabsWithUsersSelected()
            });
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(EditUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return EditUser(model.UserId);
            }

            var result = await _editUserCommand.Execute(model, UserManager);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return EditUser(model.UserId);
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (model.UserId == loggedInUserId)
            {
                var user = _getAdminAppUserByIdQuery.Execute(model.UserId);

                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                }
            }

            return RedirectToActionJson<GlobalSettingsController>(x => x.Users(), "User updated successfully.");
        }

        public ActionResult EditOdsInstanceRegistrationsForUser(string userId)
        {
            var existingUser = _getAdminAppUserByIdQuery.Execute(userId);

            var odsInstanceRegistrations = _getOdsInstanceRegistrationsQuery.Execute().Select(x => new OdsInstanceRegistrationSelection
            {
                Name = x.Name,
                OdsInstanceRegistrationId = x.Id,
                Selected = false
            }).ToList();
            var userOdsInstanceRegistrationIds = _getOdsInstanceRegistrationsByUserIdQuery.Execute(userId).Select(x => x.Id);
            odsInstanceRegistrations.Where(x => userOdsInstanceRegistrationIds.Contains(x.OdsInstanceRegistrationId)).ForEach(x => x.Selected = true);
            return View("EditOdsInstanceRegistrationsForUser", new EditOdsInstanceRegistrationForUserModel
            {
                UserId = userId,
                Email = existingUser.Email,
                OdsInstanceRegistrations = odsInstanceRegistrations,
                GlobalSettingsTabEnumerations = GetGlobalSettingsTabsWithUsersSelected()
            });
        }

        [HttpPost]
        public ActionResult EditOdsInstanceRegistrationsForUser(EditOdsInstanceRegistrationForUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model.UserId);
            }

            _editOdsInstanceRegistrationForUserCommand.Execute(model);

            SuccessToastMessage("Assignments successfully updated.");

            return RedirectToAction("Users", "GlobalSettings");
        }

        public ActionResult EditUserRole(string userId)
        {
            var existingUser = _getAdminAppUserByIdQuery.Execute(userId);
            var currentRole = _getRoleForUserQuery.Execute(userId);

            return PartialView("_EditUserRoleModal", new EditUserRoleModel
            {
                UserId = userId,
                Email = existingUser.Email,
                RoleId = currentRole.Value.ToString(),
                GlobalSettingsTabEnumerations = GetGlobalSettingsTabsWithUsersSelected()
            });
        }

        [HttpPost]
        public ActionResult EditUserRole(EditUserRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return EditUserRole(model);
            }

            _editUserRoleCommand.Execute(model);

            return RedirectToActionJson<GlobalSettingsController>(x => x.Users(), "Role successfully assigned.");
        }

        public ActionResult DeleteUser(string userId)
        {
            var existingUser = _getAdminAppUserByIdQuery.Execute(userId);

            return PartialView("_DeleteUserModal", new DeleteUserModel
            {
                Email = existingUser.Email,
                UserId = userId
            });
        }

        [HttpPost]
        public ActionResult DeleteUser(DeleteUserModel model)
        {
            _deleteUserCommand.Execute(model);

            return RedirectToActionJson<GlobalSettingsController>(x => x.Users(), "User deleted successfully.");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private List<TabDisplay<GlobalSettingsTabEnumeration>> GetGlobalSettingsTabsWithUsersSelected()
        {
            return _tabDisplayService.GetGlobalSettingsTabDisplay(
                GlobalSettingsTabEnumeration.Users);
        }
    }
}
