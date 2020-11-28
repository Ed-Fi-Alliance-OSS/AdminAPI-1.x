// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using static EdFi.Ods.AdminApp.Management.Azure.IntegrationTests.Testing;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        EnsureInitialized();

        var subscriptionId = Scoped<IOptions<AppSettings>, string>(appSettings => appSettings.Value.IdaSubscriptionId);

        if (string.IsNullOrEmpty(subscriptionId))
            Assert.Ignore("See the README in this test project for guidance on running Azure Integration Tests against a test Cloud ODS deployment.");
    }
}
