// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Helpers;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    public static class AzureTestSettingsProvider
    {
        private static readonly AppSettings _appSettings = ConfigurationHelper.GetAppSettings();

        public static string GetTestConfigVariable(string variableName, string defaultValue = null)
        {
            return Environment.GetEnvironmentVariable(variableName) ??
                   defaultValue;
        }

        public static AzureActiveDirectoryClientInfo DefaultAzureActiveDirectoryClientInfo => new AzureActiveDirectoryClientInfo
        {
            ClientId = _appSettings.IdaClientId,
            ClientSecret = _appSettings.IdaClientSecret,
            TenantId = _appSettings.IdaTenantId,
            SubscriptionId = _appSettings.IdaSubscriptionId
        };

        public static string ResourceGroupName => GetTestConfigVariable("resourceGroupName", "cloud_ods_integration_tests");

        public static AzureCloudOdsInstance DefaultTestCloudOdsInstance => new AzureCloudOdsInstance
        {
            Edition = "release",
            FriendlyName = "Cloud ODS Integration Tests",
            SystemId = $"/subscriptions/{_appSettings.IdaSubscriptionId}/resourceGroups/{ResourceGroupName}",
            SystemName = ResourceGroupName,
            Version = "0.0.1"
        };

        public static CloudOdsApiOperationContext ProductionApiOperationContext => new CloudOdsApiOperationContext
        {
            Instance = DefaultTestCloudOdsInstance,
            TargetEnvironment = CloudOdsEnvironment.Production
        };
    }
}
