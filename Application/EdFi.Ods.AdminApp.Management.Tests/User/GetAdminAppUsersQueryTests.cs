// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.User;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class GetAdminAppUsersQueryTests
    {
        [Test]
        public void ShouldGetAdminAppUsersSortedByUserName()
        {
            ResetUsers();

            var testUsers = SetupUsers().OrderBy(x => x.UserName).ToList();

            var command = new GetAdminAppUsersQuery();

            var results = command.Execute();

            results.Count.ShouldBe(testUsers.Count);
            results.Select(x => x.UserName).ShouldBe(testUsers.Select(x => x.UserName));
        }
    }
}