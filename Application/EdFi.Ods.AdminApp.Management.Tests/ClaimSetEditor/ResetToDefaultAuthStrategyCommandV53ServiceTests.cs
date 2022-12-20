// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using AutoMapper;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using Shouldly;

using static EdFi.Ods.AdminApp.Management.Tests.Testing;

using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;
using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using EdFi.Ods.AdminApp.Management.Api.Automapper;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class ResetToDefaultAuthStrategyCommandV53ServiceTests : SecurityData53TestBase
    {
        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AdminManagementMappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Test]
        public void ShouldResetAuthorizationStrategiesForParentResourcesOnClaimSet()
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
            var testResourceToEdit = testResourceClaims.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim1");

            var resultResourceClaimBeforeOverride = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testResourceToEdit.ResourceClaimId);
            resultResourceClaimBeforeOverride.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
            resultResourceClaimBeforeOverride.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaimBeforeOverride.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaimBeforeOverride.AuthStrategyOverridesForCRUD[3].ShouldBeNull();

            SetupOverridesForResourceCreateAction(testResourceToEdit.ResourceClaimId, testClaimSet.ClaimSetId,
                appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy4")
                    .AuthorizationStrategyId);

            var resultResourceClaimAfterOverride = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testResourceToEdit.ResourceClaimId);

            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[0].ShouldNotBeNull();
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[0].AuthStrategyName.ShouldBe("TestAuthStrategy4");

            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[3].ShouldBeNull();


            var resetModel = new ResetToDefaultAuthStrategyModel
            {
                ResourceClaimId = testResourceToEdit.ResourceClaimId,
                ClaimSetId = testClaimSet.ClaimSetId
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new ResetToDefaultAuthStrategyCommand(new StubOdsSecurityModelVersionResolver.V3_5(),
                    new ResetToDefaultAuthStrategyCommandV53Service(securityContext), null);
                command.Execute(resetModel);
            });

            var resultResourceClaimAfterReset = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testResourceToEdit.ResourceClaimId);
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[3].ShouldBeNull();
        }

        [Test]
        public void ShouldResetAuthorizationStrategiesForChildResourcesOnClaimSet()
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

            var testParentResource = testResourceClaims.Select(x => x.ResourceClaim).Single(x => x.ResourceName == "TestParentResourceClaim1");
            var testChildResourceToEdit = testResourceClaims.Select(x => x.ResourceClaim).Single(x =>
                x.ResourceName == "TestChildResourceClaim1" &&
                x.ParentResourceClaimId == testParentResource.ResourceClaimId);

            var resultParentResource = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testParentResource.ResourceClaimId);
            var resultResourceBeforeOverride =
                resultParentResource.Children.Single(x => x.Id == testChildResourceToEdit.ResourceClaimId);

            resultResourceBeforeOverride.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
            resultResourceBeforeOverride.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceBeforeOverride.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceBeforeOverride.AuthStrategyOverridesForCRUD[3].ShouldBeNull();

            SetupOverridesForResourceCreateAction(testChildResourceToEdit.ResourceClaimId, testClaimSet.ClaimSetId,
                appAuthorizationStrategies.Single(x => x.AuthorizationStrategyName == "TestAuthStrategy4")
                    .AuthorizationStrategyId);

            resultParentResource = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testParentResource.ResourceClaimId);
            var resultResourceClaimAfterOverride = resultParentResource.Children.Single(x => x.Id == testChildResourceToEdit.ResourceClaimId);
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[0].ShouldNotBeNull();
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[0].AuthStrategyName.ShouldBe("TestAuthStrategy4");

            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaimAfterOverride.AuthStrategyOverridesForCRUD[3].ShouldBeNull();


            var resetModel = new ResetToDefaultAuthStrategyModel
            {
                ResourceClaimId = testChildResourceToEdit.ResourceClaimId,
                ClaimSetId = testClaimSet.ClaimSetId
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new ResetToDefaultAuthStrategyCommand(new StubOdsSecurityModelVersionResolver.V3_5(),
                    new ResetToDefaultAuthStrategyCommandV53Service(securityContext), null);
                command.Execute(resetModel);
            });

            resultParentResource = SingleResourceClaimForClaimSet(testClaimSet.ClaimSetId, testParentResource.ResourceClaimId);
            var resultResourceClaimAfterReset = resultParentResource.Children.Single(x => x.Id == testChildResourceToEdit.ResourceClaimId);
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[0].ShouldBeNull();
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[1].ShouldBeNull();
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[2].ShouldBeNull();
            resultResourceClaimAfterReset.AuthStrategyOverridesForCRUD[3].ShouldBeNull();
        }

        [Test]
        public void ShouldNotResetWhenResourceActionsDoNotExist()
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

            var testResourceClaims = SetupResourceClaims(testApplication);

            var testResourceToEdit = testResourceClaims.Single(x => x.ResourceName == "TestParentResourceClaim1");

            Transaction(securityContext => securityContext.ClaimSetResourceClaims
                .Any(x => x.ResourceClaim.ResourceClaimId == testResourceToEdit.ResourceClaimId && x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId))
                .ShouldBe(false);

            var invalidResetModel = new ResetToDefaultAuthStrategyModel
            {
                ResourceClaimId = testResourceToEdit.ResourceClaimId,
                ClaimSetId = testClaimSet.ClaimSetId
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new ResetToDefaultAuthStrategyCommand(new StubOdsSecurityModelVersionResolver.V3_5(),
                    new ResetToDefaultAuthStrategyCommandV53Service(securityContext), null);
                command.Execute(invalidResetModel);
            });

            Scoped<ISecurityContext>(securityContext =>
            {
                var getResourcesByClaimSetIdQuery = new GetResourcesByClaimSetIdQuery(new StubOdsSecurityModelVersionResolver.V3_5(),
                    new GetResourcesByClaimSetIdQueryV53Service(securityContext, _mapper), null);

                var validator = new ResetToDefaultAuthStrategyModelValidator(getResourcesByClaimSetIdQuery);
                var validationResults = validator.Validate(invalidResetModel);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("No actions for this claimset and resource exist in the system");
            });
        }

        private void SetupOverridesForResourceCreateAction(int resourceClaimId, int claimSetId, int authorizationStrategyId)
        {
            var overrideModel = new OverrideDefaultAuthorizationStrategyModel
            {
                ResourceClaimId = resourceClaimId,
                ClaimSetId = claimSetId,
                AuthorizationStrategyForCreate = authorizationStrategyId,
                AuthorizationStrategyForRead = 0,
                AuthorizationStrategyForUpdate = 0,
                AuthorizationStrategyForDelete = 0
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new OverrideDefaultAuthorizationStrategyCommand(
                    new StubOdsSecurityModelVersionResolver.V3_5(),
                    new OverrideDefaultAuthorizationStrategyV53Service(securityContext), null);
                command.Execute(overrideModel);
            });
        }

        private ResourceClaim SingleResourceClaimForClaimSet(int securityContextClaimSetId, int resourceClaimId)
        {
            ResourceClaim resourceClaim = null;
            Scoped<ISecurityContext>(securityContext =>
            {
                var getResourcesByClaimSetIdQuery = new GetResourcesByClaimSetIdQuery(new StubOdsSecurityModelVersionResolver.V3_5(),
                    new GetResourcesByClaimSetIdQueryV53Service(securityContext, _mapper), null);
                resourceClaim = getResourcesByClaimSetIdQuery.SingleResource(securityContextClaimSetId, resourceClaimId);
            });
            return resourceClaim;
        }
    }
}
