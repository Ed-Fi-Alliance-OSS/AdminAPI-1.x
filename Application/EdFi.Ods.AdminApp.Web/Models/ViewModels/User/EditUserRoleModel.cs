// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using FluentValidation;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.User
{
    public class EditUserRoleModel : IEditUserRoleModel
    {
        public string UserId { get; set; }

        public string RoleId { get; set; }

        public string Email { get; set; }

        public List<TabDisplay<GlobalSettingsTabEnumeration>> GlobalSettingsTabEnumerations { get; set; }
    }

    public class EditUserRoleModelValidator : AbstractValidator<EditUserRoleModel>
    {
        private readonly AdminAppUserContext _userContext;

        public EditUserRoleModelValidator(AdminAppUserContext userContext)
        {
            _userContext = userContext;

            RuleFor(m => m.UserId).NotEmpty()
                .Must(BeAnExistingUser).WithMessage("The user you are trying to edit does not exist in the database.")
                .Must(NotBeCurrentUser).WithMessage("The user is not allowed to assign/remove roles as they are logged in as a Super Administrator or have an Administrator role.");
            RuleFor(m => m.RoleId).NotEmpty().Must(BeAnExistingRole).WithMessage("The role you are trying to assign does not exist in the database.");
            RuleFor(m => m.Email).NotEmpty();
        }

        private bool NotBeCurrentUser(string userId)
        {
            if(_userContext.User != null)
                return !_userContext.User.Id.Equals(userId);
            return true;
        }

        private bool BeAnExistingUser(string userId)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                return database.Users.Any(x => x.Id == userId);
            }
        }

        private bool BeAnExistingRole(string roleId)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                return database.Set<IdentityRole>().Any(x => x.Id == roleId);
            }
        }
    }
}