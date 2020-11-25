// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management
{
    public static class CloudOdsApplicationName
    {
        public static string GetDisplayName(string applicationName)
        {
            return applicationName.Replace("Production-", "").Replace("Staging-", "");
        }

        public static string GetPersistedNamePrefix()
        {
            var environment = CloudOdsEnvironment.Production;
            return $"{environment.DisplayName}-";
        }

        public static string GetPersistedName(string applicationName)
        {
            var environmentPrefix = GetPersistedNamePrefix();
            return applicationName.StartsWith(environmentPrefix) 
                ? applicationName 
                : $"{environmentPrefix}{applicationName}";
        }
    }
}
