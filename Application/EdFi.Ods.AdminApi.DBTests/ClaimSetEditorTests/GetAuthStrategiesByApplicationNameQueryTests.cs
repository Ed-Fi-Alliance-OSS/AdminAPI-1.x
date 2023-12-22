// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using NUnit.Framework;
using Shouldly;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

using Application = EdFi.Admin.DataAccess.Models.Application;

namespace EdFi.Ods.AdminApi.DBTests.ClaimSetEditorTests;

[TestFixture]
public class GetAuthStrategiesByApplicationNameQueryTests : SecurityDataTestBase
{
    //[Test]
    //public void ShouldGetAllTheAuthorizationStrategiesFromAnApplication()
    //{
    //    var testApplication = new Application
    //    {
    //        ApplicationName = "TestApplicationName"
    //    };
    //    SaveAdminContext(testApplication);

    //    var authStrategies = SetupApplicationAuthorizationStrategies(testApplication);

    //    using var securityContext = TestContext;
    //    var query = new GetAuthStrategiesByApplicationNameQuery(securityContext);
    //    var results = query.Execute(authStrategies.First().Application.ApplicationName).ToArray();

    //    results.Length.ShouldBe(authStrategies.Count);
    //    results.Select(x => x.AuthStrategyName).ShouldBe(authStrategies.Select(x => x.AuthorizationStrategyName), true);

    //}

    //[Test]
    //public void ShouldNotGetAuthStrategiesFromOtherApplications()
    //{
    //    var otherApplication = new Application
    //    {
    //        ApplicationName = "OtherApplication"
    //    };
    //    Save(otherApplication);

    //    var yetAnotherApplication = new Application
    //    {
    //        ApplicationName = "YetAnotherApplication"
    //    };
    //    Save(yetAnotherApplication);

    //    var testApplication = new Application
    //    {
    //        ApplicationName = "TestApplicationName"
    //    };
    //    SaveAdminContext(testApplication);

    //    SetupApplicationAuthorizationStrategies(otherApplication);
    //    SetupApplicationAuthorizationStrategies(yetAnotherApplication);

    //    var authStrategies = SetupApplicationAuthorizationStrategies(testApplication);

    //    using var securityContext = TestContext;

    //    var query = new GetAuthStrategiesByApplicationNameQuery(securityContext);
    //    var results = query.Execute(authStrategies.First().Application.ApplicationName).ToArray();

    //    results.Length.ShouldBe(authStrategies.Count);
    //    results.Select(x => x.AuthStrategyName).ShouldBe(authStrategies.Select(x => x.AuthorizationStrategyName), true);
    //}
}
