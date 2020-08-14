// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    public static class AzureTestSettingsProvider
    {
        public static string GetTestConfigVariable(string variableName, string defaultValue = null)
        {
            return Environment.GetEnvironmentVariable(variableName) ??
                   ConfigurationManager.AppSettings[variableName] ??
                   defaultValue;
        }

        public static AzureActiveDirectoryClientInfo DefaultAzureActiveDirectoryClientInfo => new AzureActiveDirectoryClientInfo
        {
            ClientId = GetTestConfigVariable("ida:ClientId"),
            ClientSecret = GetTestConfigVariable("ida:ClientSecret"),
            TenantId = GetTestConfigVariable("ida:TenantId"),
            SubscriptionId = GetTestConfigVariable("ida:SubscriptionId"),
        };

        public static string ResourceGroupName => GetTestConfigVariable("resourceGroupName", "cloud_ods_integration_tests");

        public static AzureCloudOdsInstance DefaultTestCloudOdsInstance => new AzureCloudOdsInstance
        {
            Edition = "release",
            FriendlyName = "Cloud ODS Integration Tests",
            SystemId = $"/subscriptions/{GetTestConfigVariable("ida:SubscriptionId")}/resourceGroups/{ResourceGroupName}",
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
