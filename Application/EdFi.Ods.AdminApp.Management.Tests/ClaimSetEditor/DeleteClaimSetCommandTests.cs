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
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

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

            Scoped<ISecurityContext>(securityContext =>
            {
                var command = new DeleteClaimSetCommand(securityContext);

                command.Execute(deleteModel.Object);
            });

            Transaction(securityContext => securityContext.ClaimSets.SingleOrDefault(x => x.ClaimSetId == testClaimSetToDelete.ClaimSetId)).ShouldBeNull();
            Transaction(securityContext => securityContext.ClaimSetResourceClaims.Count(x => x.ClaimSet.ClaimSetId == testClaimSetToDelete.ClaimSetId))
                .ShouldBe(0);

            var preservedClaimSet = Transaction(securityContext => securityContext.ClaimSets.Single(x => x.ClaimSetId == testClaimSetToPreserve.ClaimSetId));
            preservedClaimSet.ClaimSetName.ShouldBe(testClaimSetToPreserve.ClaimSetName);

            var results =
                Scoped<IGetResourcesByClaimSetIdQuery, Management.ClaimSetEditor.ResourceClaim[]>(
                    query => query.AllResources(testClaimSetToPreserve.ClaimSetId).ToArray());

            var testParentResourceClaimsForId =
                resourceClaimsForPreservedClaimSet.Where(x => x.ClaimSet.ClaimSetId == testClaimSetToPreserve.ClaimSetId && x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

            results.Length.ShouldBe(testParentResourceClaimsForId.Length);
            results.Select(x => x.Name).ShouldBe(testParentResourceClaimsForId.Select(x => x.ResourceName), true);
            results.Select(x => x.Id).ShouldBe(testParentResourceClaimsForId.Select(x => x.ResourceClaimId), true);
            results.All(x => x.Create).ShouldBe(true);

            Transaction(securityContext =>
            {
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

            var exception = Assert.Throws<Exception>(() => Scoped<ISecurityContext>(securityContext =>
            {
                var command = new DeleteClaimSetCommand(securityContext);
                command.Execute(deleteModel.Object);
            }));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"Claim set({systemReservedClaimSet.ClaimSetName}) is system reserved.Can not be deleted.");
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

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new DeleteClaimSetModelValidator(securityContext);
                var validationResults = validator.Validate(claimSetToDelete);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("Only user created claim sets can be deleted");
            });
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

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new DeleteClaimSetModelValidator(securityContext);
                var validationResults = validator.Validate(claimSetToDelete);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("No such claim set exists in the database");
            });
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

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new DeleteClaimSetModelValidator(securityContext);
                var validationResults = validator.Validate(claimSetToDelete);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe($"Cannot delete this claim set. This claim set has {claimSetToDelete.VendorApplicationCount} associated application(s).");
            });
        }
    }
}
