// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class CloudOdsAdminAppSettingsForAzure
    {
        private static readonly Lazy<CloudOdsAdminAppSettingsForAzure> _instance = new Lazy<CloudOdsAdminAppSettingsForAzure>(() => new CloudOdsAdminAppSettingsForAzure());

        protected CloudOdsAdminAppSettingsForAzure()
        {
        }

        public static CloudOdsAdminAppSettingsForAzure Instance => _instance.Value;
        
        public string AzureActiveDirectoryInstance => ConfigurationManager.AppSettings["ida:AADInstance"];
        public string AzureActiveDirectoryClientId => ConfigurationManager.AppSettings["ida:ClientId"];
        public string AzureActiveDirectoryClientSecret => ConfigurationManager.AppSettings["ida:ClientSecret"];
        public string AzureActiveDirectoryTenantId => ConfigurationManager.AppSettings["ida:TenantId"];
        public string AzureActiveDirectorySubscriptionId => ConfigurationManager.AppSettings["ida:SubscriptionId"];
    }
}