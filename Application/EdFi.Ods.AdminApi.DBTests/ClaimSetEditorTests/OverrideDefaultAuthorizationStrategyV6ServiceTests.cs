// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using Shouldly;
using System.Collections.Generic;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class OverrideDefaultAuthorizationStrategyV6ServiceTests : SecurityDataTestBase
{
    [Test]
    public void ShouldOverrideAuthorizationStrategiesForParentResourcesOnClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = "TestApplicationName"
        };

        Save(testApplication);

        var testClaimSet = new ClaimSet
        {
            ClaimSetName = "TestClaimSet",
            Application = testApplication
        };

        Save(testClaimSet);

        var appAuthorizationStrategies = SetupApplicationAuthorizationStrategies(testApplication).ToList();
        var parentRcNames = UniqueNameList("ParentRc", 2);

        var testResourceClaims = SetupParentResourceClaimsWithChildren(
            testClaimSet, testApplication, parentRcNames, UniqueNameList("Child", 1));

        SetupResourcesWithDefaultAuthorizationStrategies(
            appAuthorizationStrategies, testResourceClaims.ToList());

        var testResource1ToEdit = testResourceClaims.Select(x => x.ResourceClaim)
            .Single(x => x.ResourceName == parentRcNames.First());

        var testResource2ToNotEdit = testResourceClaims.Select(x => x.ResourceClaim)
            .Single(x => x.ResourceName == parentRcNames.Last());

        var overrideModel = new OverrideAuthorizationStrategyModel
        {
            ResourceClaimId = testResource1ToEdit.ResourceClaimId,
            ClaimSetId = testClaimSet.ClaimSetId,
            AuthorizationStrategyForCreate = new int[1] { appAuthorizationStrategies
                .Single(x => x.AuthorizationStrategyName == "TestAuthStrategy4").AuthorizationStrategyId },
            AuthorizationStrategyForRead = new int[0],
            AuthorizationStrategyForUpdate = new int[0],
            AuthorizationStrategyForDelete = new int[0],
            AuthorizationStrategyForReadChanges = new int[0],
        };

        List<ResourceClaim> resourceClaimsForClaimSet = null;

        using var securityContext = TestContext;
        var command = new OverrideDefaultAuthorizationStrategyV6Service(securityContext);
        command.Execute(overrideModel);
        var getResourcesByClaimSetIdQuery = new GetResourcesByClaimSetIdQuery(new GetResourcesByClaimSetIdQueryV6Service(securityContext, SecurityDataTestBase.Mapper()));
        resourceClaimsForClaimSet = getResourcesByClaimSetIdQuery.AllResources(testClaimSet.ClaimSetId).ToList();

        var resultResourceClaim1 =
            resourceClaimsForClaimSet.Single(x => x.Id == overrideModel.ResourceClaimId);

        resultResourceClaim1.AuthStrategyOverridesForCRUD[0].AuthorizationStrategies[0].AuthStrategyName.ShouldBe("TestAuthStrategy4");
        resultResourceClaim1.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
        resultResourceClaim1.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
        resultResourceClaim1.AuthStrategyOverridesForCRUD[3].ShouldBeNull();
        resultResourceClaim1.AuthStrategyOverridesForCRUD[4].ShouldBeNull();

        var resultResourceClaim2 =
            resourceClaimsForClaimSet.Single(x => x.Id == testResource2ToNotEdit.ResourceClaimId);

        resultResourceClaim2.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
        resultResourceClaim2.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
        resultResourceClaim2.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
        resultResourceClaim2.AuthStrategyOverridesForCRUD[3].ShouldBeNull();
        resultResourceClaim2.AuthStrategyOverridesForCRUD[4].ShouldBeNull();
    }
}
