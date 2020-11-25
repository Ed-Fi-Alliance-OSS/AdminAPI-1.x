// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    [TestFixture]
    public class GetAzureCloudOdsHostedInstanceQueryTester : AzureIntegrationTestBase
    {
        [Test]
        public async Task ShouldMatchExpectedIntegrationTestValues()
        {
            var query = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var instances = await query.Execute();
            var instance = instances.Single(i => i.FriendlyName.Equals(DefaultTestCloudOdsInstance.FriendlyName, StringComparison.InvariantCultureIgnoreCase));

            ValidateAgainstDefaultTestInstance(instance);
        }

        [Test]
        public async Task ShouldFindSpecificIntegrationTestInstance()
        {
            var query = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var instance = await query.Execute(DefaultTestCloudOdsInstance.FriendlyName);

            ValidateAgainstDefaultTestInstance(instance);
        }

        //Note that this test is marked as explicit because for it to run properly,
        //the integration test app has to be given Contributor access on the test account.
        //
        //In practice, as long as the test Azure account already has a non-Cloud ODS resource group,
        //the above tests will validate that non-Cloud ODS resource groups are ignored by this query.
        [Ignore("This test is intended to be marked [Explicit], but a " +
                "VS2019 Test Explorer bug prevents that attribute from " +
                "functioning as intended. To run this tests, temporarily" +
                "comment out this [Ignore(...)] attribute.")]
        [Test]
        public async Task ShouldIgnoreNonOdsResourceGroups()
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new ResourceManagementClient(token)
            {
                SubscriptionId = DefaultAzureActiveDirectoryClientInfo.SubscriptionId
            };

            var testResourceGroupName = $"cloud_ods_integration_test_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
 
            var query = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var instances = await query.Execute();

            instances.Any(i => i.SystemName.Equals(testResourceGroupName, StringComparison.InvariantCultureIgnoreCase)).ShouldBeFalse();

            await client.ResourceGroups.DeleteAsync(testResourceGroupName);
        }

        private void ValidateAgainstDefaultTestInstance(AzureCloudOdsInstance test)
        {
            test.SystemName.ShouldBe(DefaultTestCloudOdsInstance.SystemName);
            test.FriendlyName.ShouldBe(DefaultTestCloudOdsInstance.FriendlyName);
            test.Edition.ShouldBe(DefaultTestCloudOdsInstance.Edition);
            test.SystemId.ShouldBe(DefaultTestCloudOdsInstance.SystemId);
            test.Version.ShouldBe(DefaultTestCloudOdsInstance.Version);
        }
    }
}
