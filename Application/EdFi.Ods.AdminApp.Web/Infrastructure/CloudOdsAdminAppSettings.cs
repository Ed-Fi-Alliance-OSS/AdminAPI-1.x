// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.Common.Extensions;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class CloudOdsAdminAppSettings
    {
        private static readonly Lazy<CloudOdsAdminAppSettings> _instance =
            new Lazy<CloudOdsAdminAppSettings>(() => new CloudOdsAdminAppSettings());
        private readonly AppSettings _appSettings = ConfigurationHelper.GetAppSettings();

        protected CloudOdsAdminAppSettings() { }

        public static CloudOdsAdminAppSettings Instance => _instance.Value;

        public string OdsInstanceName => _appSettings.DefaultOdsInstance;

        public string ProductionApiUrl => _appSettings.ProductionApiUrl;

        public bool SystemManagedSqlServer
            => _appSettings.SystemManagedSqlServer == null ||
               bool.TrueString.Equals(
                   _appSettings.SystemManagedSqlServer,
                   StringComparison.InvariantCultureIgnoreCase);

        public bool DbSetupEnabled => bool.TrueString.Equals(
            _appSettings.DbSetupEnabled, StringComparison.InvariantCultureIgnoreCase);

        public int SecurityMetadataCacheTimeoutMinutes
        {
            get
            {
                var timeOut = _appSettings.SecurityMetadataCacheTimeoutMinutes;
                return int.Parse(timeOut ?? "0");
            }
        }

        public ApiMode Mode
        {
            get
            {
                var mode = _appSettings.ApiStartupType;
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
