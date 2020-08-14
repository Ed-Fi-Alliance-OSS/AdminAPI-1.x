// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class GetAzureCloudOdsHostedInstanceQuery
    {
        private readonly AzureActiveDirectoryClientInfo _activeDirectoryClientInfo;
        private readonly AzureResourceManagerClient _armClient;
        
        public GetAzureCloudOdsHostedInstanceQuery(AzureActiveDirectoryClientInfo activeDirectoryClientInfo, TokenCache tokenCache)
        {
            _activeDirectoryClientInfo = activeDirectoryClientInfo;
            _armClient = new AzureResourceManagerClient(activeDirectoryClientInfo, tokenCache);
        }

        public async Task<IEnumerable<AzureCloudOdsInstance>> Execute()
        {
            var resourceGroups =
                await _armClient.GetResourceGroupListAsync(_activeDirectoryClientInfo.SubscriptionId);

            return resourceGroups
                .Where(IsCloudOdsResourceGroup)
                .Select(rg => new AzureCloudOdsInstance
            {
                Edition = rg.Tags[CloudOdsTags.Edition],
                FriendlyName = rg.Tags[CloudOdsTags.FriendlyName],
                SystemId = rg.Id,
                SystemName = rg.Name,
                Version = rg.Tags[CloudOdsTags.Version]
            });
        }

        public async Task<AzureCloudOdsInstance> Execute(string friendlyName)
        {
            var instances = await Execute();
            return instances.SingleOrDefault(i => i.FriendlyName.Equals(friendlyName, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<AzureCloudOdsInstance> Execute(CloudOdsInstance instance)
        {
            return await Execute(instance.FriendlyName);
        }

        private bool IsCloudOdsResourceGroup(ResourceGroup resourceGroup)
        {
            return resourceGroup.Tags != null &&
                   resourceGroup.Tags.ContainsKey(CloudOdsTags.Version) &&
                   resourceGroup.Tags.ContainsKey(CloudOdsTags.FriendlyName) &&
                   resourceGroup.Tags.ContainsKey(CloudOdsTags.Edition);
        }
    }
}
