// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.Admin.Api.Features.ClaimSets;
using Shouldly;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using Application = EdFi.Security.DataAccess.Models.Application;


namespace EdFi.Ods.Admin.Api.Tests.ClaimSetEditor;

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

}
