// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureCloudOdsDatabaseNameProvider: ICloudOdsDatabaseNameProvider
    {
        public string GetDatabaseName(CloudOdsDatabases cloudOdsDatabase)
        {
            if (cloudOdsDatabase == CloudOdsDatabases.Admin)
                return GetAdminDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.Security)
                return GetSecurityDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.ProductionOds)
                return GetProductionOdsDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.Master)
                return GetMasterDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.MinimalTemplate)
                return GetMinimalTemplateDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.PopulatedTemplate)
                return GetPopulatedTemplateDatabaseName();

            return string.Empty;
        }

        private static string GetAdminDatabaseName()
        {
            return "EdFi_Admin";
        }

        private static string GetSecurityDatabaseName()
        {
            return "EdFi_Security";
        }

        private static string GetProductionOdsDatabaseName()
        {
            return "EdFi_Ods_Production";
        }

        private static string GetMasterDatabaseName()
        {
            return "master";
        }

        private static string GetMinimalTemplateDatabaseName()
        {
            return "EdFi_Ods_Minimal_Template";
        }

        private static string GetPopulatedTemplateDatabaseName()
        {
            return "EdFi_Ods_Populated_Template";
        }
    }
}
