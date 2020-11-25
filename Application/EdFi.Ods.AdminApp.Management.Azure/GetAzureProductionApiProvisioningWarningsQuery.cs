// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class GetAzureProductionApiProvisioningWarningsQuery : IGetProductionApiProvisioningWarningsQuery
    {
        private readonly AzureSqlDatabasePerformanceLevel _expectedDatabasePerformanceLevel = AzureSqlDatabasePerformanceLevel.P1;
        private readonly AzureWebsitePerformanceLevel _expectedWebsitePerformanceLevel = AzureWebsitePerformanceLevel.S3;

        private readonly AzureDatabaseManagementService _azureDatabaseManagementService;
        private readonly IGetAzureCloudOdsWebsitePerformanceLevelQuery _getAzureCloudOdsWebsitePerformanceLevelQuery;
        private readonly IRawSqlConnectionService _rawSqlConnectionService;
        private readonly ICloudOdsDatabaseNameProvider _cloudOdsDatabaseNameProvider;

        public GetAzureProductionApiProvisioningWarningsQuery(AzureDatabaseManagementService azureDatabaseManagementService, 
            IGetAzureCloudOdsWebsitePerformanceLevelQuery getAzureCloudOdsWebsitePerformanceLevelQuery,
            IRawSqlConnectionService rawSqlConnectionService,
            ICloudOdsDatabaseNameProvider cloudOdsDatabaseNameProvider)
        {
            _azureDatabaseManagementService = azureDatabaseManagementService;
            _getAzureCloudOdsWebsitePerformanceLevelQuery = getAzureCloudOdsWebsitePerformanceLevelQuery;
            _rawSqlConnectionService = rawSqlConnectionService;
            _cloudOdsDatabaseNameProvider = cloudOdsDatabaseNameProvider;
        }

        public async Task<ProductionApiProvisioningWarnings> Execute(CloudOdsInstance cloudOdsInstance)
        {
            var result = new ProductionApiProvisioningWarnings
            {
                ResolutionUrl = "https://portal.azure.com"
            };

            var warnings = new List<string>();

            using (var conn = _rawSqlConnectionService.GetDatabaseConnectionFromConfigFile(_cloudOdsDatabaseNameProvider.GetDatabaseName(CloudOdsDatabases.ProductionOds)))
            {
                var sqlPerformanceLevel = _azureDatabaseManagementService.GetDatabasePerformanceLevel(conn, CloudOdsDatabaseNames.ProductionOds);

                //we have to be careful here not to output a warning in case the user is not running against an Azure Sql instance, hence the validity check
                if (sqlPerformanceLevel.IsValid() && sqlPerformanceLevel < _expectedDatabasePerformanceLevel)
                {
                    warnings.Add($"Azure Sql performance tier '{_expectedDatabasePerformanceLevel.ServiceObjective}' or above is recommended");
                }
            }

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
