// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureResourceManagerClient
    {
        private readonly AzureActiveDirectoryClientInfo _clientInfo;
        private readonly TokenCache _tokenCache;

        public AzureResourceManagerClient(AzureActiveDirectoryClientInfo clientInfo, TokenCache tokenCache)
        {
            _clientInfo = clientInfo;
            _tokenCache = tokenCache;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var clientCredentials = new ClientCredential(_clientInfo.ClientId, _clientInfo.ClientSecret);
            var authContext = new AuthenticationContext(_clientInfo.Authority, _tokenCache);

            var result = await authContext.AcquireTokenAsync(AzureResourceManagerClientInfo.Default.ResourceManagerIdentifier, clientCredentials).ConfigureAwait(false);
            return result.AccessToken;
        }

        public async Task<TokenCredentials> GetAccessTokenCredentialsAsync()
        {
            var token = await GetAccessTokenAsync();
            return new TokenCredentials(token);
        }

        public async Task<IEnumerable<ResourceGroup>> GetResourceGroupListAsync(string subscriptionId)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new ResourceManagementClient(token) {SubscriptionId = subscriptionId};
            var groups = await client.ResourceGroups.ListAsync().ConfigureAwait(false);

            return groups;
        }

        public async Task<IEnumerable<Site>> GetWebsiteListAsync(string subscriptionId, string resourceGroupName)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) {SubscriptionId = subscriptionId};
            var sites = await client.Sites.GetSitesAsync(resourceGroupName).ConfigureAwait(false);

            return sites.Value;
        }

        public async Task<StringDictionary> GetWebsiteSettingsAsync(string subscriptionId, string resourceGroupName, string websiteName)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) { SubscriptionId = subscriptionId };
            var settings = await client.Sites.ListSiteAppSettingsAsync(resourceGroupName, websiteName).ConfigureAwait(false);

            return settings;
        }

        public async Task SaveWebsiteSettingsAsync(string subscriptionId, string resourceGroupName, string websiteName, StringDictionary appSettings)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) { SubscriptionId =  subscriptionId };
            await client.Sites.UpdateSiteAppSettingsAsync(resourceGroupName, websiteName, appSettings).ConfigureAwait(false);
        }

        public async Task<ServerFarmCollection> GetServerFarmsAsync(string subscriptionId, string resourceGroupName)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) { SubscriptionId = subscriptionId };

            var serverFarms = await client.ServerFarms.GetServerFarmsAsync(resourceGroupName);
            return serverFarms;
        }

        public async Task<ServerFarmWithRichSku> GetServerFarmByIdAsync(string subscriptionId, string resourceGroupName, string serverFarmId)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) { SubscriptionId = subscriptionId };
            var serverFarms = await client.ServerFarms.GetServerFarmsAsync(resourceGroupName);

            return serverFarms.Value.FirstOrDefault(f => f.Id == serverFarmId);
        }

        public async Task RestartAppService(string subscriptionId, string resourceGroupName, string websiteName)
        {
            var token = await GetAccessTokenCredentialsAsync();
            var client = new WebSiteManagementClient(token) { SubscriptionId = subscriptionId };
            await client.Sites.RestartSiteAsync(resourceGroupName, websiteName).ConfigureAwait(false);
        }
    }
}
