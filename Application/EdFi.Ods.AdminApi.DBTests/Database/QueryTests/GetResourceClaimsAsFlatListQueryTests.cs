// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using ResourceClaim = EdFi.Security.DataAccess.Models.ResourceClaim;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetResourceClaimsAsFlatListQueryTests : SecurityDataTestBase
{
    [Test]
    public void ShouldGetResourceClaimsAsFlatList()
    {
        var testResourceClaims = SetupResourceClaims();

        Infrastructure.ClaimSetEditor.ResourceClaim[] results = null;
        using var securityContext = TestContext;
        var query = new GetResourceClaimsAsFlatListQuery(securityContext);
        results = query.Execute().ToArray();
        results.Length.ShouldBe(testResourceClaims.Count);
        results.Select(x => x.Name).ShouldBe(testResourceClaims.Select(x => x.ResourceName), true);
        results.Select(x => x.Id).ShouldBe(testResourceClaims.Select(x => x.ResourceClaimId), true);
        results.All(x => x.Actions == null).ShouldBe(true);
        results.All(x => x.ParentId.Equals(0)).ShouldBe(true);
        results.All(x => x.ParentName == null).ShouldBe(true);
        results.All(x => x.Children.Count == 0).ShouldBe(true);
    }

    [Test]
    public void ShouldGetAlphabeticallySortedFlatListForResourceClaims()
    {
        var testClaimSet = new ClaimSet
        { ClaimSetName = "TestClaimSet_test" };
        Save(testClaimSet);
        var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSet, UniqueNameList("ParentRc", 3), UniqueNameList("ChildRc", 1)).ToList();
        var parentResourceNames = testResourceClaims.Where(x => x.ResourceClaim?.ParentResourceClaim == null)
            .OrderBy(x => x.ResourceClaim.ResourceName).Select(x => x.ResourceClaim?.ResourceName).ToList();
        var childResourceNames = testResourceClaims.Where(x => x.ResourceClaim?.ParentResourceClaim != null)
            .OrderBy(x => x.ResourceClaim?.ResourceName).Select(x => x.ResourceClaim?.ResourceName).ToList();

        List<Infrastructure.ClaimSetEditor.ResourceClaim> results = null;
        using var securityContext = TestContext;
        var query = new GetResourceClaimsAsFlatListQuery(securityContext);
        results = query.Execute().ToList();
        results.Count.ShouldBe(testResourceClaims.Count);
        results.Where(x => x.ParentId == 0).Select(x => x.Name).ToList().ShouldBe(parentResourceNames);
        results.Where(x => x.ParentId != 0).Select(x => x.Name).ToList().ShouldBe(childResourceNames);
    }

    private IReadOnlyCollection<ResourceClaim> SetupResourceClaims(int resourceClaimCount = 5)
    {
        var resourceClaims = new List<ResourceClaim>();
        foreach (var index in Enumerable.Range(1, resourceClaimCount))
        {
            var resourceClaim = new ResourceClaim
            {
                ClaimName = $"TestResourceClaim{index:N}",
                ResourceName = $"TestResourceClaim{index:N}",
            };
            resourceClaims.Add(resourceClaim);
        }

        Save(resourceClaims.Cast<object>().ToArray());

        return resourceClaims;
    }

}
