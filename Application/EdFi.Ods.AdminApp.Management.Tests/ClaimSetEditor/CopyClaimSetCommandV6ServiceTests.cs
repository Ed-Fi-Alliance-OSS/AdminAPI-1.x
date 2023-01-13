// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Moq;
using Shouldly;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using EdFi.Ods.AdminApp.Management.Database.Queries;

using static EdFi.Ods.AdminApp.Management.Tests.Testing;

using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class CopyClaimSetCommandV6ServiceTests : SecurityDataTestBase
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

            var testResourceClaims = SetupParentResourceClaimsWithChildren(testClaimSet, testApplication,
               UniqueNameList("ParentRc", 3), UniqueNameList("ChildRc", 1));

            var newClaimSet = new Mock<ICopyClaimSetModel>();
            newClaimSet.Setup(x => x.Name).Returns("TestClaimSet_Copy");
            newClaimSet.Setup(x => x.OriginalId).Returns(testClaimSet.ClaimSetId);

            var copyClaimSetId = 0;
            ClaimSet copiedClaimSet = null;
            using (var securityContext = TestContext)
            {
                var command = new CopyClaimSetCommandV6Service(securityContext);
                copyClaimSetId =  command.Execute(newClaimSet.Object);
                copiedClaimSet = securityContext.ClaimSets.Single(x => x.ClaimSetId == copyClaimSetId);

                copiedClaimSet.ClaimSetName.ShouldBe(newClaimSet.Object.Name);
                copiedClaimSet.ForApplicationUseOnly.ShouldBe(false);
                copiedClaimSet.IsEdfiPreset.ShouldBe(false);

                var results = ResourceClaimsForClaimSet(copiedClaimSet.ClaimSetId).ToList();

                var testParentResourceClaimsForId =
                    testResourceClaims.Where(x => x.ClaimSet.ClaimSetId == testClaimSet.ClaimSetId && x.ResourceClaim.ParentResourceClaim == null).Select(x => x.ResourceClaim).ToArray();

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

            using var securityContext = TestContext;
            var getAllClaimSetsQuery = new GetAllClaimSetsQuery(securityContext);
            var validator = new CopyClaimSetModelValidator(getAllClaimSetsQuery);
            var validationResults = validator.Validate(newClaimSet);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("The new claim set must have a unique name");
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

            using var securityContext = TestContext;
            var getAllClaimSetsQuery = new GetAllClaimSetsQuery(securityContext);
            var validator = new CopyClaimSetModelValidator(getAllClaimSetsQuery);
            var validationResults = validator.Validate(newClaimSet);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("The claim set name must be less than 255 characters.");
        }
    }
}
