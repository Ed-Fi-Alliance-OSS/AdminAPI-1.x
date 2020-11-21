// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.User;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.User
{
    [TestFixture]
    public class GetAdminAppUserByIdQueryTests
    {
        [Test]
        public void ShouldGetAdminAppUserById()
        {
            var testUser = SetupUsers(1).Single();

            Scoped<AdminAppIdentityDbContext>(identity =>
            {
                var command = new GetAdminAppUserByIdQuery(identity);

                var result = command.Execute(testUser.Id);
            
                result.Email.ShouldBe(testUser.Email);
                result.UserName.ShouldBe(testUser.UserName);
            });
        }
    }
}
