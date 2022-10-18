// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web;
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
    public class CopyClaimSetCommandTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldCopyClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet {ClaimSetName = "TestClaimSet", Application = testApplication};
            Save(testClaimSet);

            var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication);

            var newClaimSet = new Mock<ICopyClaimSetModel>();
            newClaimSet.Setup(x => x.Name).Returns("TestClaimSet_Copy");
            newClaimSet.Setup(x => x.OriginalId).Returns(testClaimSet.ClaimSetId);

            int copyClaimSetId = Scoped<ISecurityContext, int>(securityContext =>
            {
                var command = new CopyClaimSetCommand(securityContext);
                return command.Execute(newClaimSet.Object);
            });

            var copiedClaimSet = Transaction(securityContext => securityContext.ClaimSets.Single(x => x.ClaimSetId == copyClaimSetId));
            copiedClaimSet.ClaimSetName.ShouldBe(newClaimSet.Object.Name);

            var results = Scoped<IGetResourcesByClaimSetIdQuery, Management.ClaimSetEditor.ResourceClaim[]>(
                query => query.AllResources(copiedClaimSet.ClaimSetId).ToArray());

            var testParentResourceClaimsForId =
                testResourceClaims.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

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

            Scoped<IUsersContext>(usersContext =>
            {
                usersContext.Applications.Count(x => x.ClaimSetName == copiedClaimSet.ClaimSetName)
                    .ShouldBe(0);
            });
        }

        [Test]
        public void ShouldNotCopyClaimSetIfNameNotUnique()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(testClaimSet);

            var newClaimSet = new CopyClaimSetModel()
            {
                Name = "TestClaimSet",
                OriginalId = testClaimSet.ClaimSetId
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new CopyClaimSetModelValidator(securityContext);
                var validationResults = validator.Validate(newClaimSet);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("The new claim set must have a unique name");
            });
        }

        [Test]
        public void ShouldNotCopyClaimSetIfNameLengthGreaterThan255Characters()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(testClaimSet);

            var newClaimSet = new CopyClaimSetModel()
            {
                Name = "ThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255Characters",
                OriginalId = testClaimSet.ClaimSetId
            };

            Scoped<ISecurityContext>(securityContext =>
            {
                var validator = new CopyClaimSetModelValidator(securityContext);
                var validationResults = validator.Validate(newClaimSet);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("The claim set name must be less than 255 characters.");
            });
        }

    }
}
