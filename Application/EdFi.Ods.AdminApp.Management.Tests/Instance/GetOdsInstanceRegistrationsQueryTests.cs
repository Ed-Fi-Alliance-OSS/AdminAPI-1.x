// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.Instances;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Instance.InstanceTestSetup;

namespace EdFi.Ods.AdminApp.Management.Tests.Instance
{
    [TestFixture]
    public class GetOdsInstanceRegistrationsQueryTests: AdminAppDataTestBase
    {
        [TestCase(1)]
        [TestCase(5)]
        public void ShouldGetAdminAppOdsInstancesSortedByName(int instanceCount)
        {
            ResetOdsInstanceRegistrations();

            var testInstances = SetupOdsInstanceRegistrations(instanceCount).OrderBy(x => x.Name).ToList();

            var results = Transaction(database =>
            {
                var command = new GetOdsInstanceRegistrationsQuery(database);

                return command.Execute().ToList();
            });

            results.Count.ShouldBe(testInstances.Count);
            results.Select(x => x.Name).ShouldBe(testInstances.Select(x => x.Name));
        }

        [TestCase(1)]
        [TestCase(5)]
        public void ShouldGetAdminAppOdsInstanceCount(int instanceCount)
        {
            ResetOdsInstanceRegistrations();

            SetupOdsInstanceRegistrations(instanceCount).OrderBy(x => x.Name).ToList();

            var result = Transaction(database =>
            {
                var command = new GetOdsInstanceRegistrationsQuery(database);

                return command.ExecuteCount();
            });

            result.ShouldBe(instanceCount);
        }

        [Test]
        public void ShouldGetAdminAppOdsInstanceById()
        {
            var testInstance = SetupOdsInstanceRegistrations(1).Single();

            var result = Transaction(database =>
            {
                var command = new GetOdsInstanceRegistrationsQuery(database);

                return command.Execute(testInstance.Id);
            });

            result.Id.ShouldBe(testInstance.Id);
            result.Name.ShouldBe(testInstance.Name);
            result.Description.ShouldBe(testInstance.Description);
            result.DatabaseName.ShouldBe(testInstance.DatabaseName);
        }

        [Test]
        public void ShouldGetAdminAppOdsInstanceByName()
        {
            var testInstance = SetupOdsInstanceRegistrations(1).Single();

            var result = Transaction(database =>
            {
                var command = new GetOdsInstanceRegistrationsQuery(database);

                return command.Execute(testInstance.Name);
            });

            result.Id.ShouldBe(testInstance.Id);
            result.Name.ShouldBe(testInstance.Name);
            result.Description.ShouldBe(testInstance.Description);
            result.DatabaseName.ShouldBe(testInstance.DatabaseName);
        }
    }
}
