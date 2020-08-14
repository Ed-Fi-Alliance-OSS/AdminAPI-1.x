// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Settings;
using Microsoft.Azure.Management.WebSites;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    [TestFixture]
    public class GetAzureCloudOdsApiWebsiteSettingsQueryTester : AzureIntegrationTestBase
    {
        [Test]
        public async Task ShouldExecuteForProduction()
        {
            await ShouldExecute(ProductionApiOperationContext);
        }

        private async Task ShouldExecute(CloudOdsApiOperationContext context)
        {
            var token = await GetAccessTokenCredentialsAsync();
            
            var site = await GetApiWebsiteAsync(context.TargetEnvironment);

            var client = new WebSiteManagementClient(token)
            {
                SubscriptionId = DefaultAzureActiveDirectoryClientInfo.SubscriptionId
            };
            var settings = await client.Sites.ListSiteAppSettingsAsync(DefaultTestCloudOdsInstance.SystemName, site.Name);
            settings.Properties[CloudOdsApiWebsiteSettings.BearerTokenTimeoutSettingName] = "20";
            settings.Properties[CloudOdsApiWebsiteSettings.LogLevelSettingName] = LogLevel.Warn.DisplayName;

            await client.Sites.UpdateSiteAppSettingsAsync(DefaultTestCloudOdsInstance.SystemName, site.Name, settings);

            var getAzureCloudOdsHostedInstanceQuery = new GetAzureCloudOdsHostedInstanceQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache);
            var getWebsiteQuery = new GetAzureCloudOdsHostedComponentsQuery(DefaultAzureActiveDirectoryClientInfo, DefaultTokenCache, getAzureCloudOdsHostedInstanceQuery);
            var query = new GetAzureCloudOdsApiWebsiteSettingsQuery(DefaultAzureActiveDirectoryClientInfo,
                getWebsiteQuery, DefaultTokenCache, getAzureCloudOdsHostedInstanceQuery);

            var result = await query.Execute(context);
            result.BearerTokenTimeoutInMinutes.ShouldBe(20);
            result.LogLevel.ShouldBe(LogLevel.Warn);
        }
    }
}
