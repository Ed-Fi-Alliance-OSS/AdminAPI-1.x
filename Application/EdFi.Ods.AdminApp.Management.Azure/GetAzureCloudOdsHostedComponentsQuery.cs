// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public interface IGetAzureCloudOdsHostedComponentsQuery : IGetCloudOdsHostedComponentsQuery
    {
        Task<IEnumerable<Site>> ExecuteRaw(ICloudOdsOperationContext context);
    }

    public class GetAzureCloudOdsHostedComponentsQuery : IGetAzureCloudOdsHostedComponentsQuery
    {
        private readonly AzureActiveDirectoryClientInfo _activeDirectoryClientInfo;
        private readonly AzureResourceManagerClient _armClient;
        private readonly GetAzureCloudOdsHostedInstanceQuery _getCloudOdsHostedInstanceQuery;

        public GetAzureCloudOdsHostedComponentsQuery(AzureActiveDirectoryClientInfo activeDirectoryClientInfo, TokenCache tokenCache, GetAzureCloudOdsHostedInstanceQuery getCloudOdsHostedInstanceQuery)
        {
            _activeDirectoryClientInfo = activeDirectoryClientInfo;
            _armClient = new AzureResourceManagerClient(activeDirectoryClientInfo, tokenCache);
            _getCloudOdsHostedInstanceQuery = getCloudOdsHostedInstanceQuery;
        }

        public async Task<IEnumerable<CloudOdsWebsite>> Execute(ICloudOdsOperationContext context)
        {
            var websites = await ExecuteRaw(context);
            return websites.Select(w => AzureModelExtensions.ToCloudOdsWebsite(w));
        }

        public async Task<IEnumerable<Site>> ExecuteRaw(ICloudOdsOperationContext context)
        {
            var instance = await _getCloudOdsHostedInstanceQuery.Execute(context.Instance);
            var websites = await _armClient.GetWebsiteListAsync(_activeDirectoryClientInfo.SubscriptionId, instance?.SystemName);

            return websites.Where(w =>
                w.IsInEnvironment(CloudOdsEnvironment.Production) &&
                (context.TargetRole == null || w.IsInRole(context.TargetRole)));
        }

        public async Task<IEnumerable<OdsComponent>> Execute(CloudOdsInstance instance)
        {
            var cloudOdsOperationContext = new CloudOdsOperationContext
            {
                Instance = instance
            };

            var websites = await Execute(cloudOdsOperationContext);

            return websites.Select(w => new OdsComponent
            {
                Name = w.Role.DisplayName,
                Environment = w.Environment.DisplayName,
                Url = w.Url,
                Version = instance.Version
            });
        }
    }
}
