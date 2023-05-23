// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using EdFi.Ods.Admin.Api.Infrastructure.Api;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.Admin.Api.Infrastructure.UnitTests.Api;

[TestFixture]
public class OdsSecurityVersionResolverTests
{
    public static void ShouldReturnV3_5ForOdsV3() => new OdsSecurityVersionResolver(
        new StubValidApi("3.2.0"), "").DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.ThreeThroughFive);

    public static void ShouldReturnV3_5ForOdsV51() => new OdsSecurityVersionResolver(
        new StubValidApi("5.1"), "").DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.ThreeThroughFive);

    public static void ShouldReturnV3_5ForOdsV53() => new OdsSecurityVersionResolver(
        new StubValidApi("5.3"), "").DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.ThreeThroughFive);

    public static void ShouldReturnV6ForOds6() => new OdsSecurityVersionResolver(
        new StubValidApi("6.0"), "").DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.Six);

    public static void ShouldReturnV6ForOdsGreaterThanV6() => new OdsSecurityVersionResolver(
        new StubValidApi("6.1"), "").DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.Six);

    public static void ShouldReturnV6ForOdsMuchGreaterThanV6() => new OdsSecurityVersionResolver(
        new StubValidApi("10.1"), "").DetermineSecurityModel().ShouldBe(EdFiOdsSecurityModelCompatibility.Six);

    public static void ShouldThrowExceptionWhenValidApiReturnsNoVersion()
    {
        Should.Throw<Exception>(
            () => new OdsSecurityVersionResolver(new StubValidApi(null), "").DetermineSecurityModel());
    }

    public static void ShouldThrowMatchingExceptionWhenValidationFails()
    {
        Should.Throw<JsonException>(
            () => new OdsSecurityVersionResolver(new StubInvalidApi(), "").DetermineSecurityModel());
    }

    private class StubValidApi : IOdsApiValidator
    {
        private readonly Version _version;

        public StubValidApi(string version)
        {
            _version = version == null ? null : new Version(version);
        }
        public Task<OdsApiValidatorResult> Validate(string apiServerUrl)
            => Task.FromResult(new OdsApiValidatorResult { IsValidOdsApi = true, Version = _version, });
    }

    private class StubInvalidApi : IOdsApiValidator
    {
        public Task<OdsApiValidatorResult> Validate(string apiServerUrl)
            => Task.FromResult(new OdsApiValidatorResult { IsValidOdsApi = false, Exception = new JsonException(), });
    }
}
