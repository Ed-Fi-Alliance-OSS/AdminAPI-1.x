// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    public static class AzureTestSettingsProvider
    {
        public static string GetTestConfigVariable(string variableName, string defaultValue = null)
        {
            return Environment.GetEnvironmentVariable(variableName) ??
                   defaultValue;
        }
        
        public static string ResourceGroupName => GetTestConfigVariable("resourceGroupName", "cloud_ods_integration_tests");

        public static AzureCloudOdsInstance DefaultTestCloudOdsInstance => new AzureCloudOdsInstance
        {
            Edition = "release",
            FriendlyName = "Cloud ODS Integration Tests",
            SystemId = $"/subscriptions/{ConfigurationHelper.GetAppSettings().IdaSubscriptionId}/resourceGroups/{ResourceGroupName}",
            SystemName = ResourceGroupName,
            Version = "0.0.1"
        };
    }
}
