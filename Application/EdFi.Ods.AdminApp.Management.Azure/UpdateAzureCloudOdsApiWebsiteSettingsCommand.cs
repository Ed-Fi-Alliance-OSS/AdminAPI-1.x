// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Settings;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class UpdateAzureCloudOdsApiWebsiteSettingsCommand : IUpdateCloudOdsApiWebsiteSettingsCommand
    {
        private readonly AzureActiveDirectoryClientInfo _azureActiveDirectoryClientInfo;
        private readonly IGetAzureCloudOdsHostedComponentsQuery _getAzureCloudOdsHostedComponentsQuery;
        private readonly GetAzureCloudOdsApiWebsiteSettingsQuery _getAzureCloudOdsComponentApiWebsiteSettingsQuery;
        private readonly AzureResourceManagerClient _armClient;
        private readonly GetAzureCloudOdsHostedInstanceQuery _getCloudOdsHostedInstanceQuery;

        public UpdateAzureCloudOdsApiWebsiteSettingsCommand(AzureActiveDirectoryClientInfo azureActiveDirectoryClientInfo, IGetAzureCloudOdsHostedComponentsQuery getAzureCloudOdsHostedComponentsQuery,
            IGetCloudOdsApiWebsiteSettingsQuery getAzureCloudOdsComponentApiWebsiteSettingsQuery, TokenCache tokenCache, GetAzureCloudOdsHostedInstanceQuery getCloudOdsHostedInstanceQuery)
        {
            _azureActiveDirectoryClientInfo = azureActiveDirectoryClientInfo;
            _getAzureCloudOdsHostedComponentsQuery = getAzureCloudOdsHostedComponentsQuery;
            _getAzureCloudOdsComponentApiWebsiteSettingsQuery = (GetAzureCloudOdsApiWebsiteSettingsQuery) getAzureCloudOdsComponentApiWebsiteSettingsQuery;
            _armClient = new AzureResourceManagerClient(azureActiveDirectoryClientInfo, tokenCache);
            _getCloudOdsHostedInstanceQuery = getCloudOdsHostedInstanceQuery;
        }

        public async Task Execute(CloudOdsApiOperationContext context, CloudOdsApiWebsiteSettings newSettings)
        {
            var website = (await _getAzureCloudOdsHostedComponentsQuery.Execute(context)).Single();
            var azureSettings = await _getAzureCloudOdsComponentApiWebsiteSettingsQuery.ExecuteRaw(context);

            foreach (var setting in newSettings.AsDictionary())
            {
                azureSettings.Properties[setting.Key] = setting.Value;
            }
            var instance = await _getCloudOdsHostedInstanceQuery.Execute(context.Instance);
            await _armClient.SaveWebsiteSettingsAsync(_azureActiveDirectoryClientInfo.SubscriptionId, instance?.SystemName, website.SystemName, azureSettings);
        }
    }
}
