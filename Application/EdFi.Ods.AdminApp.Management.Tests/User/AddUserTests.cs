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
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class AddUserTests
    {
        [Test]
        public async Task ShouldAddUserWithRequirePasswordChangeAsTrue()
        {
            var guidString = Guid.NewGuid().ToString("N");

            var newUser = new AddUserModel
            {
                Email = $"test{guidString}@test.com",
                Password = "Passw0rd!",
                ConfirmPassword = "Passw0rd!"
            };

            await ScopedAsync<UserManager<AdminAppUser>>(async manager =>
            {
                var command = new AddUserCommand();

                var (userId, identityResult) = await command.Execute(newUser, manager);

                string.Join(Environment.NewLine, identityResult.Errors.Select(x => x.Description)).ShouldBe("");
                identityResult.Succeeded.ShouldBeTrue();

                var addedUser = Query(userId);
                addedUser.UserName.ShouldBe($"test{guidString}@test.com");
                addedUser.Email.ShouldBe($"test{guidString}@test.com");
                addedUser.RequirePasswordChange.ShouldBe(true);
            });
        }

        [Test]
        public void ShouldNotAddUserIfEmailNotValid()
        {
            var newUser = new AddUserModel
            {
                Email = "not-a-valid-email",
                Password = "testPassword",
                ConfirmPassword = "testPassword"
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new AddUserModelValidator(identity);
                var validationResults = validator.Validate(newUser);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("'Email' is not a valid email address.");
            });
        }

        [Test]
        public void ShouldNotAddUserIfPasswordsDoNotMatch()
        {
            var newUser = new AddUserModel
            {
                Email = "test@test.com",
                Password = "testPassword",
                ConfirmPassword = "notTestPassword"
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new AddUserModelValidator(identity);
                var validationResults = validator.Validate(newUser);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("The password and confirmation password do not match.");
            });
        }

        [Test]
        public void ShouldNotAddUserIfRequiredFieldsEmpty()
        {
            var newUser = new AddUserModel();

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new AddUserModelValidator(identity);
                var validationResults = validator.Validate(newUser);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldBe(new List<string>
                {
                    "'Email' must not be empty.",
                    "'Password' must not be empty.",
                    "'Confirm Password' must not be empty."
                }, false);
            });
        }

        [Test, TestCaseSource("TestPasswords")]
        public void ShouldNotAddUserIfPasswordLengthNotValid(string testPassword)
        {

            var newUser = new AddUserModel
            {
                Email = "test@test.com",
                Password = testPassword,
                ConfirmPassword = testPassword
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new AddUserModelValidator(identity);
                var validationResults = validator.Validate(newUser);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldContain($"'Password' must be between 6 and 100 characters. You entered {testPassword.Length} characters.");
            });
        }

        [Test]
        public void ShouldNotAddUserIfEmailNotUnique()
        {
            ResetUsers();

            var existingUser = SetupUsers(1).Single();

            var newUser = new AddUserModel
            {
                Email = existingUser.Email,
                Password = "testPassword",
                ConfirmPassword = "testPassword"
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new AddUserModelValidator(identity);
                var validationResults = validator.Validate(newUser);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("A user with this email address already exists in the database.");
            });
        }

        private static readonly string[] TestPasswords = {
            "test",
            "test"+new string('0',110)
        };
    }
}
