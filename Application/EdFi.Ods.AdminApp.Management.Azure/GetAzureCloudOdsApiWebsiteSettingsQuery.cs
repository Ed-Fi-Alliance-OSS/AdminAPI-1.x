// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Settings;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class GetAzureCloudOdsApiWebsiteSettingsQuery : IGetCloudOdsApiWebsiteSettingsQuery
    {
        private readonly AzureActiveDirectoryClientInfo _azureActiveDirectoryClientInfo;
        private readonly IGetAzureCloudOdsHostedComponentsQuery _getAzureCloudOdsHostedComponentsQuery;
        private readonly AzureResourceManagerClient _armClient;
        private readonly GetAzureCloudOdsHostedInstanceQuery _getCloudOdsHostedInstanceQuery;

        public GetAzureCloudOdsApiWebsiteSettingsQuery(AzureActiveDirectoryClientInfo azureActiveDirectoryClientInfo, IGetAzureCloudOdsHostedComponentsQuery getAzureCloudOdsHostedComponentsQuery, 
            TokenCache tokenCache, GetAzureCloudOdsHostedInstanceQuery getCloudOdsHostedInstanceQuery)
        {
            _azureActiveDirectoryClientInfo = azureActiveDirectoryClientInfo;
            _getAzureCloudOdsHostedComponentsQuery = getAzureCloudOdsHostedComponentsQuery;
            _armClient = new AzureResourceManagerClient(azureActiveDirectoryClientInfo, tokenCache);
            _getCloudOdsHostedInstanceQuery = getCloudOdsHostedInstanceQuery;
        }

        public async Task<CloudOdsApiWebsiteSettings> Execute(CloudOdsApiOperationContext context)
        {
            var settings = await ExecuteRaw(context);
            return new CloudOdsApiWebsiteSettings(settings.Properties);
        }

        public async Task<StringDictionary> ExecuteRaw(CloudOdsApiOperationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.TargetEnvironment == null)
                throw new InvalidOperationException("You must provide a TargetEnvironment for this operation");
            
            var website = Enumerable.Single<CloudOdsWebsite>((await _getAzureCloudOdsHostedComponentsQuery.Execute(context)));
            var instance = await _getCloudOdsHostedInstanceQuery.Execute(context.Instance);
            var settings = await _armClient.GetWebsiteSettingsAsync(_azureActiveDirectoryClientInfo.SubscriptionId, instance?.SystemName, website.SystemName);

            return settings;
        }
    }
}
