// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.Settings
{
    public class CloudOdsApiWebsiteSettings
    {
        public const string BearerTokenTimeoutSettingName = "BearerTokenTimeoutInMinutes";
        public const int DefaultBearerTokenTimeoutInMinutes = 15;
        public const string LogLevelSettingName = "LogLevelOverride";

        public static readonly LogLevel DefaultLogLevel = LogLevel.Info;

        private readonly IDictionary<string, string> _settingsDictionary; 

        public CloudOdsApiWebsiteSettings()
        {
            _settingsDictionary = new Dictionary<string, string>();
        }

        public CloudOdsApiWebsiteSettings(IDictionary<string, string> settings)
        {
            _settingsDictionary = settings;
        }

        public IDictionary<string, string> AsDictionary()
        {
            return _settingsDictionary;
        }


        public int BearerTokenTimeoutInMinutes
        {
            get => _settingsDictionary.GetOptionalIntValueOrDefault(BearerTokenTimeoutSettingName, DefaultBearerTokenTimeoutInMinutes);
            set => _settingsDictionary[BearerTokenTimeoutSettingName] = value.ToString();
        }

        public LogLevel LogLevel
        {
            get
            {
                var stringSetting = _settingsDictionary.GetOptionalStringValue(LogLevelSettingName, DefaultLogLevel.DisplayName);
                LogLevel result;

                return LogLevel.TryParse(stringSetting, out result)
                    ? result
                    : DefaultLogLevel;
            }
            set => _settingsDictionary[LogLevelSettingName] = value.DisplayName;
        }
    }
}