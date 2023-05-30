// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using NUnit.Framework;
using Shouldly;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using System.Net;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;

using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class GetClaimSetByIdQueryV53ServiceTests : SecurityData53TestBase
{
    [Test]
    public void ShouldGetClaimSetById()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testClaimSet = new ClaimSet { ClaimSetName = "TestClaimSet", Application = testApplication };
        Save(testClaimSet);

        using var securityContext = TestContext;
        var query = new GetClaimSetByIdQueryV53Service(securityContext);
        var result = query.Execute(testClaimSet.ClaimSetId);
        result.Name.ShouldBe(testClaimSet.ClaimSetName);
        result.Id.ShouldBe(testClaimSet.ClaimSetId);
    }

    [Test]
    public void ShouldThrowExceptionForNonExistingClaimSetId()
    {
        const int NonExistingClaimSetId = 1;

        using var securityContext = TestContext;
        EnsureZeroClaimSets(securityContext);
        var adminApiException = Assert.Throws<AdminApiException>(() =>
        {
            var query = new GetClaimSetByIdQueryV53Service(securityContext);
            query.Execute(NonExistingClaimSetId);
        });
        adminApiException.ShouldNotBeNull();
        adminApiException.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        adminApiException.Message.ShouldBe("No such claim set exists in the database.");

        static void EnsureZeroClaimSets(ISecurityContext database)
        {
            foreach (var entity in database.ClaimSets)
                database.ClaimSets.Remove(entity);
            database.SaveChanges();
        }
    }
}
