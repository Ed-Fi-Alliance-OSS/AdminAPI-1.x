// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using NUnit.Framework;
using System;
using Shouldly;

using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor;

[TestFixture]
public class ClaimSetCheckServiceV6Tests : SecurityDataTestBase
{
    [Test]
    public void ShouldReturnTrueIfRequiredClaimSetsExist()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        var testAdminAppClaimSet = new ClaimSet { ClaimSetName = CloudOdsAdminApp.InternalAdminAppClaimSet, Application = testApplication };
        Save(testAdminAppClaimSet);

        using var securityContext = TestContext;
        var service = new ClaimSetCheckService(securityContext);
        var result = service.RequiredClaimSetsExist();
        result.ShouldBeTrue();
    }

    [Test]
    public void ShouldReturnFalseIfRequiredClaimSetsDoNotExist()
    {
        var testApplication = new Application
        {
            ApplicationName = $"Test Application {DateTime.Now:O}"
        };
        Save(testApplication);

        using var securityContext = TestContext;
        var service = new ClaimSetCheckService(securityContext);
        var result = service.RequiredClaimSetsExist();
        result.ShouldBeFalse();
    }
}
