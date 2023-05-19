// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using NUnit.Framework;
using EdFi.Ods.Admin.Api.Infrastructure.ClaimSetEditor;
using Shouldly;
using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;

namespace EdFi.Ods.Admin.Api.DBTests.ClaimSetEditorTests;

[TestFixture]
public class AddClaimSetCommandV53ServiceTests : SecurityData53TestBase
{
    [Test]
    public void ShouldAddClaimSet()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var newClaimSet = new AddClaimSetModel {ClaimSetName = "TestClaimSet"};

        var addedClaimSetId = 0;
        ClaimSet addedClaimSet = null;
        using (var context = base.TestContext)
        {
            var command = new AddClaimSetCommandV53Service(context);
            addedClaimSetId = command.Execute(newClaimSet);
            addedClaimSet = context.ClaimSets.Single(x => x.ClaimSetId == addedClaimSetId);
        }
        addedClaimSet.ClaimSetName.ShouldBe(newClaimSet.ClaimSetName);
    }
}
