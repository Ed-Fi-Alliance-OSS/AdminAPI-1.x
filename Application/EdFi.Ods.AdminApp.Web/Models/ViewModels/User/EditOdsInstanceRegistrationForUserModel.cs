// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.User
{
    public class EditOdsInstanceRegistrationForUserModel: IEditOdsInstanceRegistrationForUserModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        [Display(Name = "Selected ODS Instances")]
        public List<OdsInstanceRegistrationSelection> OdsInstanceRegistrations { get; set; }

        public List<TabDisplay<GlobalSettingsTabEnumeration>> GlobalSettingsTabEnumerations { get; set; }

        public EditOdsInstanceRegistrationForUserModel()
        {
            OdsInstanceRegistrations = new List<OdsInstanceRegistrationSelection>();
        }
    }

    public class EditOdsInstanceRegistrationForUserModelValidator : AbstractValidator<EditOdsInstanceRegistrationForUserModel>
    {
        private static AdminAppDbContext _database;
        private readonly AdminAppIdentityDbContext _identity;

        public EditOdsInstanceRegistrationForUserModelValidator(AdminAppDbContext database, AdminAppIdentityDbContext identity)
        {
            _database = database;
            _identity = identity;

            RuleFor(m => m.UserId).NotEmpty().Must(BeAnExistingUser).WithMessage("The user you are trying to edit does not exist in the database.");
            RuleFor(m => m.Email).NotEmpty();
            RuleFor(m => m.OdsInstanceRegistrations).NotEmpty().Must(BeAnExistingOdsInstanceRegistration).WithMessage("A selected instance does not exist in the database."); ;
        }

        private bool BeAnExistingUser(string userId)
        {
            return _identity.Users.Any(x => x.Id == userId);
        }

        private bool BeAnExistingOdsInstanceRegistration(List<OdsInstanceRegistrationSelection> instances)
        {
            foreach (var instance in instances)
            {
                if (instance.Selected && !_database.OdsInstanceRegistrations.Any(x => x.Id == instance.OdsInstanceRegistrationId))
                    return false;
            }

            return true;
        }
    }
}
