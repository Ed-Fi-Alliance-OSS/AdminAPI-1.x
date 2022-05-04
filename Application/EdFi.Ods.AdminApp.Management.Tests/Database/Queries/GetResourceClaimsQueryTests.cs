// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Security.DataAccess.Contexts;
using NUnit.Framework;
using Shouldly;
using Application = EdFi.Security.DataAccess.Models.Application;
using ResourceClaim = EdFi.Security.DataAccess.Models.ResourceClaim;
using static EdFi.Ods.AdminApp.Web.Infrastructure.ResourceClaimSelectListBuilder;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

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
            Scoped<ISecurityContext>(securityContext =>
            {
                var query = new GetResourceClaimsQuery(securityContext);

                results = query.Execute().ToArray();
            });

            Scoped<ISecurityContext>(securityContext =>
            {
                results.Length.ShouldBe(testResourceClaims.Count);
                results.Select(x => x.Name).ShouldBe(testResourceClaims.Select(x => x.ResourceName), true);
                results.Select(x => x.Id).ShouldBe(testResourceClaims.Select(x => x.ResourceClaimId), true);
                results.All(x => x.Create == false).ShouldBe(true);
                results.All(x => x.Delete == false).ShouldBe(true);
                results.All(x => x.Update == false).ShouldBe(true);
                results.All(x => x.Read == false).ShouldBe(true);
                results.All(x => x.ParentId.Equals(0)).ShouldBe(true);
                results.All(x => x.Children.Count == 0).ShouldBe(true);
            });
        }

        [Test]
        public void ShouldGetAlphabeticallySortedSelectListForResourceClaims()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplicationName"
            };

            Save(testApplication);

            var testResourceClaims = SetupParentResourceClaimsWithChildren(testApplication, UniqueNameList("Parent", 1), UniqueNameList("Child", 1)).ToList();
            var parentResourceNames = testResourceClaims.Where(x => x.ParentResourceClaim == null)
                .OrderBy(x => x.ResourceName).Select(x => x.ResourceName).ToList();
            var childResourceNames = testResourceClaims.Where(x => x.ParentResourceClaim != null)
                .OrderBy(x => x.ResourceName).Select(x => x.ResourceName).ToList();

            List<SelectListItem> results = null;
            Scoped<ISecurityContext>(securityContext =>
            {
                var query = new GetResourceClaimsQuery(securityContext);

                var allResourceClaims = query.Execute().ToList();

                results = GetSelectListForResourceClaims(allResourceClaims);

                // Removing "Please select a value" SelectListItem from the results
                results.RemoveAt(0);
            });

            Scoped<ISecurityContext>(securityContext =>
            {
                results.Count.ShouldBe(testResourceClaims.Count);
                results.Where(x => x.Group.Name == "Groups").Select(x => x.Text).ToList().ShouldBe(parentResourceNames);
                results.Where(x => x.Group.Name == "Resources").Select(x => x.Text).ToList().ShouldBe(childResourceNames);
            });
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

        private IReadOnlyCollection<ResourceClaim> SetupParentResourceClaimsWithChildren(Application testApplication, IList<string> parentRcNames, IList<string> childRcNames)
        {
            var parentResourceClaims = parentRcNames.Select(parentRcName => new ResourceClaim
            {
                ClaimName = parentRcName,
                DisplayName = parentRcName,
                ResourceName = parentRcName,
                Application = testApplication
            }).ToList();

            var childResourceClaims = parentResourceClaims.SelectMany(x => childRcNames
                .Select(childIndex => new ResourceClaim
                {
                    ClaimName = $"{childIndex}-{x.ClaimName}",
                    DisplayName = $"{childIndex}-{x.ClaimName}",
                    ResourceName = $"{childIndex}-{x.ClaimName}",
                    Application = testApplication,
                    ParentResourceClaim = x
                })).ToList();

            Save(childResourceClaims.Cast<object>().ToArray());
            parentResourceClaims.AddRange(childResourceClaims);
            return parentResourceClaims;
        }
    }
}
