// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor;

[TestFixture]
public class GetAllAuthorizationStrategiesQueryTests : SecurityDataTestBase
{
    [Test]
    public void ShouldGetAllAuthorizationStrategies()
    {
        LoadSeedData();

        using var securityContext = TestContext;
        var query = new GetAllAuthorizationStrategiesQuery(securityContext);
        var resultNames = query.Execute().Select(x => x.AuthStrategyName).ToList();

        resultNames.Count.ShouldBe(2);

        resultNames.ShouldContain("NamespaceBased");
        resultNames.ShouldContain("NoFurtherAuthorizationRequired");
    }
}
