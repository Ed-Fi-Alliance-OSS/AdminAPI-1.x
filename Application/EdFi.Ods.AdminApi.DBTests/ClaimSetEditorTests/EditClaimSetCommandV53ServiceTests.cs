// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using Shouldly;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using VendorApplication = EdFi.Admin.DataAccess.Models.Application;
using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;
using EdFi.Ods.AdminApi.Features.ClaimSets;
using EdFi.Ods.AdminApi.Infrastructure;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class EditClaimSetCommandV53ServiceTests : SecurityData53TestBase
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

        var editModel = new EditClaimSetModel { ClaimSetName = "TestClaimSetEdited", ClaimSetId = alreadyExistingClaimSet.ClaimSetId };

        using var securityContext = TestContext;
        UsersTransaction((usersContext) =>
        {
            var command = new EditClaimSetCommandV53Service(securityContext, usersContext);
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

        var systemReservedClaimSet = new ClaimSet { ClaimSetName = "Ed-Fi Sandbox", Application = testApplication };
        Save(systemReservedClaimSet);

        var editModel = new EditClaimSetModel { ClaimSetName = "TestClaimSetEdited", ClaimSetId = systemReservedClaimSet.ClaimSetId };

        using var securityContext = TestContext;
        var exception = Assert.Throws<AdminAppException>(() => UsersTransaction(usersContext =>
        {
            var command = new EditClaimSetCommandV53Service(securityContext, usersContext);
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
        UsersTransaction(usersContext =>
        {
            var command = new EditClaimSetCommandV53Service(securityContext, usersContext);
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
        UsersTransaction(usersContext =>
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
        UsersTransaction(
            usersContext =>
            {
                var results = new GetApplicationsByClaimSetId53Query(securityContext, usersContext).Execute(claimSetId);
                var testApplications = usersContext.Applications.Where(x => x.ClaimSetName == claimSetNameToAssert).ToList();
                results.Count().ShouldBe(testApplications.Count);
                results.Select(x => x.Name).ShouldBe(testApplications.Select(x => x.ApplicationName), true);
            });
    }
}
