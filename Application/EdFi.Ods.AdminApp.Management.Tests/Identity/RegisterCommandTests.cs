// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Identity;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Identity
{
    [TestFixture]
    public class RegisterCommandTests
    {
        [Test]
        public async Task ShouldRegisterUserWithRequirePasswordChangeAsFalse()
        {
            var guidString = Guid.NewGuid().ToString("N");

            var newUser = new RegisterViewModel
            {
                Email = $"test{guidString}@test.com",
                Password = "testPassword",
                ConfirmPassword = "testPassword"
            };

            var manager = SetupApplicationUserManager();

            var command = new RegisterCommand();

            var ( adminAppUser, _ ) = await command.Execute(newUser, manager);

            using (var database = AdminAppIdentityDbContext.Create())
            {
                var addedUser = database.Users.Single(x => x.Id == adminAppUser.Id);
                addedUser.UserName.ShouldBe($"test{guidString}@test.com");
                addedUser.Email.ShouldBe($"test{guidString}@test.com");
                addedUser.RequirePasswordChange.ShouldBe(false);
            }
        }

        [Test]
        public void ShouldNotRegisterUserIfEmailNotValid()
        {
            var newUser = new RegisterViewModel
            {
                Email = "not-a-valid-email",
                Password = "testPassword",
                ConfirmPassword = "testPassword"
            };

            var validator = new RegisterViewModelValidator();
            var validationResults = validator.Validate(newUser);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("'Email' is not a valid email address.");
        }

        [Test]
        public void ShouldNotRegisterUserIfPasswordsDoNotMatch()
        {
            var newUser = new RegisterViewModel
            {
                Email = "test@test.com",
                Password = "testPassword",
                ConfirmPassword = "notTestPassword"
            };

            var validator = new RegisterViewModelValidator();
            var validationResults = validator.Validate(newUser);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("The password and confirmation password do not match.");
        }

        [Test]
        public void ShouldNotRegisterUserIfRequiredFieldsEmpty()
        {
            var newUser = new RegisterViewModel();

            var validator = new RegisterViewModelValidator();
            var validationResults = validator.Validate(newUser);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldBe(new List<string>
            {
                "'Email' must not be empty.",
                "'Password' must not be empty.",
                "'Confirm Password' must not be empty."
            }, false);
        }

        [Test, TestCaseSource("TestPasswords")]
        public void ShouldNotRegisterUserIfPasswordLengthNotValid(string testPassword)
        {

            var newUser = new RegisterViewModel
            {
                Email = "test@test.com",
                Password = testPassword,
                ConfirmPassword = testPassword
            };

            var validator = new RegisterViewModelValidator();
            var validationResults = validator.Validate(newUser);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldContain($"'Password' must be between 6 and 100 characters. You entered {testPassword.Length} characters.");
        }

        [Test]
        public void ShouldNotRegisterUserIfEmailNotUnique()
        {
            EnsureZeroUsers();

            using (var database = AdminAppIdentityDbContext.Create())
            {
                var user = new AdminAppUser()
                {
                    Email = "existinguser@test.com",
                    UserName = "existinguser@test.com"
                };
                database.Users.Add(user);
                database.SaveChanges();
            }

            var newUser = new RegisterViewModel
            {
                Email = "existinguser@test.com",
                Password = "testPassword",
                ConfirmPassword = "testPassword"
            };

            var validator = new RegisterViewModelValidator();
            var validationResults = validator.Validate(newUser);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Select(x => x.ErrorMessage).ShouldContain("A user with the email already exists in the database");
        }

        private static readonly string[] TestPasswords = {
            "test",
            "test"+new string('0',110)
        };

        private static void EnsureZeroUsers()
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                foreach (var entity in database.Users)
                    database.Users.Remove(entity);
                database.SaveChanges();
            }
        }

        private static UserManager<AdminAppUser> SetupApplicationUserManager()
        {
            return new UserManager<AdminAppUser>(new UserStore<AdminAppUser>(AdminAppIdentityDbContext.Create()));
        }
    }
}