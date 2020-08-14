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
    public class GetAzureCloudOdsPerformanceLevelQueryTester : AzureIntegrationTestBase
    {
        [Test]
        public async Task ShouldExecute()
        {
            var getAzureCloudOdsHostedInstanceQuery = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var websiteQuery = new GetAzureCloudOdsHostedComponentsQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache, getAzureCloudOdsHostedInstanceQuery);
            var performanceQuery = new GetAzureCloudOdsWebsitePerformanceLevelQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache, websiteQuery, getAzureCloudOdsHostedInstanceQuery);

            var context = new CloudOdsOperationContext
            {
                Instance = DefaultTestCloudOdsInstance,
                TargetEnvironment = CloudOdsEnvironment.Production,
                TargetRole = CloudOdsRole.Api
            };

            var result = await performanceQuery.Execute(context);
            result.Equals(AzureWebsitePerformanceLevel.Free).ShouldBeTrue();
        }
    }
}
