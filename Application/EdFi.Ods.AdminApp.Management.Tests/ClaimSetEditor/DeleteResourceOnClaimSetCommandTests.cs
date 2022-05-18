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
using EdFi.Security.DataAccess.Contexts;
using Shouldly;
using static EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets.DeleteClaimSetResourceModel;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using ResourceClaim = EdFi.Security.DataAccess.Models.ResourceClaim;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class DeleteResourceOnClaimSetCommandTests : SecurityDataTestBase
    {
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

            var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication);

            var parentResourcesOnClaimSetOriginalCount =
                testResources.Count(x => x.ResourceClaim.ParentResourceClaim == null);

            var testResourceToDelete = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim1");
            
            var deleteResourceOnClaimSetModel = new DeleteClaimSetResourceModel
            {
                ClaimSetId = testClaimSet.ClaimSetId,
                ResourceClaimId = testResourceToDelete.ResourceClaimId,
                ClaimSetName = testClaimSet.ClaimSetName,
                ResourceName = testResourceToDelete.ResourceName
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new DeleteResourceOnClaimSetCommand(securityContext);
                command.Execute(deleteResourceOnClaimSetModel);
            });

            Transaction(securityContext =>
            {
                var resourceClaimsForClaimSet =
                    securityContext.ClaimSetResourceClaims.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaimId == null);
                resourceClaimsForClaimSet.Count().ShouldBe(parentResourcesOnClaimSetOriginalCount - 1);

                var resultResourceClaim = resourceClaimsForClaimSet.SingleOrDefault(x => x.ResourceClaim.ResourceClaimId == testResourceToDelete.ResourceClaimId);

                resultResourceClaim.ShouldBeNull();
            });
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

            var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication);

            var parentResourcesOnClaimSetOriginalCount =
                testResources.Count(x => x.ResourceClaim.ParentResourceClaim == null);

            var testParentResource = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim1");
            var childResourcesForParentOriginalCount = testResources.Count(x => x.ResourceClaim.ParentResourceClaimId == testParentResource.ResourceClaimId);
            var testChildResourceToDelete = testResources.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestChildResourceClaim1" && x.ParentResourceClaimId == testParentResource.ResourceClaimId);

            var deleteResourceOnClaimSetModel = new DeleteClaimSetResourceModel
            {
                ClaimSetId = testClaimSet.ClaimSetId,
                ResourceClaimId = testChildResourceToDelete.ResourceClaimId,
                ClaimSetName = testClaimSet.ClaimSetName,
                ResourceName = testChildResourceToDelete.ResourceName
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new DeleteResourceOnClaimSetCommand(securityContext);
                command.Execute(deleteResourceOnClaimSetModel);
            });

            var resourceClaimsForClaimSet =
                Scoped<IGetResourcesByClaimSetIdQuery, List<Management.ClaimSetEditor.ResourceClaim>>(
                    query => query.AllResources(testClaimSet.ClaimSetId).ToList());
            resourceClaimsForClaimSet.Count.ShouldBe(parentResourcesOnClaimSetOriginalCount);

            Transaction(securityContext =>
            {
                var resultChildResources =
                    securityContext.ClaimSetResourceClaims.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaimId == testParentResource.ResourceClaimId);
                resultChildResources.Count().ShouldBe(childResourcesForParentOriginalCount - 1);

                var resultResourceClaim = resultChildResources.SingleOrDefault(x => x.ResourceClaim.ResourceClaimId == testChildResourceToDelete.ResourceClaimId);

                resultResourceClaim.ShouldBeNull();
            });
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

            var testResources = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication);

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

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new DeleteClaimSetResourceModelValidator(securityContext);
                var validationResults = validator.Validate(deleteResourceOnClaimSetModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("This resource does not exist on the claimset.");
            });
        }
    }
}
