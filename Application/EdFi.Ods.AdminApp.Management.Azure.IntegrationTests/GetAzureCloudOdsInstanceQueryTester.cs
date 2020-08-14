// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    [TestFixture]
    public class GetAzureCloudOdsInstanceQueryTester : AzureIntegrationTestBase
    {
        [Test]
        public async Task ShouldMatchExpectedIntegrationTestValues()
        {
            var hostedInstanceQuery = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var query = new GetAzureCloudOdsInstanceQuery(hostedInstanceQuery);
            var instance = await query.Execute(DefaultTestCloudOdsInstance.FriendlyName);

            ValidateAgainstDefaultTestInstance(instance);
        }

        private void ValidateAgainstDefaultTestInstance(CloudOdsInstance test)
        {
            test.FriendlyName.ShouldBe(DefaultTestCloudOdsInstance.FriendlyName);
            test.Version.ShouldBe(DefaultTestCloudOdsInstance.Version);
        }
    }
}
