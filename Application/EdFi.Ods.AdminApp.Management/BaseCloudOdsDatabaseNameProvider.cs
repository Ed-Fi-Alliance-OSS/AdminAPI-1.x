// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management
{
    public class BaseCloudOdsDatabaseNameProvider
    {
        public string GetAdminDatabaseName()
        {
            return "EdFi_Admin";
        }

        public string GetSecurityDatabaseName()
        {
            return "EdFi_Security";
        }

        public string GetProductionOdsDatabaseName()
        {
            return "EdFi_Ods_Production";
        }

        public string GetMasterDatabaseName()
        {
            return "master";
        }

        public string GetMinimalTemplateDatabaseName()
        {
            return "EdFi_Ods_Minimal_Template";
        }

        public string GetPopulatedTemplateDatabaseName()
        {
            return "EdFi_Ods_Populated_Template";
        }
    }
}