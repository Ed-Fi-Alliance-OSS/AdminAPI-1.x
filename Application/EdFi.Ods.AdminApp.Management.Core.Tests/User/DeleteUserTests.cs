// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.User;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;
using static EdFi.Ods.AdminApp.Management.Tests.Instance.InstanceTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class DeleteUserTests: AdminAppDataTestBase
    {
        [SetUp]
        public void Init()
        {
            EnsureDefaultRoles();
        }

        [Test]
        public void ShouldDeleteUser()
        {
            var existingUsers = SetupUsers(2, Role.Admin).ToList();

            var userToBeDeleted = existingUsers[0];
            var userNotToBeDeleted = existingUsers[1];

            var testInstances = SetupOdsInstanceRegistrations(6).OrderBy(x => x.Name).ToList();

            var testInstancesAssignedToDeletedUser = testInstances.Take(3).ToList();
            var testInstancesAssignedToNotDeletedUser = testInstances.Skip(3).Take(3).ToList();

            SetupUserWithOdsInstanceRegistrations(userToBeDeleted.Id, testInstancesAssignedToDeletedUser);
            SetupUserWithOdsInstanceRegistrations(userNotToBeDeleted.Id, testInstancesAssignedToNotDeletedUser);

            Scoped<IGetOdsInstanceRegistrationsByUserIdQuery>(queryInstances =>
            {
                queryInstances.Execute(userToBeDeleted.Id).Count().ShouldBe(3);
                queryInstances.Execute(userNotToBeDeleted.Id).Count().ShouldBe(3);
            });

            var deleteModel = new DeleteUserModel
            {
                Email = userToBeDeleted.Email,
                UserId = userToBeDeleted.Id
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var command = new DeleteUserCommand(identity);
                command.Execute(deleteModel);
            });

            Scoped<IGetOdsInstanceRegistrationsByUserIdQuery>(queryInstances =>
            {
                queryInstances.Execute(userToBeDeleted.Id).Count().ShouldBe(0);
                queryInstances.Execute(userNotToBeDeleted.Id).Count().ShouldBe(3);
            });

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var queryRoles = new GetRoleForUserQuery(identity);

                var deletedUser = Query(userToBeDeleted.Id);
                deletedUser.ShouldBeNull();
                queryRoles.Execute(userToBeDeleted.Id).ShouldBeNull();

                var notDeletedUser = Query(userNotToBeDeleted.Id);
                notDeletedUser.UserName.ShouldBe(userNotToBeDeleted.UserName);
                notDeletedUser.Email.ShouldBe(userNotToBeDeleted.Email);
                queryRoles.Execute(userNotToBeDeleted.Id).ShouldBe(Role.Admin);
            });
        }

        [Test]
        public void ShouldNotDeleteUserIfRequiredFieldsEmpty()
        {
            var superAdminUser = SetupUsers(1, Role.SuperAdmin).Single();

            var deleteModel = new DeleteUserModel
            {
                Email = "",
                UserId = ""
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new DeleteUserModelValidator(GetMockUserContext(superAdminUser, Role.SuperAdmin), identity);
                validator.ShouldNotValidate(deleteModel,
                    "'User Id' must not be empty.",
                    "The user you are trying to delete does not exist in the database.",
                    "'Email' must not be empty.");
            });
        }

        [Test]
        public void ShouldNotDeleteIfUserDoesNotExists()
        {
            var superAdminUser = SetupUsers(1, Role.SuperAdmin).Single();

            var testUserNotInSystem = new AdminAppUser
            {
                Email = $"testuser{Guid.NewGuid():N}@test.com",
                UserName = $"testuser{Guid.NewGuid():N}@test.com"
            };

            var deleteModel = new DeleteUserModel
            {
                Email = testUserNotInSystem.Email,
                UserId = testUserNotInSystem.Id
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new DeleteUserModelValidator(GetMockUserContext(superAdminUser, Role.SuperAdmin), identity);
                validator.ShouldNotValidate(
                    deleteModel,
                    "The user you are trying to delete does not exist in the database.");
            });
        }

        [Test]
        public void ShouldNotDeleteUserIfCurrentlyLoggedIn()
        {
            var superAdminUser = SetupUsers(1, Role.SuperAdmin).Single();

            var deleteModel = new DeleteUserModel
            {
                Email = superAdminUser.Email,
                UserId = superAdminUser.Id
            };

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var validator = new DeleteUserModelValidator(GetMockUserContext(superAdminUser, Role.SuperAdmin), identity);
                validator.ShouldNotValidate(
                    deleteModel,
                    "The user is not allowed to delete themselves.");
            });
        }
    }
}
