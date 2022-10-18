// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using Shouldly;
using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using System.Collections.Generic;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class OverrideDefaultAuthorizationStrategyCommandTests : SecurityDataTestBase
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
            var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication);
            SetupResourcesWithDefaultAuthorizationStrategies(appAuthorizationStrategies, testResourceClaims.ToList());

            var testResource1ToEdit = testResourceClaims.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim1");
            var testResource2ToNotEdit = testResourceClaims.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim2");
            
            var overrideModel = new OverrideDefaultAuthorizationStrategyModel
            {
                ResourceClaimId = testResource1ToEdit.ResourceClaimId,
                ClaimSetId = testClaimSet.ClaimSetId,
                AuthorizationStrategyForCreate = appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy4").AuthorizationStrategyId,
                AuthorizationStrategyForRead = 0,
                AuthorizationStrategyForUpdate = 0,
                AuthorizationStrategyForDelete = 0
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new OverrideDefaultAuthorizationStrategyCommand(securityContext);
                command.Execute(overrideModel);
            });

            var resourceClaimsForClaimSet =
                Scoped<IGetResourcesByClaimSetIdQuery, List<Management.ClaimSetEditor.ResourceClaim>>(
                    query => query.AllResources(testClaimSet.ClaimSetId).ToList());

            var resultResourceClaim1 = resourceClaimsForClaimSet.Single(x => x.Id == overrideModel.ResourceClaimId);
            
            resultResourceClaim1.AuthStrategyOverridesForCRUD[0].AuthStrategyName.ShouldBe("TestAuthStrategy4");
            resultResourceClaim1.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaim1.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaim1.AuthStrategyOverridesForCRUD[3].ShouldBeNull();

            var resultResourceClaim2 = resourceClaimsForClaimSet.Single(x => x.Id == testResource2ToNotEdit.ResourceClaimId);
            
            resultResourceClaim2.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
            resultResourceClaim2.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaim2.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaim2.AuthStrategyOverridesForCRUD[3].ShouldBeNull();
        }

        [Test]
        public void ShouldOverrideAuthorizationStrategiesForChildResourcesOnClaimSet()
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
            var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication);
            SetupResourcesWithDefaultAuthorizationStrategies(appAuthorizationStrategies, testResourceClaims.ToList());

            var testParentResource = testResourceClaims.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim1");
            var testChildResourceToEdit = testResourceClaims.Select(x => x.ResourceClaim).Single(x =>
                x.ResourceName == "TestChildResourceClaim1" &&
                x.ParentResourceClaimId == testParentResource.ResourceClaimId);
            var testChildResourceNotToEdit = testResourceClaims.Select(x => x.ResourceClaim).Single(x =>
                x.ResourceName == "TestChildResourceClaim2" &&
                x.ParentResourceClaimId == testParentResource.ResourceClaimId);

            var overrideModel = new OverrideDefaultAuthorizationStrategyModel
            {
                ResourceClaimId = testChildResourceToEdit.ResourceClaimId,
                ClaimSetId = testClaimSet.ClaimSetId,
                AuthorizationStrategyForCreate = appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy4").AuthorizationStrategyId,
                AuthorizationStrategyForRead = 0,
                AuthorizationStrategyForUpdate = 0,
                AuthorizationStrategyForDelete = 0
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new OverrideDefaultAuthorizationStrategyCommand(securityContext);
                command.Execute(overrideModel);
            });

            var resourceClaimsForClaimSet =
                Scoped<IGetResourcesByClaimSetIdQuery, List<Management.ClaimSetEditor.ResourceClaim>>(
                    query => query.AllResources(testClaimSet.ClaimSetId).ToList());

            var resultParentResource = resourceClaimsForClaimSet.Single(x => x.Id == testParentResource.ResourceClaimId);
            var resultChildResource1 =
                resultParentResource.Children.Single(x => x.Id == testChildResourceToEdit.ResourceClaimId);

            resultChildResource1.AuthStrategyOverridesForCRUD[0].AuthStrategyName.ShouldBe("TestAuthStrategy4");
            resultChildResource1.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultChildResource1.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultChildResource1.AuthStrategyOverridesForCRUD[3].ShouldBeNull();

            var resultResourceClaim2 = resultParentResource.Children.Single(x => x.Id == testChildResourceNotToEdit.ResourceClaimId);

            resultResourceClaim2.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
            resultResourceClaim2.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaim2.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaim2.AuthStrategyOverridesForCRUD[3].ShouldBeNull();
        }

        [Test]
        public void ShouldNotOverrideWhenResourceActionsDoNotExist()
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
            var testResourceClaims = SetupResourceClaims(testApplication);

            var testResource1ToEdit = testResourceClaims.Single(x => x.ResourceName == "TestParentResourceClaim1");

            Transaction(securityContext => securityContext.ClaimSetResourceClaims
                .Any(x => x.ResourceClaim.ResourceClaimId == testResource1ToEdit.ResourceClaimId && x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId))
                .ShouldBe(false);

            var invalidOverrideModel = new OverrideDefaultAuthorizationStrategyModel
            {
                ResourceClaimId = testResource1ToEdit.ResourceClaimId,
                ClaimSetId = testClaimSet.ClaimSetId,
                AuthorizationStrategyForCreate = appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy4").AuthorizationStrategyId,
                AuthorizationStrategyForRead = appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy2").AuthorizationStrategyId,
                AuthorizationStrategyForUpdate = appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy2").AuthorizationStrategyId,
                AuthorizationStrategyForDelete = appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy2").AuthorizationStrategyId
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new OverrideDefaultAuthorizationStrategyModelValidator(securityContext);
                var validationResults = validator.Validate(invalidOverrideModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("No actions for this claimset and resource exist in the system");
            });
        }
    }
}
