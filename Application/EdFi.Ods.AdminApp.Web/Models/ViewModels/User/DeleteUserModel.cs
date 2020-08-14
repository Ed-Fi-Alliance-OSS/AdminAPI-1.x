// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.User;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.User
{
    public class DeleteUserModel: IDeleteUserModel
    {
        public string UserId { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class DeleteUserModelValidator : AbstractValidator<DeleteUserModel>
    {
        private readonly AdminAppUserContext _userContext;

        public DeleteUserModelValidator(AdminAppUserContext userContext)
        {
            _userContext = userContext;
            RuleFor(m => m.UserId)
                .NotEmpty()
                .Must(BeAnExistingUser).WithMessage("The user you are trying to delete does not exist in the database.")
                .Must(NotBeCurrentUser).WithMessage("The user is not allowed to delete themselves.");
            RuleFor(m => m.Email).NotEmpty();
        }

        private bool NotBeCurrentUser(string userId)
        {
            return _userContext.User.Id != userId;
        }

        private bool BeAnExistingUser(string userId)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                return database.Users.Any(x => x.Id == userId);
            }
        }
    }
}