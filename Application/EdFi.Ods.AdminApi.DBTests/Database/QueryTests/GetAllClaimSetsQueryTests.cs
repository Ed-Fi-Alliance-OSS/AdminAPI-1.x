// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetAllClaimSetsQueryTests : SecurityDataTestBase
{
    public GetAllClaimSetsQueryTests()
    {
        SeedSecurityContextOnFixtureSetup = true;
    }

    [Test]
    public void Should_Retreive_ClaimSetNames()
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
            var query = new GetAllClaimSetsQuery(securityContext);
            return query.Execute().Select(x => x.Name).ToArray();
        });

        claimSetNames.ShouldContain(claimSet1.ClaimSetName);
        claimSetNames.ShouldContain(claimSet2.ClaimSetName);
    }

    [Test]
    public void Should_Retreive_ClaimSetNames_With_Offset_And_Limit()
    {
        var application = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(application);

        var claimSet1 = GetClaimSet(application);
        var claimSet2 = GetClaimSet(application);
        Save(claimSet1, claimSet2);

        var offset = 0;
        var limit = 2;

        var claimSetNames = Transaction<string[]>(securityContext =>
        {
            var query = new GetAllClaimSetsQuery(securityContext);
            return query.Execute(offset, limit).Select(x => x.Name).ToArray();
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
