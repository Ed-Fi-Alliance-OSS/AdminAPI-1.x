// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Shouldly;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using VendorApplication = EdFi.Admin.DataAccess.Models.Application;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class EditClaimSetCommandV6ServiceTests : SecurityDataTestBase
    {
        [Test]
        public void ShouldEditClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var alreadyExistingClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
            Save(alreadyExistingClaimSet);

            var editModel = new EditClaimSetModel {ClaimSetName = "TestClaimSetEdited", ClaimSetId = alreadyExistingClaimSet.ClaimSetId};

            using var securityContext = TestContext;
            Scoped<IUsersContext>((usersContext) =>
            {
                var command = new EditClaimSetCommandV6Service(securityContext, usersContext);
                command.Execute(editModel);
            });

            var editedClaimSet = securityContext.ClaimSets.Single(x => x.ClaimSetId == alreadyExistingClaimSet.ClaimSetId);
            editedClaimSet.ClaimSetName.ShouldBe(editModel.ClaimSetName);
        }

        [Test]
        public void ShouldThrowExceptionOnEditSystemReservedClaimSet()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var systemReservedClaimSet = new ClaimSet { ClaimSetName = "Ed-Fi Sandbox", Application = testApplication, IsEdfiPreset=true};
            Save(systemReservedClaimSet);

            var editModel = new EditClaimSetModel { ClaimSetName = "TestClaimSetEdited", ClaimSetId = systemReservedClaimSet.ClaimSetId };

            using var securityContext = TestContext;
            var exception = Assert.Throws<AdminAppException>(() => Scoped<IUsersContext>(usersContext =>
            {
                var command = new EditClaimSetCommandV6Service(TestContext, usersContext);
                command.Execute(editModel);
            }));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"Claim set ({systemReservedClaimSet.ClaimSetName}) is system reserved.May not be modified.");
        }

        [Test]
        public void ShouldEditClaimSetWithVendorApplications()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var claimSetToBeEdited = new ClaimSet { ClaimSetName = $"TestClaimSet{Guid.NewGuid():N}", Application = testApplication };
            Save(claimSetToBeEdited);
            SetupVendorApplicationsForClaimSet(claimSetToBeEdited);

            var claimSetNotToBeEdited = new ClaimSet { ClaimSetName = $"TestClaimSet{Guid.NewGuid():N}", Application = testApplication };
            Save(claimSetNotToBeEdited);
            SetupVendorApplicationsForClaimSet(claimSetNotToBeEdited);

            var editModel = new EditClaimSetModel { ClaimSetName = "TestClaimSetEdited", ClaimSetId = claimSetToBeEdited.ClaimSetId };

            using var securityContext = TestContext;
            Scoped<IUsersContext>(usersContext =>
            {
                var command = new EditClaimSetCommandV6Service(securityContext, usersContext);
                command.Execute(editModel);
            });

            var editedClaimSet = securityContext.ClaimSets.Single(x => x.ClaimSetId == claimSetToBeEdited.ClaimSetId);
            editedClaimSet.ClaimSetName.ShouldBe(editModel.ClaimSetName);
            AssertApplicationsForClaimSet(claimSetToBeEdited.ClaimSetId, editModel.ClaimSetName, securityContext);

            var unEditedClaimSet = securityContext.ClaimSets.Single(x => x.ClaimSetId == claimSetNotToBeEdited.ClaimSetId);
            unEditedClaimSet.ClaimSetName.ShouldBe(claimSetNotToBeEdited.ClaimSetName);
            AssertApplicationsForClaimSet(claimSetNotToBeEdited.ClaimSetId, claimSetNotToBeEdited.ClaimSetName, securityContext);
        }

        private void SetupVendorApplicationsForClaimSet(ClaimSet testClaimSet, int applicationCount = 5)
        {
            Scoped<IUsersContext>(usersContext =>
            {
                foreach (var _ in Enumerable.Range(1, applicationCount))
                {
                    usersContext.Applications.Add(new VendorApplication
                    {
                        ApplicationName = $"TestAppVendorName{Guid.NewGuid():N}",
                        ClaimSetName = testClaimSet.ClaimSetName,
                        OperationalContextUri = OperationalContext.DefaultOperationalContextUri
                    });
                }
                usersContext.SaveChanges();
            });
        }

        private void AssertApplicationsForClaimSet(int claimSetId, string claimSetNameToAssert, ISecurityContext securityContext)
        {
            Scoped<IUsersContext>(
                usersContext =>
                {
                    var results = new GetApplicationsByClaimSetIdQuery(securityContext, usersContext).Execute(claimSetId);
                    var testApplications = usersContext.Applications.Where(x => x.ClaimSetName == claimSetNameToAssert).ToList();
                    results.Count().ShouldBe(testApplications.Count());
                    results.Select(x => x.Name).ShouldBe(testApplications.Select(x => x.ApplicationName), true);
                });
        }

        [Test]
        public void ShouldNotEditClaimSetIfNameNotUnique()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var alreadyExistingClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(alreadyExistingClaimSet);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet2", Application = testApplication };
            Save(testClaimSet);

            var editModel = new EditClaimSetModel { ClaimSetName = "TestClaimSet1", ClaimSetId = testClaimSet.ClaimSetId };

            using var securityContext = TestContext;
            var validator = new EditClaimSetModelValidator(AllClaimSetsQuery(securityContext),
                ClaimSetByIdQuery(securityContext));
            var validationResults = validator.Validate(editModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("A claim set with this name already exists in the database. Please enter a unique name.");
        }

        [Test]
        public void ShouldNotEditClaimSetIfNameEmpty()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(testClaimSet);

            var editModel = new EditClaimSetModel { ClaimSetName = "", ClaimSetId = testClaimSet.ClaimSetId };

            using var securityContext = TestContext;
            var validator = new EditClaimSetModelValidator(AllClaimSetsQuery(securityContext),
                ClaimSetByIdQuery(securityContext));
            var validationResults = validator.Validate(editModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("'Claim Set Name' must not be empty.");
        }

        [Test]
        public void ShouldNotEditClaimSetIfNameLengthGreaterThan255Characters()
        {
            var testApplication = new Application
            {
                ApplicationName = $"Test Application {DateTime.Now:O}"
            };
            Save(testApplication);

            var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet1", Application = testApplication };
            Save(testClaimSet);

            var editModel = new EditClaimSetModel { ClaimSetName = "ThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255CharactersThisIsAClaimSetWithNameLengthGreaterThan255Characters", ClaimSetId = testClaimSet.ClaimSetId };

            using var securityContext = TestContext;
            var validator = new EditClaimSetModelValidator(AllClaimSetsQuery(securityContext),
                ClaimSetByIdQuery(securityContext));
            var validationResults = validator.Validate(editModel);
            validationResults.IsValid.ShouldBe(false);
            validationResults.Errors.Single().ErrorMessage.ShouldBe("The claim set name must be less than 255 characters.");
        }

        private GetClaimSetByIdQuery ClaimSetByIdQuery(ISecurityContext securityContext) => new GetClaimSetByIdQuery(new StubOdsSecurityModelVersionResolver.V6(),
                        null, new GetClaimSetByIdQueryV6Service(securityContext));

        private IGetAllClaimSetsQuery AllClaimSetsQuery(ISecurityContext securityContext) => new GetAllClaimSetsQuery(securityContext);
    }
}
