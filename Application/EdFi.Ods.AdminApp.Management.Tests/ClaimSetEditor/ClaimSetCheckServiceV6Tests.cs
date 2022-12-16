// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using NUnit.Framework;
using System;
using EdFi.Security.DataAccess.Contexts;
using Shouldly;

using static EdFi.Ods.AdminApp.Management.Tests.Testing;

using Application = EdFi.Security.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor;

[TestFixture]
public class ClaimSetCheckServiceV6Tests : SecurityDataTestBase
{
    [Test]
    public void ShouldReturnTrueWhenClaimSetExists()
    {
        var application = new Application { ApplicationName = $"Test Application {DateTime.Now:O}" };
        Save(application);

        var claimSet = new ClaimSet { ClaimSetName = $"Test ClaimSet {DateTime.Now:O}", Application = application };
        Save(claimSet);

        Scoped<ISecurityContext>(securityContext =>
        {
            var service = new ClaimSetCheckService(securityContext);
            service.ClaimSetExists(claimSet.ClaimSetName).ShouldBeTrue();
        });
    }

    [Test]
    public void ShouldReturnFalseWhenClaimSetDoesNotExist()
    {
        Scoped<ISecurityContext>(securityContext =>
        {
            var service = new ClaimSetCheckService(securityContext);
            service.ClaimSetExists(Guid.NewGuid().ToString()).ShouldBeFalse();
        });
    }
}
