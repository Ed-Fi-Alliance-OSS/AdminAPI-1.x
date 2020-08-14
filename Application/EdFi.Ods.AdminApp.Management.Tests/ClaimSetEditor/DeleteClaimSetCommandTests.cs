// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Moq;
using Shouldly;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;


namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class DeleteClaimSetCommandTests : SecurityDataTestBase
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
                {ClaimSetName = "TestClaimSet_Delete", Application = testApplication};
            Save(testClaimSetToDelete);
            SetupParentResourceClaimsWithChildren(testClaimSetToDelete, testApplication);

            var testClaimSetToPreserve = new ClaimSet
                {ClaimSetName = "TestClaimSet_Preserve", Application = testApplication};
            Save(testClaimSetToPreserve);
            var resourceClaimsForPreservedClaimSet = SetupParentResourceClaimsWithChildren(testClaimSetToPreserve, testApplication);

            var deleteModel = new Mock<IDeleteClaimSetModel>();
            deleteModel.Setup(x => x.Name).Returns(testClaimSetToDelete.ClaimSetName);
            deleteModel.Setup(x => x.Id).Returns(testClaimSetToDelete.ClaimSetId);
            var command = new DeleteClaimSetCommand(TestContext);

            command.Execute(deleteModel.Object);

            TestContext.ClaimSets.SingleOrDefault(x => x.ClaimSetId == testClaimSetToDelete.ClaimSetId).ShouldBeNull();
            TestContext.ClaimSetResourceClaims.Count(x => x.ClaimSet.ClaimSetId == testClaimSetToDelete.ClaimSetId)
                .ShouldBe(0);

            var preservedClaimSet = TestContext.ClaimSets.Single(x => x.ClaimSetId == testClaimSetToPreserve.ClaimSetId);
            preservedClaimSet.ClaimSetName.ShouldBe(testClaimSetToPreserve.ClaimSetName);
            Transaction<SqlServerSecurityContext>(securityContext =>
            {
                var query = new GetResourcesByClaimSetIdQuery(securityContext, GetMapper());

                var results = query.AllResources(testClaimSetToPreserve.ClaimSetId).ToArray();

                var testParentResourceClaimsForId =
                    resourceClaimsForPreservedClaimSet.Where(x => x.ClaimSet.ClaimSetId == testClaimSetToPreserve.ClaimSetId && x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

                results.Length.ShouldBe(testParentResourceClaimsForId.Length);
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
            });
        }

        [Test]
        public void ShouldNotDeleteClaimSetIfNotEditable()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplication1"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet {ClaimSetName = $"TestClaimSet{DateTime.Now:O}", Application = testApplication};
            Save(testClaimSet);

            var claimSetToDelete = new DeleteClaimSetModel()
            {
                Name = testClaimSet.ClaimSetName,
                Id = testClaimSet.ClaimSetId,
                IsEditable = false
            };
            var validator = new DeleteClaimSetModelValidator(TestContext);
            var validationResults = validator.Validate(claimSetToDelete);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("Only user created claim sets can be deleted");

        }

        [Test]
        public void ShouldNotDeleteClaimSetIfNotAnExistingId()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplication2"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = $"TestClaimSet{DateTime.Now:O}", Application = testApplication };
            Save(testClaimSet);

            var claimSetToDelete = new DeleteClaimSetModel()
            {
                Name = testClaimSet.ClaimSetName,
                Id = 99,
                IsEditable = true
            };
            var validator = new DeleteClaimSetModelValidator(TestContext);
            var validationResults = validator.Validate(claimSetToDelete);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("No such claim set exists in the database");

        }

        [Test]
        public void ShouldNotDeleteClaimSetHasAnAssociatedApplication()
        {
            var testApplication = new Application
            {
                ApplicationName = "TestApplication3"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = $"TestClaimSet{DateTime.Now:O}", Application = testApplication };
            Save(testClaimSet);

            var claimSetToDelete = new DeleteClaimSetModel()
            {
                Name = testClaimSet.ClaimSetName,
                Id = testClaimSet.ClaimSetId,
                IsEditable = true,
                VendorApplicationCount = 1
            };
            var validator = new DeleteClaimSetModelValidator(TestContext);
            var validationResults = validator.Validate(claimSetToDelete);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe($"Cannot delete this claim set. This claim set has {claimSetToDelete.VendorApplicationCount} associated application(s).");
        }
    }
}