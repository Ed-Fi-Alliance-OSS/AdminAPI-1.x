// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class EditUserRoleCommandTests
    {
        [SetUp]
        public void Init()
        {
            EnsureDefaultRoles();
        }

        [Test]
        public void ShouldEditUserRole()
        {
            var existingUsers = SetupUsers(2, Role.Admin).ToList();

            var userToBeSuperAdmin = existingUsers[0];
            var userToRemainAdmin = existingUsers[1];

            var guidString = Guid.NewGuid().ToString("N");

            var updateModel = new EditUserRoleModel
            {
                UserId = userToBeSuperAdmin.Id,
                RoleId = Role.SuperAdmin.Value.ToString()
            };

            var command = new EditUserRoleCommand();

            command.Execute(updateModel);

            var query = new GetRoleForUserQuery();

            var editedUserRole = query.Execute(userToBeSuperAdmin.Id);
            editedUserRole.ShouldBe(Role.SuperAdmin);
            
            var notEditedUserRole = query.Execute(userToRemainAdmin.Id);
            notEditedUserRole.ShouldBe(Role.Admin);
        }

        [Test]
        public void ShouldNotEditRoleIfUserDoesNotExists()
        {
            var testUserNotInSystem = new AdminAppUser
            {
                Email = $"testuser{Guid.NewGuid():N}@test.com",
                UserName = $"testuser{Guid.NewGuid():N}@test.com"
            };

            var updateModel = new EditUserRoleModel
            {
                UserId = testUserNotInSystem.Id,
                RoleId = Role.Admin.Value.ToString(),
                Email = testUserNotInSystem.Email
            };

            var validator = new EditUserRoleModelValidator(GetMockUserContext(testUserNotInSystem, Role.Admin));
            var validationResults = validator.Validate(updateModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("The user you are trying to edit does not exist in the database.");
        }

        [Test]
        public void ShouldNotEditRoleIfUserIsCurrentlyLoggedIn()
        {
            var existingUser = SetupUsers(1).Single();

            var updateModel = new EditUserRoleModel
            {
                UserId = existingUser.Id,
                RoleId = Role.Admin.Value.ToString(),
                Email = existingUser.Email
            };

            var validator = new EditUserRoleModelValidator(GetMockUserContext(existingUser, Role.Admin));
            var validationResults = validator.Validate(updateModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("The user is not allowed to assign/remove roles as they are logged in as a Super Administrator or have an Administrator role.");
        }

        [Test]
        public void ShouldNotEditRoleIfRoleDoesNotExist()
        {
            var existingUser = SetupUsers(1).Single();

            var notExistingRoleId = "3";

            var updateModel = new EditUserRoleModel
            {
                UserId = existingUser.Id,
                RoleId = notExistingRoleId,
                Email = existingUser.Email
            };

            var validator = new EditUserRoleModelValidator(GetMockUserContext(existingUser));
            var validationResults = validator.Validate(updateModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("The role you are trying to assign does not exist in the database.");
        }

        [Test]
        public void ShouldNotEditRoleIfRequiredFieldsEmpty()
        {
            var updateModel = new EditUserRoleModel();
            var validator = new EditUserRoleModelValidator(GetMockUserContext());
            var validationResults = validator.Validate(updateModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldBe(new List<string>
            {
                "'User Id' must not be empty.",
                "The user you are trying to edit does not exist in the database.",
                "'Role Id' must not be empty.",
                "The role you are trying to assign does not exist in the database.",
                "'Email' must not be empty."
            }, false);
        }
    }
}