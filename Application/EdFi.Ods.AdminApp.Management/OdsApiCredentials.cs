// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management
{
    public class OdsApiCredential
    {
        public OdsApiCredential()
        {
        }

        public OdsApiCredential(string key, string secret)
        {
            Key = key;
            Secret = secret;
        }

        public string Key { get; set; }
        public string Secret { get; set; }
    }

    public class OdsAdminAppApiCredentials
    {
        public OdsApiCredential ProductionApiCredential { get; set; }
    }
}
