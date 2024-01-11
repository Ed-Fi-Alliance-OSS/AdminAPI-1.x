// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.Infrastructure.UnitTests.Api;

[TestFixture]
public class OdsSecurityVersionResolverTests
{
    [Test]
    public static void ShouldReturnV3_5ForOdsV53() => new OdsSecurityVersionResolver("5.3")
        .DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.ThreeThroughFive);

    [Test]
    public static void ShouldReturnV3_5ForOdsV53Cqe() => new OdsSecurityVersionResolver("5.3cqe")
        .DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.FiveThreeCqe);

    [Test]
    public static void ShouldReturnV6ForOds6() => new OdsSecurityVersionResolver("6.0")
        .DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.Six);

    [Test]
    public static void ShouldReturnV6ForOdsGreaterThanV6() => new OdsSecurityVersionResolver("6.1")
        .DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.Six);

    [Test]
    public static void ShouldThrowExceptionForOdsV3()
    {
        Should.Throw<Exception>(() => new OdsSecurityVersionResolver("3.2.0").DetermineSecurityModel());
    }

    [Test]
    public static void ShouldThrowExceptionForOdsV51()
    {
        Should.Throw<Exception>(() => new OdsSecurityVersionResolver("5.1").DetermineSecurityModel());
    }

    [Test]
    public static void ShouldThrowExceptionForOdsMuchGreaterThanV6()
    {
        Should.Throw<Exception>(() => new OdsSecurityVersionResolver("10.1").DetermineSecurityModel());
    }

    [Test]
    public static void ShouldThrowExceptionWhenValidApiReturnsNoVersion()
    {
        Should.Throw<Exception>(() => new OdsSecurityVersionResolver(string.Empty).DetermineSecurityModel());
    }
}
