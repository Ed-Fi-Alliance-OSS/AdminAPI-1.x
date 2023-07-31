// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Features.ClaimSets;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class DeleteResourceClaimOnClaimSetCommandTests : SecurityDataTestBase
{
    [Test]
    public void ShouldDeleteResourceClaimOnClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
        Save(testClaimSet);

        var parentRcNames = UniqueNameList("ParentRc", 2);
        var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, parentRcNames, UniqueNameList("ChildRc", 1));

        var filter = new FilterResourceClaimOnClaimSet()
        {
            ClaimSetId = testClaimSet.ClaimSetId,
            ResourceClaimId = testResources.First().ResourceClaimId
        };

        using var securityContext = TestContext;
        var command = new DeleteResouceClaimOnClaimSetCommand(securityContext);
        command.Execute(filter);

        var resourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        resourceClaimsForClaimSet.Count.ShouldBeLessThan(testResources.Count);
    }
}
