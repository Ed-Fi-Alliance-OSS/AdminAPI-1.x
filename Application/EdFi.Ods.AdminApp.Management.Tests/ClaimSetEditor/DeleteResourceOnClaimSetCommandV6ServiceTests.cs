// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using Shouldly;
using AutoMapper;
using static EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets.DeleteClaimSetResourceModel;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using ResourceClaim = EdFi.Security.DataAccess.Models.ResourceClaim;
using EdFi.Ods.AdminApp.Management.Api.Automapper;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class DeleteResourceOnClaimSetCommandV6ServiceTests : SecurityDataTestBase
    {
        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AdminManagementMappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Test]
        public void ShouldDeleteParentResourceOnClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(testClaimSet);

            var parentIds = UniqueNameList("ParentRc", 1);
            var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, parentIds, UniqueNameList("ChildRc", 1));

            var parentResourcesOnClaimSetOriginalCount =
                testResources.Count(x => x.ResourceClaim.ParentResourceClaim == null);

            var testResourceToDelete = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == parentIds.First());

            var deleteResourceOnClaimSetModel = new DeleteClaimSetResourceModel
            {
                ClaimSetId = testClaimSet.ClaimSetId,
                ResourceClaimId = testResourceToDelete.ResourceClaimId,
                ClaimSetName = testClaimSet.ClaimSetName,
                ResourceName = testResourceToDelete.ResourceName
            };

            using var securityContext = TestContext;
            var command = new DeleteResourceOnClaimSetCommandV6Service(securityContext);
            command.Execute(deleteResourceOnClaimSetModel);
            var resourceClaimsForClaimSet =
                securityContext.ClaimSetResourceClaimActions.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaimId == null);
            resourceClaimsForClaimSet.Count().ShouldBe(parentResourcesOnClaimSetOriginalCount - 1);

            var resultResourceClaim = resourceClaimsForClaimSet.SingleOrDefault(x => x.ResourceClaim.ResourceClaimId == testResourceToDelete.ResourceClaimId);

            resultResourceClaim.ShouldBeNull();
        }

        [Test]
        public void ShouldDeleteChildResourceOnClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(testClaimSet);

            var parentRcNames = UniqueNameList("ParentRc", 1);
            var childRcNames = UniqueNameList("ChildRc", 1);
            var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, parentRcNames, childRcNames);

            var childRcNameToTest = $"{childRcNames.First()}-{parentRcNames.First()}";
            var parentResourcesOnClaimSetOriginalCount =
                testResources.Count(x => x.ResourceClaim.ParentResourceClaim == null);

            var testParentResource = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == parentRcNames.First());
            var childResourcesForParentOriginalCount = testResources.Count(x => x.ResourceClaim.ParentResourceClaimId == testParentResource.ResourceClaimId);
            var testChildResourceToDelete = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == childRcNameToTest && x.ParentResourceClaimId == testParentResource.ResourceClaimId);

            var deleteResourceOnClaimSetModel = new DeleteClaimSetResourceModel
            {
                ClaimSetId = testClaimSet.ClaimSetId,
                ResourceClaimId = testChildResourceToDelete.ResourceClaimId,
                ClaimSetName = testClaimSet.ClaimSetName,
                ResourceName = testChildResourceToDelete.ResourceName
            };

            using var securityContext = TestContext;
            var command = new DeleteResourceOnClaimSetCommandV6Service(securityContext);
            command.Execute(deleteResourceOnClaimSetModel);

            List<Management.ClaimSetEditor.ResourceClaim> resourceClaimsForClaimSet = null;
            var getResourcesByClaimSetIdQuery = new GetResourcesByClaimSetIdQuery(new StubOdsSecurityModelVersionResolver.V6(),
                null, new GetResourcesByClaimSetIdQueryV6Service(securityContext, _mapper));
            resourceClaimsForClaimSet = getResourcesByClaimSetIdQuery.AllResources(testClaimSet.ClaimSetId).ToList();

            resourceClaimsForClaimSet.Count.ShouldBe(parentResourcesOnClaimSetOriginalCount);

            var resultChildResources =
                securityContext.ClaimSetResourceClaimActions.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaimId == testParentResource.ResourceClaimId);
            resultChildResources.Count().ShouldBe(childResourcesForParentOriginalCount - 1);

            var resultResourceClaim = resultChildResources.SingleOrDefault(x => x.ResourceClaim.ResourceClaimId == testChildResourceToDelete.ResourceClaimId);

            resultResourceClaim.ShouldBeNull();
        }

        [Test]
        public void ShouldNotDeleteIfResourceNotOnClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(testClaimSet);

            SetupParentResourceClaimsWithChildren(testClaimSet, testApplication, UniqueNameList("ParentRc", 3), UniqueNameList("ChildRc", 1));

            var resourceNotOnClaimSet = new ResourceClaim
            {
                Application = testApplication,
                ClaimName = "TestClaim99",
                DisplayName = "TestResource99",
                ParentResourceClaim = null,
                ResourceName = "TestResource99"
            };
            Save(resourceNotOnClaimSet);

            var deleteResourceOnClaimSetModel = new DeleteClaimSetResourceModel
            {
                ClaimSetId = testClaimSet.ClaimSetId,
                ResourceClaimId = resourceNotOnClaimSet.ResourceClaimId,
                ClaimSetName = testClaimSet.ClaimSetName,
                ResourceName = resourceNotOnClaimSet.ResourceName
            };

            using var securityContext = TestContext;
            var getResourcesByClaimSetIdQuery = new GetResourcesByClaimSetIdQuery(new StubOdsSecurityModelVersionResolver.V6(),
                    null, new GetResourcesByClaimSetIdQueryV6Service(securityContext, _mapper));
            var validator = new DeleteClaimSetResourceModelValidator(getResourcesByClaimSetIdQuery);
            var validationResults = validator.Validate(deleteResourceOnClaimSetModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("This resource does not exist on the claimset.");
        }
    }
}
