// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Azure;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class CloudOdsAzureActiveDirectoryClientInfo
    {
        public static AzureActiveDirectoryClientInfo GetActiveDirectoryClientInfoForUser()
        {
            return new AzureActiveDirectoryClientInfo
            {
                AzureActiveDirectoryInstance = CloudOdsAdminAppSettingsForAzure.Instance.AzureActiveDirectoryInstance,
                ClientId = CloudOdsAdminAppSettingsForAzure.Instance.AzureActiveDirectoryClientId,
                ClientSecret = CloudOdsAdminAppSettingsForAzure.Instance.AzureActiveDirectoryClientSecret,
                SubscriptionId = CloudOdsAdminAppSettingsForAzure.Instance.AzureActiveDirectorySubscriptionId,
                TenantId = CloudOdsAdminAppSettingsForAzure.Instance.AzureActiveDirectoryTenantId
            };
        }
    }
}