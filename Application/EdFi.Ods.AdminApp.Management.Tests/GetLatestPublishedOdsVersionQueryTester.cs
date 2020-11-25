// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public class GetLatestPublishedOdsVersionQueryTester
    {
        [Test]
        public void ShouldRetrieveVersionNumberSuccessfully()
        {
            var command = new GetLatestPublishedOdsVersionQuery();
            var result = command.Execute();

            result.ShouldNotBeEmpty();

            Version version;
            Version.TryParse(result, out version).ShouldBeTrue();
        }
    }
}
