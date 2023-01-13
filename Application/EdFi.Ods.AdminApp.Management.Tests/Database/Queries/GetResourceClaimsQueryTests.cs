// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Web.Infrastructure.ResourceClaimSelectListBuilder;

using Application = EdFi.Security.DataAccess.Models.Application;
using ResourceClaim = EdFi.Security.DataAccess.Models.ResourceClaim;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetResourceClaimsQueryTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldGetResourceClaims()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);

            var testResourceClaims = SetupResourceClaims(testApplication);

            Management.ClaimSetEditor.ResourceClaim[] results = null;
            using var securityContext = TestContext;
            var query = new GetResourceClaimsQuery(securityContext);
            results = query.Execute().ToArray();
            results.Length.ShouldBe(testResourceClaims.Count);
            results.Select(x => x.Name).ShouldBe(testResourceClaims.Select(x => x.ResourceName), true);
            results.Select(x => x.Id).ShouldBe(testResourceClaims.Select(x => x.ResourceClaimId), true);
            results.All(x => x.Create == false).ShouldBe(true);
            results.All(x => x.Delete == false).ShouldBe(true);
            results.All(x => x.Update == false).ShouldBe(true);
            results.All(x => x.Read == false).ShouldBe(true);
            results.All(x => x.ParentId.Equals(0)).ShouldBe(true);
            results.All(x => x.ParentName == null).ShouldBe(true);
            results.All(x => x.Children.Count == 0).ShouldBe(true);
        }

        [Test]
        public void ShouldGetAlphabeticallySortedSelectListForResourceClaims()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);

            var testClaimSet = new ClaimSet
            { ClaimSetName = "TestClaimSet_test", Application = testApplication };
            Save(testClaimSet);

            var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, UniqueNameList("ParentRc", 3), UniqueNameList("ChildRc", 1)).ToList();
            var parentResourceNames = testResourceClaims.Where(x => x.ResourceClaim?.ParentResourceClaim == null)
                .OrderBy(x => x.ResourceClaim.ResourceName).Select(x => x.ResourceClaim?.ResourceName).ToList();
            var childResourceNames = testResourceClaims.Where(x => x.ResourceClaim?.ParentResourceClaim != null)
                .OrderBy(x => x.ResourceClaim?.ResourceName).Select(x => x.ResourceClaim?.ResourceName).ToList();

            List<SelectListItem> results = null;
            using var securityContext = TestContext;
            var query = new GetResourceClaimsQuery(securityContext);

            var allResourceClaims = query.Execute().ToList();

            results = GetSelectListForResourceClaims(allResourceClaims);

            // Removing "Please select a value" SelectListItem from the results
            results.RemoveAt(0);
            results.Count.ShouldBe(testResourceClaims.Count);
            results.Where(x => x.Group.Name == "Groups").Select(x => x.Text).ToList().ShouldBe(parentResourceNames);
            results.Where(x => x.Group.Name == "Resources").Select(x => x.Text).ToList().ShouldBe(childResourceNames);
        }

        private IReadOnlyCollection<ResourceClaim> SetupResourceClaims(Application testApplication, int resourceClaimCount = 5)
        {
            var resourceClaims = new List<ResourceClaim>();
            foreach (var index in Enumerable.Range(1, resourceClaimCount))
            {
                var resourceClaim = new ResourceClaim
                {
                    ClaimName = $"TestResourceClaim{index:N}",
                    DisplayName = $"TestResourceClaim{index:N}",
                    ResourceName = $"TestResourceClaim{index:N}",
                    Application = testApplication
                };
                resourceClaims.Add(resourceClaim);
            }

            Save(resourceClaims.Cast<object>().ToArray());

            return resourceClaims;
        }
    }
}
