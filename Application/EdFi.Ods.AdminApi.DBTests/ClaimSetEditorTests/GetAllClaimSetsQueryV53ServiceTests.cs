// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
extern alias Compatability;

using System;
using System.Linq;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;
using NUnit.Framework;
using Shouldly;

using ClaimSet = Compatability::EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using Application = Compatability::EdFi.SecurityCompatiblity53.DataAccess.Models.Application;
using EdFi.Ods.AdminApi.Infrastructure;

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

    [Test]
    public void Should_Retrieve_ClaimSetNames_with_offset_and_limit()
    {
        var application = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(application);

        var claimSet1 = GetClaimSet(application);
        var claimSet2 = GetClaimSet(application);
        var claimSet3 = GetClaimSet(application);
        var claimSet4 = GetClaimSet(application);
        var claimSet5 = GetClaimSet(application);

        Save(claimSet1, claimSet2, claimSet3, claimSet4, claimSet5);

        var commonQueryParams = new CommonQueryParams(0, 2);
        var claimSetNames = Transaction<string[]>(securityContext =>
        {
            var query = new GetAllClaimSetsQueryV53Service(securityContext);
            return query.Execute(commonQueryParams).Select(x => x.Name).ToArray();
        });

        claimSetNames.Length.ShouldBe(2);
        claimSetNames.ShouldContain(claimSet1.ClaimSetName);
        claimSetNames.ShouldContain(claimSet2.ClaimSetName);

        commonQueryParams.Offset = 2;
        claimSetNames = Transaction<string[]>(securityContext =>
        {
            var query = new GetAllClaimSetsQueryV53Service(securityContext);
            return query.Execute(commonQueryParams).Select(x => x.Name).ToArray();
        });

        claimSetNames.Length.ShouldBe(2);
        claimSetNames.ShouldContain(claimSet3.ClaimSetName);
        claimSetNames.ShouldContain(claimSet4.ClaimSetName);

        commonQueryParams.Offset = 4;
        claimSetNames = Transaction<string[]>(securityContext =>
        {
            var query = new GetAllClaimSetsQueryV53Service(securityContext);
            return query.Execute(commonQueryParams).Select(x => x.Name).ToArray();
        });

        claimSetNames.Length.ShouldBe(1);
        claimSetNames.ShouldContain(claimSet5.ClaimSetName);
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
