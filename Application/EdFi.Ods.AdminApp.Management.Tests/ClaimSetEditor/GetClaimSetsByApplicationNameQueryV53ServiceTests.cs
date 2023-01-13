// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using NUnit.Framework;
using Shouldly;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;

using static EdFi.Ods.AdminApp.Management.Tests.Testing;

using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;
using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using VendorApplication = EdFi.Admin.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class GetClaimSetsByApplicationNameQueryV53ServiceTests : SecurityData53TestBase
    {
        [Test]
        public void ShouldExcludeInternalAdminAppClaimSet()
        {
            var testClaimSets = SetupApplicationClaimSets();

            var testApplication = testClaimSets.First().Application;

            Save(new ClaimSet
            {
                ClaimSetName = CloudOdsAdminApp.InternalAdminAppClaimSet,
                Application = testApplication
            });

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var query = new GetClaimSetsByApplicationNameQueryV53Service(securityContext, usersContext);
                var results = query.Execute(testApplication.ApplicationName).ToArray();

                results.ShouldNotContain(x => x.Name == CloudOdsAdminApp.InternalAdminAppClaimSet);
            });
        }

        [Test]
        public void ShouldExcludeSystemReservedClaimSets()
        {
            var testClaimSets = SetupApplicationClaimSets();

            var testApplication = testClaimSets.First().Application;

            Save(new ClaimSet
            {
                ClaimSetName = "Bootstrap Descriptors and EdOrgs",
                Application = testApplication
            });

            Save(new ClaimSet
            {
                ClaimSetName = CloudOdsAdminApp.InternalAdminAppClaimSet,
                Application = testApplication
            });

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var query = new GetClaimSetsByApplicationNameQueryV53Service(securityContext, usersContext);
                var results = query.Execute(testApplication.ApplicationName).ToArray();

                results.ShouldNotContain(x => x.Name == CloudOdsAdminApp.InternalAdminAppClaimSet);
                results.ShouldNotContain(x => x.Name == "Bootstrap Descriptors and EdOrgs");
            });
        }

        [Test]
        public void ShouldGetAllTheClaimSetsFromAnApplication()
        {
            var testClaimSets = SetupApplicationClaimSets();

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var query = new GetClaimSetsByApplicationNameQueryV53Service(securityContext, usersContext);
                var results = query.Execute(testClaimSets.First().Application.ApplicationName).ToArray();

                results.Length.ShouldBe(testClaimSets.Count);
                results.Select(x => x.Name).ShouldBe(testClaimSets.Select(x => x.ClaimSetName), true);
            });
        }

        [Test]
        public void ShouldNotGetClaimSetsFromOtherApplications()
        {
            SetupApplicationClaimSets("OtherApplication");
            SetupApplicationClaimSets("YetAnotherApplication");

            var testClaimSets = SetupApplicationClaimSets();

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var query = new GetClaimSetsByApplicationNameQueryV53Service(securityContext, usersContext);
                var results = query.Execute(testClaimSets.First().Application.ApplicationName).ToArray();

                results.Length.ShouldBe(testClaimSets.Count);
                results.Select(x => x.Name).ShouldBe(testClaimSets.Select(x => x.ClaimSetName), true);
            });
        }

        [Test]
        public void ShouldGetTheClaimSetsWithTheExpectedIsEditableValue()
        {
            var testClaimSets = SetupApplicationClaimSets();
            var testApplication = testClaimSets.First().Application;

            Save(new ClaimSet
            {
                ClaimSetName = "SIS Vendor",
                Application = testApplication
            });

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var query = new GetClaimSetsByApplicationNameQueryV53Service(securityContext, usersContext);
                var results = query.Execute(testApplication.ApplicationName).ToArray();

                results.Count(x => x.IsEditable).ShouldBe(testClaimSets.Count);
                results.Single(x => !x.IsEditable).Name.ShouldBe("SIS Vendor");
                results.Count(x => !x.IsEditable).ShouldBe(1);
            });
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void ShouldGetTheCorrectApplicationCount(int applicationCount)
        {
            SetupApplicationClaimSets("OtherApplication");

            var testClaimSets = SetupApplicationClaimSets();

            Scoped<IUsersContext>(usersContext =>
            {
                foreach (var claimSet in testClaimSets)
                {
                    foreach (var _ in Enumerable.Range(1, applicationCount))
                    {
                        usersContext.Applications.Add(new VendorApplication
                        {
                            ApplicationName = $"TestAppVendorName{Guid.NewGuid():N}",
                            ClaimSetName = claimSet.ClaimSetName,
                            OperationalContextUri = OperationalContext.DefaultOperationalContextUri
                        });
                    }
                }

                usersContext.SaveChanges();
            });

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var query = new GetClaimSetsByApplicationNameQueryV53Service(securityContext, usersContext);
                var results = query.Execute(testClaimSets.First().Application.ApplicationName).ToArray();

                results.Length.ShouldBe(testClaimSets.Count);
                results.Select(x => x.Name).ShouldBe(testClaimSets.Select(x => x.ClaimSetName), true);
                results.Select(x => x.ApplicationsCount).Distinct().Single().ShouldBe(applicationCount);
            });
        }

        private IReadOnlyCollection<ClaimSet> SetupApplicationClaimSets(
            string applicationName = "TestApplicationName", int claimSetCount = 5)
        {
            var testApplication = new Application
            {
                ApplicationName = applicationName
            };

            Save(testApplication);

            var testClaimSetNames = Enumerable.Range(1, claimSetCount)
                .Select(x => $"TestClaimSetName{Guid.NewGuid():N}")
                .ToArray();

            var testClaimSets = testClaimSetNames
                .Select(x => new ClaimSet
                {
                    ClaimSetName = x,
                    Application = testApplication
                })
                .ToArray();

            Save(testClaimSets.Cast<object>().ToArray());

            return testClaimSets;
        }
    }
}
