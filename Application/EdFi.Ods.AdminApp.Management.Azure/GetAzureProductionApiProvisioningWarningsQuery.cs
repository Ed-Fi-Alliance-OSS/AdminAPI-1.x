// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class GetAzureProductionApiProvisioningWarningsQuery : IGetProductionApiProvisioningWarningsQuery
    {
        private readonly AzureWebsitePerformanceLevel _expectedWebsitePerformanceLevel = AzureWebsitePerformanceLevel.S3;

        private readonly IGetAzureCloudOdsWebsitePerformanceLevelQuery _getAzureCloudOdsWebsitePerformanceLevelQuery;

        public GetAzureProductionApiProvisioningWarningsQuery(
            IGetAzureCloudOdsWebsitePerformanceLevelQuery getAzureCloudOdsWebsitePerformanceLevelQuery)
        {
            _getAzureCloudOdsWebsitePerformanceLevelQuery = getAzureCloudOdsWebsitePerformanceLevelQuery;
        }

        [Obsolete("This operation is no longer intended to be reached, and should be phased out.")]
        public async Task<ProductionApiProvisioningWarnings> Execute(CloudOdsInstance cloudOdsInstance)
        {
            var result = new ProductionApiProvisioningWarnings
            {
                ResolutionUrl = "https://portal.azure.com"
            };

            var warnings = new List<string>();

            var cloudOdsApiOperationContext = new CloudOdsApiOperationContext(cloudOdsInstance);
            var serverPerformanceLevel = await _getAzureCloudOdsWebsitePerformanceLevelQuery.Execute(cloudOdsApiOperationContext);

            if (serverPerformanceLevel != null && serverPerformanceLevel < _expectedWebsitePerformanceLevel)
            {
                warnings.Add($"Azure Website performance tier '{_expectedWebsitePerformanceLevel.ServiceObjective}' or above is recommended");
            }

            result.Warnings = warnings;
            return result;
        }
    }
}
