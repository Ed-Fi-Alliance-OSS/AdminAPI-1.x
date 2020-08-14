// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Identity;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Identity
{
    public class RegisterViewModel: IRegisterModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(m => m.Email).NotEmpty().EmailAddress().Must(BeAUniqueEmail).WithMessage("A user with the email already exists in the database");
            RuleFor(x => x.Password).NotEmpty().Length(6, 100);
            RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");
        }

        private static bool BeAUniqueEmail(string newEmail)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                return database.Users.ToList().All(x => x.Email != newEmail);
            }
        }
    }
}