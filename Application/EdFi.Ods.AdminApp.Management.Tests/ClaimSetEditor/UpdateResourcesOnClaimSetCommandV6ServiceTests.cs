// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Shouldly;
using System.Collections.Generic;
using Moq;

using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class UpdateResourcesOnClaimSetCommandV6ServiceTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldUpdateResourcesOnClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(testClaimSet);

            var parentRcNames = UniqueNameList("ParentRc", 2);
            var childName = "ChildRc098";
            var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, parentRcNames,
                new List<string>{ childName });

            var testParentResource = testResources.Single(x => x.ResourceClaim.ResourceName == parentRcNames.First());
            var secondTestParentResource = testResources.Single(x => x.ResourceClaim.ResourceName == parentRcNames.Last());

            var firstParentChildName = $"{childName}-{parentRcNames.First()}";
            using var securityContext = TestContext;
            var testChildResource1ToEdit = securityContext.ResourceClaims.Single(x => x.ResourceName == firstParentChildName && x.ParentResourceClaimId == testParentResource.ResourceClaim.ResourceClaimId);

            var addedResourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);
            addedResourceClaimsForClaimSet.Count().ShouldBe(2);
            var secondParentResourceClaim = addedResourceClaimsForClaimSet.Single(x => x.Id == secondTestParentResource.ResourceClaim.ResourceClaimId);
            secondParentResourceClaim.ShouldNotBeNull();

            var updatedParent1 = new ResourceClaim
            {
                Id = testParentResource.ResourceClaim.ResourceClaimId,
                Name = testParentResource.ResourceClaim.ResourceName,
                Create = false,
                Read = false,
                Update = true,
                Delete = true,
                Children = new List<ResourceClaim> {new ResourceClaim
                    {
                        Id = testChildResource1ToEdit.ResourceClaimId,
                        Name = testChildResource1ToEdit.ResourceName,
                        Create = false,
                        Read = false,
                        Update = true,
                        Delete = true
                    } }
            };

            var updatedResourceClaims = new List<ResourceClaim>
            {
                updatedParent1
            };

            var updateResourcesOnClaimSetModel = new Mock<IUpdateResourcesOnClaimSetModel>();
            updateResourcesOnClaimSetModel.Setup(x => x.ClaimSetId).Returns(testClaimSet.ClaimSetId);
            updateResourcesOnClaimSetModel.Setup(x => x.ResourceClaims).Returns(updatedResourceClaims);

            using var securityContext6 = CreateDbContext();
            var addOrEditResourcesOnClaimSetCommand = new AddOrEditResourcesOnClaimSetCommand(
                new EditResourceOnClaimSetCommand(new StubOdsSecurityModelVersionResolver.V6(),
                null, new EditResourceOnClaimSetCommandV6Service(securityContext6)),
                new Management.Database.Queries.GetResourceClaimsQuery(securityContext6),
                new OverrideDefaultAuthorizationStrategyCommand(
                    new StubOdsSecurityModelVersionResolver.V6(), null,
                    new OverrideDefaultAuthorizationStrategyV6Service(securityContext6)));

            var command = new UpdateResourcesOnClaimSetCommandV6Service(securityContext6, addOrEditResourcesOnClaimSetCommand);
            command.Execute(updateResourcesOnClaimSetModel.Object);

            var resourceClaimsForClaimSet = ResourceClaimsForClaimSet(testClaimSet.ClaimSetId);
            resourceClaimsForClaimSet.Count.ShouldBe(1);
            resourceClaimsForClaimSet.First().Name.ShouldBe(testParentResource.ResourceClaim.ResourceName);
            resourceClaimsForClaimSet.First().Children.Count.ShouldBe(1);
            resourceClaimsForClaimSet.First().Children.First().Name.ShouldBe(testChildResource1ToEdit.ResourceName);
        }
    }
}
