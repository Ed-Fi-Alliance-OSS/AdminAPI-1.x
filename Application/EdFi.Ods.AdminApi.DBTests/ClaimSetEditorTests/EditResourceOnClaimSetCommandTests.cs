// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using Shouldly;
using Moq;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using ResourceClaim = EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ResourceClaim;
using System.Collections.Generic;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class EditResourceOnClaimSetCommandTests : SecurityDataTestBase
{
    [Test]
    public void ShouldEditParentResourcesOnClaimSet()
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

        var testResource1ToEdit = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == parentRcNames.First());
        var testResource2ToNotEdit = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == parentRcNames.Last());

        var editedResource = new ResourceClaim
        {
            Id = testResource1ToEdit.ResourceClaimId,
            Name = testResource1ToEdit.ResourceName,
            Actions = new List<ResourceClaimAction>
            {
                new ResourceClaimAction{ Name = "Create", Enabled = false },
                new ResourceClaimAction{ Name = "Read", Enabled = false },
                new ResourceClaimAction{ Name = "Update", Enabled = true },
                new ResourceClaimAction{ Name = "Delete", Enabled = true }
            }     
        };

        var editResourceOnClaimSetModel = new Mock<IEditResourceOnClaimSetModel>();
        editResourceOnClaimSetModel.Setup(x => x.ClaimSetId).Returns(testClaimSet.ClaimSetId);
        editResourceOnClaimSetModel.Setup(x => x.ResourceClaim).Returns(editedResource);

        using var securityContext = TestContext;
        var command = new EditResourceOnClaimSetCommand(securityContext);
        command.Execute(editResourceOnClaimSetModel.Object);

        var resourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        var resultResourceClaim1 = resourceClaimsForClaimSet.Single(x => x.Id == editedResource.Id);

        resultResourceClaim1.Actions.ShouldNotBeNull();
        resultResourceClaim1.Actions.Count.ShouldBe(2);
        resultResourceClaim1.Actions.All(x =>
        editedResource.Actions.Any(y => x.Name.Equals(y.Name) && x.Enabled.Equals(y.Enabled))).ShouldBe(true);

        var resultResourceClaim2 = resourceClaimsForClaimSet.Single(x => x.Id == testResource2ToNotEdit.ResourceClaimId);
        resultResourceClaim2.Actions.ShouldNotBeNull();
        resultResourceClaim2.Actions.Count.ShouldBe(1);
        resultResourceClaim2.Actions.Any(x => x.Name.Equals("Create")).ShouldBe(true);
    }

    [Test]
    public void ShouldEditChildResourcesOnClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
        Save(testClaimSet);

        var parentRcNames = UniqueNameList("ParentRc", 1);
        var childRcNames = UniqueNameList("ChildRc", 2);
        var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, parentRcNames, childRcNames);

        var testParentResource = testResources.Single(x => x.ResourceClaim.ResourceName == parentRcNames.First());

        var test1ChildResourceClaim = $"{childRcNames.First()}-{parentRcNames.First()}";
        var test2ChildResourceClaim = $"{childRcNames.Last()}-{parentRcNames.First()}";

        using var securityContext = TestContext;
        var testChildResource1ToEdit = securityContext.ResourceClaims.Single(x => x.ResourceName == test1ChildResourceClaim && x.ParentResourceClaimId == testParentResource.ResourceClaim.ResourceClaimId);
        var testChildResource2NotToEdit = securityContext.ResourceClaims.Single(x => x.ResourceName == test2ChildResourceClaim && x.ParentResourceClaimId == testParentResource.ResourceClaim.ResourceClaimId);
        var editedResource = new ResourceClaim
        {
            Id = testChildResource1ToEdit.ResourceClaimId,
            Name = testChildResource1ToEdit.ResourceName,
            Actions = new List<ResourceClaimAction>
            {
                new ResourceClaimAction{ Name = "Create", Enabled = false },
                new ResourceClaimAction{ Name = "Read", Enabled = false },
                new ResourceClaimAction{ Name = "Update", Enabled = true },
                new ResourceClaimAction{ Name = "Delete", Enabled = true }
            }
        };

        var editResourceOnClaimSetModel = new Mock<IEditResourceOnClaimSetModel>();
        editResourceOnClaimSetModel.Setup(x => x.ClaimSetId).Returns(testClaimSet.ClaimSetId);
        editResourceOnClaimSetModel.Setup(x => x.ResourceClaim).Returns(editedResource);

        var command = new EditResourceOnClaimSetCommand(securityContext);
        command.Execute(editResourceOnClaimSetModel.Object);

        var resourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        var resultParentResourceClaim = resourceClaimsForClaimSet.Single(x => x.Id == testParentResource.ResourceClaim.ResourceClaimId);
        resultParentResourceClaim.Actions.ShouldNotBeNull();
        resultParentResourceClaim.Actions.Count.ShouldBe(1);
        resultParentResourceClaim.Actions.Any(x => x.Name.Equals("Create")).ShouldBe(true);

        var resultChildResourceClaim1 =
            resultParentResourceClaim.Children.Single(x => x.Id == editedResource.Id);

        resultChildResourceClaim1.Actions.ShouldNotBeNull();
        resultChildResourceClaim1.Actions.Count.ShouldBe(2);
        resultChildResourceClaim1.Actions.All(x =>
        editedResource.Actions.Any(y => x.Name.Equals(y.Name) && x.Enabled.Equals(y.Enabled))).ShouldBe(true);

        var resultChildResourceClaim2 =
            resultParentResourceClaim.Children.Single(x => x.Id == testChildResource2NotToEdit.ResourceClaimId);

        resultParentResourceClaim.Actions.ShouldNotBeNull();
        resultParentResourceClaim.Actions.Count.ShouldBe(1);
        resultParentResourceClaim.Actions.Any(x => x.Name.Equals("Create")).ShouldBe(true);
    }


    [Test]
    public void ShouldAddParentResourceToClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
        Save(testClaimSet);

        var parentRcNames = UniqueNameList("Parent", 1);
        var testResources = SetupResourceClaims(testApplication, parentRcNames, UniqueNameList("child", 1));
        var testResourceToAdd = testResources.Single(x => x.ResourceName == parentRcNames.First());
        var resourceToAdd = new ResourceClaim()
        {
            Id = testResourceToAdd.ResourceClaimId,
            Name = testResourceToAdd.ResourceName,
            Actions = new List<ResourceClaimAction>
            {
                new ResourceClaimAction{ Name = "Create", Enabled = true },
                new ResourceClaimAction{ Name = "Read", Enabled = false },
                new ResourceClaimAction{ Name = "Update", Enabled = true },
                new ResourceClaimAction{ Name = "Delete", Enabled = false }
            }         
        };
        var existingResources = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        var editResourceOnClaimSetModel = new EditResourceOnClaimSetModel
        {
            ClaimSetId = testClaimSet.ClaimSetId,
            ResourceClaim = resourceToAdd
        };

        using var securityContext = TestContext;
        var command = new EditResourceOnClaimSetCommand(securityContext);
        command.Execute(editResourceOnClaimSetModel);

        var resourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        var resultResourceClaim1 = resourceClaimsForClaimSet.Single(x => x.Name == testResourceToAdd.ResourceName);

        resultResourceClaim1.Actions.ShouldNotBeNull();
        resultResourceClaim1.Actions.Count.ShouldBe(2);
        resultResourceClaim1.Actions.All(x =>
        resourceToAdd.Actions.Any(y => x.Name.Equals(y.Name) && x.Enabled.Equals(y.Enabled))).ShouldBe(true);
    }

    [Test]
    public void ShouldAddChildResourcesToClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
        Save(testClaimSet);

        var parentRcNames = UniqueNameList("Parent", 1);
        var childRcNames = UniqueNameList("Child", 1);
        var testResources = SetupResourceClaims(testApplication, parentRcNames, childRcNames);

        var testParentResource1 = testResources.Single(x => x.ResourceName == parentRcNames.First());
        var childRcToTest = $"{childRcNames.First()}-{parentRcNames.First()}";

        using var securityContext = TestContext;
        var testChildResource1ToAdd = securityContext.ResourceClaims.Single(x => x.ResourceName == childRcToTest && x.ParentResourceClaimId == testParentResource1.ResourceClaimId);
        var resourceToAdd = new ResourceClaim()
        {
            Id = testChildResource1ToAdd.ResourceClaimId,
            Name = testChildResource1ToAdd.ResourceName,
            Actions = new List<ResourceClaimAction>
            {
                new ResourceClaimAction{ Name = "Create", Enabled = true },
                new ResourceClaimAction{ Name = "Read", Enabled = false },
                new ResourceClaimAction{ Name = "Update", Enabled = true },
                new ResourceClaimAction{ Name = "Delete", Enabled = false }
            }
        };
        var existingResources = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        var editResourceOnClaimSetModel = new EditResourceOnClaimSetModel
        {
            ClaimSetId = testClaimSet.ClaimSetId,
            ResourceClaim = resourceToAdd
        };

        var command = new EditResourceOnClaimSetCommand(securityContext);
        command.Execute(editResourceOnClaimSetModel);

        var resourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);

        var resultChildResourceClaim1 =
            resourceClaimsForClaimSet.Single(x => x.Name == testChildResource1ToAdd.ResourceName);

        resultChildResourceClaim1.Actions.ShouldNotBeNull();
        resultChildResourceClaim1.Actions.Count.ShouldBe(2);
        resultChildResourceClaim1.Actions.All(x =>
        resourceToAdd.Actions.Any(y => x.Name.Equals(y.Name) && x.Enabled.Equals(y.Enabled))).ShouldBe(true);
    }
}
