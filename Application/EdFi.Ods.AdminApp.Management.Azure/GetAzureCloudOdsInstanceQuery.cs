// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class GetAzureCloudOdsInstanceQuery: IGetCloudOdsInstanceQuery
    {
        private readonly GetAzureCloudOdsHostedInstanceQuery _getAzureCloudOdsHostedInstanceQuery;

        public GetAzureCloudOdsInstanceQuery(GetAzureCloudOdsHostedInstanceQuery getAzureCloudOdsHostedInstanceQuery)
        {
            _getAzureCloudOdsHostedInstanceQuery = getAzureCloudOdsHostedInstanceQuery;
        }

        public async Task<CloudOdsInstance> Execute(string instanceName)
        {
            var instance = await _getAzureCloudOdsHostedInstanceQuery.Execute(instanceName);

            return instance == null ? null : MapAzureCloudOdsInstanceToCloudOdsInstance(instance);
        }

        private CloudOdsInstance MapAzureCloudOdsInstanceToCloudOdsInstance(AzureCloudOdsInstance odsInstance)
        {
            return new CloudOdsInstance
            {
                FriendlyName = odsInstance.FriendlyName,
                Version = odsInstance.Version
            };
        }
    }
}
