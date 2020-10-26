// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class EditUserTests
    {
        [Test]
        public async Task ShouldEditUser()
        {
            ResetUsers();

            var existingUsers = SetupUsers(2).ToList();

            var userToBeEdited = existingUsers[0];
            var userNotToBeEdited = existingUsers[1];

            var guidString = Guid.NewGuid().ToString("N");

            var updateModel = new EditUserModel
            {
                Email = $"test{guidString}@test.com",
                UserId = userToBeEdited.Id
            };

            var manager = SetupApplicationUserManager();

            var command = new EditUserCommand();

            await command.Execute(updateModel, manager);

            var editedUser = Query(userToBeEdited.Id);
            editedUser.UserName.ShouldBe(updateModel.Email);
            editedUser.Email.ShouldBe(updateModel.Email);

            var notEditedUser = Query(userNotToBeEdited.Id);
            notEditedUser.UserName.ShouldBe(userNotToBeEdited.UserName);
            notEditedUser.Email.ShouldBe(userNotToBeEdited.Email);
        }

        [Test]
        public void ShouldNotEditUserIfEmailNotValid()
        {
            var existingUser = SetupUsers(1).Single();

            var updateModel = new EditUserModel
            {
                Email = "not-a-valid-email",
                UserId = existingUser.Id
            };

            using (var identity = AdminAppIdentityDbContext.Create())
            {
                var validator = new EditUserModelValidator(identity);
                var validationResults = validator.Validate(updateModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("'Email' is not a valid email address.");
            }
        }

        [Test]
        public void ShouldNotEditUserIfRequiredFieldsEmpty()
        {
            var existingUser = SetupUsers(1).Single();

            var updateModel = new EditUserModel
            {
                UserId = existingUser.Id
            };

            using (var identity = AdminAppIdentityDbContext.Create())
            {
                var validator = new EditUserModelValidator(identity);
                var validationResults = validator.Validate(updateModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldBe(new List<string>
                {
                    "'Email' must not be empty."
                }, false);
            }
        }

        [Test]
        public void ShouldNotEditUserIfEmailNotUnique()
        {
            ResetUsers();

            var existingUsers = SetupUsers(2).ToList();

            var userToBeEdited = existingUsers[0];
            var existingUser = existingUsers[1];

            var updateModel = new EditUserModel
            {
                Email = existingUser.Email,
                UserId = userToBeEdited.Id
            };

            using (var identity = AdminAppIdentityDbContext.Create())
            {
                var validator = new EditUserModelValidator(identity);
                var validationResults = validator.Validate(updateModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("A user with this email address already exists in the database.");
            }
        }

        private static UserManager<AdminAppUser> SetupApplicationUserManager()
        {
            return new UserManager<AdminAppUser>(new UserStore<AdminAppUser>(AdminAppIdentityDbContext.Create()));
        }
    }
}
