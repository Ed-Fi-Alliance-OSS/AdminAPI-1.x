// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Common.Extensions;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class CloudOdsAdminAppSettings
    {
        private static readonly Lazy<CloudOdsAdminAppSettings> _instance =
            new Lazy<CloudOdsAdminAppSettings>(() => new CloudOdsAdminAppSettings());

        public static AppSettings AppSettings
        {
            get
            {
                return Startup.ConfigurationAppSettings;
            }
        }


        protected CloudOdsAdminAppSettings() { }

        public static CloudOdsAdminAppSettings Instance => _instance.Value;

        public string OdsInstanceName => AppSettings.DefaultOdsInstance;

        public string ProductionApiUrl => AppSettings.ProductionApiUrl;

        public string ApiExternalUrl => AppSettings.ApiExternalUrl;

        [Obsolete("This property is no longer used.")]
        public bool SystemManagedSqlServer
            => AppSettings.SystemManagedSqlServer == null ||
               bool.TrueString.Equals(
                   AppSettings.SystemManagedSqlServer,
                   StringComparison.InvariantCultureIgnoreCase);

        [Obsolete("This property is no longer used.")]
        public bool DbSetupEnabled => bool.TrueString.Equals(
            AppSettings.DbSetupEnabled, StringComparison.InvariantCultureIgnoreCase);

        public int SecurityMetadataCacheTimeoutMinutes
        {
            get
            {
                var timeOut = AppSettings.SecurityMetadataCacheTimeoutMinutes;
                return int.Parse(timeOut ?? "0");
            }
        }

        public ApiMode Mode
        {
            get
            {
                var mode = AppSettings.ApiStartupType;
                ApiMode startupMode;

                if (string.IsNullOrWhiteSpace(mode))
                {
                    throw new ConfigurationErrorsException("No value found for app key 'apiStartup:type'.");
                }

                if (ApiMode.TryParse(x => x.Value.EqualsIgnoreCase(mode), out var apiMode))
                {
                    startupMode = apiMode;
                }
                else
                {
                    throw new NotSupportedException(
                        $"Not supported apiStartup:type \"{mode}\". Supported modes: {ApiMode.Sandbox.Value}, {ApiMode.SharedInstance.Value}, {ApiMode.YearSpecific.Value} and {ApiMode.DistrictSpecific.Value}.");
                }

                return startupMode;
            }
        }
    }

    public class CloudOdsAdminAppSettingsApiModeProvider: ICloudOdsAdminAppSettingsApiModeProvider
    {
        public ApiMode GetApiMode()
        {
            return CloudOdsAdminAppSettings.Instance.Mode;
        }
    }

    public interface ICloudOdsAdminAppSettingsApiModeProvider
    {
        ApiMode GetApiMode();
    }
}
