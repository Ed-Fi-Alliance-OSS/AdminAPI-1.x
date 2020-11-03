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
using EdFi.Ods.AdminApp.Web;
using Application = EdFi.Security.DataAccess.Models.Application;
using VendorApplication = EdFi.Admin.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class GetApplicationsByClaimSetIdQueryTests : SecurityDataTestBase
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void ShouldGetApplicationsByClaimSetId(int applicationCount)
        {
            var testClaimSets = SetupApplicationWithClaimSets();

            #if NET48
                using (var usersDbContext = new SqlServerUsersContext())
            #else
                using (var usersDbContext = new SqlServerUsersContext(Startup.ConfigurationConnectionStrings.Admin))
            #endif
            {
                Transaction(usersDbContext, usersContext =>
                {
                    SetupApplications(testClaimSets, usersContext, applicationCount);

                    var query = new GetApplicationsByClaimSetIdQuery(TestContext, usersContext);

                    foreach (var testClaimSet in testClaimSets)
                    {
                        var results = query.Execute(testClaimSet.ClaimSetId).ToArray();

                        var testApplications =
                            usersContext.Applications.Where(x => x.ClaimSetName == testClaimSet.ClaimSetName).Select(x => new Application
                            {
                                ApplicationName = x.ApplicationName,
                                ApplicationId = x.ApplicationId
                            }).ToArray();
                        results.Length.ShouldBe(testApplications.Length);
                        results.Select(x => x.Name).ShouldBe(testApplications.Select(x => x.ApplicationName), true);
                    }
                });
            }
        }

        private IReadOnlyCollection<ClaimSet> SetupApplicationWithClaimSets(
            string applicationName = "TestApplicationName", int claimSetCount = 5)
        {
            var testApplication = new Application
            {
                ApplicationName = applicationName
            };

            Save(testApplication);

            var testClaimSetNames = Enumerable.Range(1, claimSetCount)
                .Select((x, index) => $"TestClaimSetName{index:N}")
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

        private static void SetupApplications(IEnumerable<ClaimSet> testClaimSets, UsersContext usersContext, int applicationCount = 5)
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
        }
    }
}
