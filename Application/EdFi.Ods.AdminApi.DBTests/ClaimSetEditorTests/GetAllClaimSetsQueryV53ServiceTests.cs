// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;
using NUnit.Framework;
using Shouldly;

using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class GetAllClaimSetsQueryV53ServiceTests : SecurityData53TestBase
{
    public GetAllClaimSetsQueryV53ServiceTests()
    {
        SeedSecurityContextOnFixtureSetup = true;
    }

    [Test]
    public void Should_Retrieve_ClaimSetNames()
    {
        var application = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(application);

        var claimSet1 = GetClaimSet(application);
        var claimSet2 = GetClaimSet(application);
        Save(claimSet1, claimSet2);

        var claimSetNames = Transaction<string[]>(securityContext =>
        {
            var query = new GetAllClaimSetsQueryV53Service(securityContext);
            return query.Execute().Select(x => x.Name).ToArray();
        });

        claimSetNames.ShouldContain(claimSet1.ClaimSetName);
        claimSetNames.ShouldContain(claimSet2.ClaimSetName);
    }

    private static int _claimSetId = 0;
    private static ClaimSet GetClaimSet(Application application)
    {
        return new ClaimSet
        {
            Application = application,
            ClaimSetName = $"Test Claim Set {_claimSetId++} - {DateTime.Now:O}"
        };
    }
}
