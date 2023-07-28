// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetAllActionsQueryTests : SecurityDataTestBase
{
    [Test]
    public void ShouldGetAllActions()
    {
        LoadSeedData();

        using var securityContext = TestContext;
        var query = new GetAllActionsQuery(securityContext);
        var resultNames = query.Execute().Select(x => x.ActionName).ToList();

        resultNames.Count.ShouldBe(4);

        resultNames.ShouldContain("Create");
        resultNames.ShouldContain("Read");
    }

}
