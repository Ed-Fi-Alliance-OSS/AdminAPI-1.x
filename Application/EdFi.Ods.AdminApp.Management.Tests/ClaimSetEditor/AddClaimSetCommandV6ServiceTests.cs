// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;

using Shouldly;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;
using AddClaimSetModel = EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets.AddClaimSetModel;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class AddClaimSetCommandV6ServiceTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldAddClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var newClaimSet = new AddClaimSetModel { ClaimSetName = "TestClaimSet" };

            var addedClaimSetId = 0;
            ClaimSet addedClaimSet = null;
            using (var securityContext = TestContext)
            {
                var command = new AddClaimSetCommandV6Service(securityContext);
                addedClaimSetId = command.Execute(newClaimSet);
                addedClaimSet = securityContext.ClaimSets.Single(x => x.ClaimSetId == addedClaimSetId);
            }
            addedClaimSet.ClaimSetName.ShouldBe(newClaimSet.ClaimSetName);
            addedClaimSet.ForApplicationUseOnly.ShouldBe(false);
            addedClaimSet.IsEdfiPreset.ShouldBe(false);
        }

        [Test]
        public void ShouldNotAddClaimSetIfNameNotUnique()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var alreadyExistingClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(alreadyExistingClaimSet);

            var newClaimSet = new AddClaimSetModel { ClaimSetName = "TestClaimSet" };

            using (var securityContext = TestContext)
            {
                var validator = new AddClaimSetModelValidator(new GetAllClaimSetsQuery(securityContext));
                var validationResults = validator.Validate(newClaimSet);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("A claim set with this name already exists in the database. Please enter a unique name.");
            }
        }

        [Test]
        public void ShouldNotAddClaimSetIfNameEmpty()
        {
            var newClaimSet = new AddClaimSetModel { ClaimSetName = "" };

            using (var securityContext = TestContext)
            {
                var validator = new AddClaimSetModelValidator(new GetAllClaimSetsQuery(securityContext));
                var validationResults = validator.Validate(newClaimSet);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("'Claim Set Name' must not be empty.");
            }
        }

        [Test]
        public void ShouldNotAddClaimSetIfNameLengthGreaterThan255Characters()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var newClaimSet = new AddClaimSetModel { ClaimSetName = "ThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255Characters" };

            using (var securityContext = TestContext)
            {
                var validator = new AddClaimSetModelValidator(new GetAllClaimSetsQuery(securityContext));
                var validationResults = validator.Validate(newClaimSet);
                validationResults.IsValid.ShouldBe(false);
                validationResults.Errors.Single().ErrorMessage.ShouldBe("The claim set name must be less than 255 characters.");
            }
        }
    }
}
