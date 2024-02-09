// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
extern alias Compatability;

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using Moq;
using Shouldly;
using ClaimSet = Compatability::EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using Application = Compatability::EdFi.SecurityCompatiblity53.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class DeleteClaimSetCommandV53ServiceTests : SecurityData53TestBase
{

    [Test]
    public void ShouldDeleteClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testClaimSetToDelete = new ClaimSet
        { ClaimSetName = "TestClaimSet_Delete", Application = testApplication };
        Save(testClaimSetToDelete);
        SetupParentResourceClaimsWithChildren(testClaimSetToDelete, testApplication);

        var testClaimSetToPreserve = new ClaimSet
        { ClaimSetName = "TestClaimSet_Preserve", Application = testApplication };
        Save(testClaimSetToPreserve);
        var resourceClaimsForPreservedClaimSet = SetupParentResourceClaimsWithChildren(testClaimSetToPreserve, testApplication);

        var deleteModel = new Mock<IDeleteClaimSetModel>();
        deleteModel.Setup(x => x.Name).Returns(testClaimSetToDelete.ClaimSetName);
        deleteModel.Setup(x => x.Id).Returns(testClaimSetToDelete.ClaimSetId);

        using var securityContext = TestContext;
        var command = new DeleteClaimSetCommandV53Service(securityContext);
        command.Execute(deleteModel.Object);

        var deletedClaimSet = securityContext.ClaimSets.SingleOrDefault(x => x.ClaimSetId == testClaimSetToDelete.ClaimSetId);
        deletedClaimSet.ShouldBeNull();
        var deletedClaimSetResourceActions = securityContext.ClaimSetResourceClaims.Count(x => x.ClaimSet.ClaimSetId == testClaimSetToDelete.ClaimSetId);
        deletedClaimSetResourceActions.ShouldBe(0);

        var preservedClaimSet = securityContext.ClaimSets.Single(x => x.ClaimSetId == testClaimSetToPreserve.ClaimSetId);
        preservedClaimSet.ClaimSetName.ShouldBe(testClaimSetToPreserve.ClaimSetName);

        var results = ResourceClaimsForClaimSet(testClaimSetToPreserve.ClaimSetId);

        var testParentResourceClaimsForId =
            resourceClaimsForPreservedClaimSet.Where(x => x.ClaimSet.ClaimSetId == testClaimSetToPreserve.ClaimSetId && x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

        results.Count.ShouldBe(testParentResourceClaimsForId.Length);
        results.Select(x => x.Name).ShouldBe(testParentResourceClaimsForId.Select(x => x.ResourceName), true);
        results.Select(x => x.Id).ShouldBe(testParentResourceClaimsForId.Select(x => x.ResourceClaimId), true);
        results.All(x => x.Create).ShouldBe(true);

        foreach (var testParentResourceClaim in testParentResourceClaimsForId)
        {
            var testChildren = securityContext.ResourceClaims.Where(x =>
                x.ParentResourceClaimId == testParentResourceClaim.ResourceClaimId).ToList();
            var parentResult = results.First(x => x.Id == testParentResourceClaim.ResourceClaimId);
            parentResult.Children.Select(x => x.Name).ShouldBe(testChildren.Select(x => x.ResourceName), true);
            parentResult.Children.Select(x => x.Id).ShouldBe(testChildren.Select(x => x.ResourceClaimId), true);
            parentResult.Children.All(x => x.Create).ShouldBe(true);
        }
    }

    [Test]
    public void ShouldThrowExceptionOnEditSystemReservedClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var systemReservedClaimSet = new ClaimSet { ClaimSetName = "SIS Vendor", Application = testApplication };
        Save(systemReservedClaimSet);

        var deleteModel = new Mock<IDeleteClaimSetModel>();
        deleteModel.Setup(x => x.Name).Returns(systemReservedClaimSet.ClaimSetName);
        deleteModel.Setup(x => x.Id).Returns(systemReservedClaimSet.ClaimSetId);
        using var securityContext = TestContext;
        var exception = Assert.Throws<AdminApiException>(() =>
        {
            var command = new DeleteClaimSetCommandV53Service(securityContext);
            command.Execute(deleteModel.Object);
        });
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe($"Claim set({systemReservedClaimSet.ClaimSetName}) is system reserved.Can not be deleted.");
    }
}
