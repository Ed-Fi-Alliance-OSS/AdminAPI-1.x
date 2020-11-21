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
                return "EdFi_Admin";

            if (cloudOdsDatabase == CloudOdsDatabases.Security)
                return "EdFi_Security";

            if (cloudOdsDatabase == CloudOdsDatabases.ProductionOds)
                return "EdFi_Ods_Production";

            if (cloudOdsDatabase == CloudOdsDatabases.Master)
                return "master";

            if (cloudOdsDatabase == CloudOdsDatabases.MinimalTemplate)
                return "EdFi_Ods_Minimal_Template";

            if (cloudOdsDatabase == CloudOdsDatabases.PopulatedTemplate)
                return "EdFi_Ods_Populated_Template";

            return string.Empty;
        }
    }
}
