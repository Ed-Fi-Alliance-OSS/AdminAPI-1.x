// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public interface IGetAzureCloudOdsWebsitePerformanceLevelQuery
    {
        Task<AzureWebsitePerformanceLevel> Execute(ICloudOdsOperationContext context);
    }

    public class GetAzureCloudOdsWebsitePerformanceLevelQuery : IGetAzureCloudOdsWebsitePerformanceLevelQuery
    {
        private readonly AzureActiveDirectoryClientInfo _activeDirectoryClientInfo;
        private readonly IGetAzureCloudOdsHostedComponentsQuery _getAzureCloudOdsHostedComponentsQuery;
        private readonly AzureResourceManagerClient _armClient;
        private readonly GetAzureCloudOdsHostedInstanceQuery _getCloudOdsHostedInstanceQuery;

        public GetAzureCloudOdsWebsitePerformanceLevelQuery(AzureActiveDirectoryClientInfo activeDirectoryClientInfo, TokenCache tokenCache, 
            IGetAzureCloudOdsHostedComponentsQuery getAzureCloudOdsHostedComponentsQuery, GetAzureCloudOdsHostedInstanceQuery getCloudOdsHostedInstanceQuery)
        {
            _activeDirectoryClientInfo = activeDirectoryClientInfo;
            _getAzureCloudOdsHostedComponentsQuery = getAzureCloudOdsHostedComponentsQuery;
            _armClient = new AzureResourceManagerClient(activeDirectoryClientInfo, tokenCache);
            _getCloudOdsHostedInstanceQuery = getCloudOdsHostedInstanceQuery;
        }

        public async Task<AzureWebsitePerformanceLevel> Execute(ICloudOdsOperationContext context)
        {
            var websites = await _getAzureCloudOdsHostedComponentsQuery.ExecuteRaw(context);

            if (websites != null && websites.Any())
            {
                var productionApiWebsite = websites.First();
                var serverFarmId = productionApiWebsite.ServerFarmId;
                var instance = await _getCloudOdsHostedInstanceQuery.Execute(context.Instance);
                var website = await _armClient.GetServerFarmByIdAsync(_activeDirectoryClientInfo.SubscriptionId, instance?.SystemName, serverFarmId);
                return new AzureWebsitePerformanceLevel(website.Sku.Tier, website.Sku.Size);
            }

            return null;
        }
    }
}
