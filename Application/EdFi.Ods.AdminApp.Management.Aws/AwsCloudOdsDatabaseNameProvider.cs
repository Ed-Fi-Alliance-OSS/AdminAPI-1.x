// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Aws
{
    [System.Obsolete("Review usages.")]
    public class AwsCloudOdsDatabaseNameProvider : BaseCloudOdsDatabaseNameProvider, ICloudOdsDatabaseNameProvider
    {
        public string GetDatabaseName(CloudOdsDatabases cloudOdsDatabase)
        {
            if (cloudOdsDatabase == CloudOdsDatabases.Admin)
                return GetAdminDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.Security)
                return GetSecurityDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.ProductionOds)
                return "EdFi_Ods";

            if (cloudOdsDatabase == CloudOdsDatabases.Master)
                return GetMasterDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.MinimalTemplate)
                return GetMinimalTemplateDatabaseName();

            if (cloudOdsDatabase == CloudOdsDatabases.PopulatedTemplate)
                return GetPopulatedTemplateDatabaseName();

            return string.Empty;
        }
    }
}
