// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.User;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class GetRoleForUserQueryTests
    {
        [Test]
        public void ShouldGetRoleForUserById()
        {
            EnsureDefaultRoles();

            var testSuperAdminUser = SetupUsers(1, Role.SuperAdmin).Single();
            var testAdminUser = SetupUsers(1, Role.Admin).Single();

            var command = new GetRoleForUserQuery();

            var result = command.Execute(testSuperAdminUser.Id);
            result.ShouldBe(Role.SuperAdmin);

            result = command.Execute(testAdminUser.Id);
            result.ShouldBe(Role.Admin);
        }
    }
}