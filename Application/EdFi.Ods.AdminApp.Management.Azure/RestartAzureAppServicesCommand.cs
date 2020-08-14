// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.Common.Utils.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class RestartAzureAppServicesCommand : IRestartAppServicesCommand
    {
        private readonly AzureActiveDirectoryClientInfo _activeDirectoryClientInfo;
        private readonly AzureResourceManagerClient _armClient;
        private readonly GetAzureCloudOdsHostedInstanceQuery _getCloudOdsHostedInstanceQuery;
        private readonly IGetAzureCloudOdsHostedComponentsQuery _getAzureCloudOdsHostedComponentsQuery;

        public RestartAzureAppServicesCommand(AzureActiveDirectoryClientInfo azureActiveDirectoryClientInfo
            , GetAzureCloudOdsHostedInstanceQuery getCloudOdsHostedInstanceQuery
            , IGetAzureCloudOdsHostedComponentsQuery getAzureCloudOdsHostedComponentsQuery
            , TokenCache tokenCache)
        {
            _activeDirectoryClientInfo = azureActiveDirectoryClientInfo;
            _armClient = new AzureResourceManagerClient(azureActiveDirectoryClientInfo, tokenCache);
            _getCloudOdsHostedInstanceQuery = getCloudOdsHostedInstanceQuery;
            _getAzureCloudOdsHostedComponentsQuery = getAzureCloudOdsHostedComponentsQuery;
        }

        public async Task Execute(ICloudOdsOperationContext context)
        {
            var cloudOdsWebsites = await _getAzureCloudOdsHostedComponentsQuery.Execute(context);
            var instance = await _getCloudOdsHostedInstanceQuery.Execute(context.Instance);
            foreach (var cloudOdsWebsite in cloudOdsWebsites)
            {
                await _armClient.RestartAppService(_activeDirectoryClientInfo.SubscriptionId, instance.SystemName, cloudOdsWebsite.SystemName);
            }
        }
    }
}
