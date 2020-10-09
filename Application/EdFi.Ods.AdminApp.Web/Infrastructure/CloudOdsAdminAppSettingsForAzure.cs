// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Helpers;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class CloudOdsAdminAppSettingsForAzure
    {
        private static readonly Lazy<CloudOdsAdminAppSettingsForAzure> _instance = new Lazy<CloudOdsAdminAppSettingsForAzure>(() => new CloudOdsAdminAppSettingsForAzure());
        private readonly AppSettings _appSettings = ConfigurationHelper.GetAppSettings();

        protected CloudOdsAdminAppSettingsForAzure()
        {
        }

        public static CloudOdsAdminAppSettingsForAzure Instance => _instance.Value;

        public string AzureActiveDirectoryInstance => _appSettings.IdaAADInstance;
        public string AzureActiveDirectoryClientId => _appSettings.IdaClientId;
        public string AzureActiveDirectoryClientSecret => _appSettings.IdaClientSecret;
        public string AzureActiveDirectoryTenantId => _appSettings.IdaTenantId;
        public string AzureActiveDirectorySubscriptionId => _appSettings.IdaSubscriptionId;
    }
}