// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    [TestFixture]
    public class GetAzureCloudOdsHostedComponentsQueryTester : AzureIntegrationTestBase
    {
        [Test]
        public async Task ShouldGetAllWebsites()
        {
            var getAzureCloudOdsHostedInstanceQuery = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var query = new GetAzureCloudOdsHostedComponentsQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache, getAzureCloudOdsHostedInstanceQuery);
            var context = new CloudOdsOperationContext
            {
                Instance = DefaultTestCloudOdsInstance,
                TargetEnvironment = null,
                TargetRole = null
            };

            var result = await query.Execute(context);
            result.Count().ShouldBe(3);
            result.Count(w => w.IsProductionApi()).ShouldBe(1);
            result.Count(w => w.IsAdminApp()).ShouldBe(1);
            result.Count(w => w.IsSwaggerWebsite()).ShouldBe(1);
        }

        [Test]
        public async Task ShouldGetAllProductionSites()
        {
            var getAzureCloudOdsHostedInstanceQuery = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var query = new GetAzureCloudOdsHostedComponentsQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache, getAzureCloudOdsHostedInstanceQuery);
            var context = new CloudOdsOperationContext
            {
                Instance = DefaultTestCloudOdsInstance,
                TargetEnvironment = CloudOdsEnvironment.Production,
                TargetRole = null
            };

            var result = await query.Execute(context);
            result.Count().ShouldBe(3);
            result.Count(w => w.IsProductionApi()).ShouldBe(1);
            result.Count(w => w.IsAdminApp()).ShouldBe(1);
            result.Count(w => w.IsSwaggerWebsite()).ShouldBe(1);
        }

        [Test]
        public async Task ShouldGetAllApiSites()
        {
            var getAzureCloudOdsHostedInstanceQuery = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var query = new GetAzureCloudOdsHostedComponentsQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache, getAzureCloudOdsHostedInstanceQuery);
            var context = new CloudOdsOperationContext
            {
                Instance = DefaultTestCloudOdsInstance,
                TargetEnvironment = null,
                TargetRole = CloudOdsRole.Api
            };

            var result = await query.Execute(context);
            result.Count().ShouldBe(1);
            result.Count(w => w.IsProductionApi()).ShouldBe(1);
        }
    }
}
