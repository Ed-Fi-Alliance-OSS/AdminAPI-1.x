// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    public abstract class AzureIntegrationTestBase
    {
        protected AzureActiveDirectoryClientInfo DefaultAzureActiveDirectoryClientInfo => AzureTestSettingsProvider.DefaultAzureActiveDirectoryClientInfo;

        protected TokenCache DefaultTokenCache => TokenCache.DefaultShared;

        //Note that these values are hard-coded for an Azure Resource Group named "Cloud Integration Tests"
        protected AzureCloudOdsInstance DefaultTestCloudOdsInstance => AzureTestSettingsProvider.DefaultTestCloudOdsInstance;

        protected CloudOdsApiOperationContext ProductionApiOperationContext => AzureTestSettingsProvider.ProductionApiOperationContext;

        public async Task<TokenCredentials> GetAccessTokenCredentialsAsync()
        {
            return await GetAccessTokenCredentialsAsync(DefaultAzureActiveDirectoryClientInfo);
        }

        public async Task<TokenCredentials> GetAccessTokenCredentialsAsync(AzureActiveDirectoryClientInfo clientInfo)
        {            
            var clientCredentials = new ClientCredential(clientInfo.ClientId, clientInfo.ClientSecret);
            var authContext = new AuthenticationContext(clientInfo.Authority, DefaultTokenCache);

            var result = await authContext.AcquireTokenAsync("https://management.azure.com/", clientCredentials).ConfigureAwait(false);
            return new TokenCredentials(result.AccessToken);
        }

        public async Task<IEnumerable<Site>> GetWebsitesAsync()
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) { SubscriptionId = DefaultAzureActiveDirectoryClientInfo.SubscriptionId };
            var sites = await client.Sites.GetSitesAsync(DefaultTestCloudOdsInstance.SystemName).ConfigureAwait(false);

            return sites.Value;
        }

        public async Task<Site> GetApiWebsiteAsync(CloudOdsEnvironment environment)
        {
            if (environment == CloudOdsEnvironment.Production)
            {
                return await GetProductionApiWebsiteAsync();
            }

            throw new NotSupportedException("Unsupported value for CloudOdsInstanceManagementTarget");
        }

        public async Task<Site> GetProductionApiWebsiteAsync()
        {
            return (await GetWebsitesAsync()).Single(s => AzureModelExtensions.IsProductionApi(s));
        }
    }
}