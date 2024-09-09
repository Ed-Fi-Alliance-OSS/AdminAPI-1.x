// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Shouldly;
using ClaimSetModel = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class GetAllClaimSetsQueryV6ServiceTests : SecurityDataTestBase
{
    public GetAllClaimSetsQueryV6ServiceTests()
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
            var query = new GetAllClaimSetsQueryV6Service(securityContext, Testing.GetAppSettings());
            return query.Execute().Select(x => x.Name).ToArray();
        });

        claimSetNames.ShouldContain(claimSet1.ClaimSetName);
        claimSetNames.ShouldContain(claimSet2.ClaimSetName);
    }

    [Test]
    public void Should_Retrieve_EdfiPreset_ClaimSet()
    {
        var application = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(application);

        var claimSet1 = GetClaimSet(application, true);
        var claimSet2 = GetClaimSet(application);
        Save(claimSet1, claimSet2);

        var claimSets = Transaction(securityContext =>
        {
            var query = new GetAllClaimSetsQueryV6Service(securityContext, Testing.GetAppSettings());
            return query.Execute().ToArray();
        });

        var edfiPresetClaimSet = claimSets.Where( x=> x.Name.Equals(claimSet1.ClaimSetName) && x.IsEditable == false).ToList();
        edfiPresetClaimSet.Count().ShouldBe(1);
    }

    private static int _claimSetId = 0;
    private static ClaimSetModel GetClaimSet(Application application, bool IsEdfiPreset = false)
    {
        return new ClaimSetModel
        {
            Application = application,
            ClaimSetName = $"Test Claim Set {_claimSetId++} - {DateTime.Now:O}",
            IsEdfiPreset= IsEdfiPreset
        };
    }
}
