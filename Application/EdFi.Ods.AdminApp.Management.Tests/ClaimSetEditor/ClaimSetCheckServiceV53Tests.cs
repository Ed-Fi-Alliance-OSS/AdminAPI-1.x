// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

//Test using v53 ClaimSet entity against service using v6 SecurityContext
using Application = EdFi.SecurityCompatiblity53.DataAccess.Models.Application;
using ClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;
using ConstructContext = EdFi.Security.DataAccess.Contexts.ISecurityContext;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor;

[TestFixture]
public class ClaimSetCheckServiceV53Tests : SecurityData53TestBase
{
    [Test]
    public void ShouldReturnTrueWhenClaimSetExists()
    {
        var application = new Application { ApplicationName = $"Test Application {DateTime.Now:O}" };
        Save(application);

        var claimSet = new ClaimSet { ClaimSetName = $"Test ClaimSet {DateTime.Now:O}", Application = application };
        Save(claimSet);

        Scoped<ConstructContext>(securityContext =>
        {
            var service = new ClaimSetCheckService(securityContext);
            service.ClaimSetExists(claimSet.ClaimSetName).ShouldBeTrue();
        });
    }

    [Test]
    public void ShouldReturnFalseWhenClaimSetDoesNotExist()
    {
        Scoped<ConstructContext>(securityContext =>
        {
            var service = new ClaimSetCheckService(securityContext);
            service.ClaimSetExists(Guid.NewGuid().ToString()).ShouldBeFalse();
        });
    }
}
