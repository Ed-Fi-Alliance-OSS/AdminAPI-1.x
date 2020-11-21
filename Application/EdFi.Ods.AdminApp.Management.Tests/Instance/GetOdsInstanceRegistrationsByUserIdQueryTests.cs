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
using static EdFi.Ods.AdminApp.Management.Tests.Instance.InstanceTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.Instance
{
    [TestFixture]
    public class GetOdsInstanceRegistrationsByUserIdQueryTests: AdminAppDataTestBase
    {
        [Test]
        public void ShouldGetAdminAppOdsInstancesForUserSortedByName()
        {
            var users = SetupUsers(2).ToList();
            var testUser1 = users[0];
            var testUser2 = users[1];
            
            var testInstances = SetupOdsInstanceRegistrations(6).OrderBy(x => x.Name).ToList();

            var testInstancesAssignedToUser1 = testInstances.Take(3).ToList();
            var testInstancesAssignedToUser2 = testInstances.Skip(3).Take(3).ToList();

            SetupUserWithOdsInstanceRegistrations(testUser1.Id, testInstancesAssignedToUser1);
            SetupUserWithOdsInstanceRegistrations(testUser2.Id, testInstancesAssignedToUser2);

            Scoped<IGetOdsInstanceRegistrationsByUserIdQuery>(command =>
            {
                var resultsForUser1 = command.Execute(testUser1.Id).ToList();

                resultsForUser1.Count.ShouldBe(testInstancesAssignedToUser1.Count);
                resultsForUser1.Select(x => x.Name).ShouldBe(testInstancesAssignedToUser1.Select(x => x.Name));

                var resultsForUser2 = command.Execute(testUser2.Id).ToList();

                resultsForUser2.Count.ShouldBe(testInstancesAssignedToUser2.Count);
                resultsForUser2.Select(x => x.Name).ShouldBe(testInstancesAssignedToUser2.Select(x => x.Name));
            });
        }
    }
}
