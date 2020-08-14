// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureResourceManagerClientInfo
    {
        public string ResourceManagerIdentifier { get; set; }
        public string Url { get; set; }
        public string ApiVersion { get; set; }

        public static AzureResourceManagerClientInfo Default { private set; get; } = new AzureResourceManagerClientInfo
        {
            ApiVersion = "2014-04-01-preview",
            ResourceManagerIdentifier = "https://management.azure.com/",
            Url = "https://management.azure.com/"
        };
    }
}
