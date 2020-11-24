// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Configuration;

namespace EdFi.Ods.AdminApp.Management.Helpers
{
    public static class ConfigurationHelper
    {

        private static readonly AppSettings _appSettings;

        private static readonly ConnectionStrings _connectionStrings;

        static ConfigurationHelper()
        {
            _appSettings = new AppSettings();
            _connectionStrings = new ConnectionStrings();

            _appSettings.AppStartup = ConfigurationManager.AppSettings["owin:appStartup"];
            _appSettings.DatabaseEngine = ConfigurationManager.AppSettings["DatabaseEngine"];
            _appSettings.ApplicationInsightsInstrumentationKey =
                ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];
            _appSettings.XsdFolder = ConfigurationManager.AppSettings["XsdFolder"];
            _appSettings.DefaultOdsInstance = ConfigurationManager.AppSettings["DefaultOdsInstance"];
            _appSettings.ProductionApiUrl = ConfigurationManager.AppSettings["ProductionApiUrl"];
            _appSettings.SystemManagedSqlServer = ConfigurationManager.AppSettings["SystemManagedSqlServer"];
            _appSettings.DbSetupEnabled = ConfigurationManager.AppSettings["DbSetupEnabled"];
            _appSettings.SecurityMetadataCacheTimeoutMinutes =
                ConfigurationManager.AppSettings["SecurityMetadataCacheTimeoutMinutes"];
            _appSettings.ApiStartupType = ConfigurationManager.AppSettings["apiStartup:type"];
            _appSettings.LocalEducationAgencyTypeValue = ConfigurationManager.AppSettings["LocalEducationAgencyTypeValue"];
            _appSettings.SchoolTypeValue = ConfigurationManager.AppSettings["SchoolTypeValue"];
            _appSettings.BulkUploadHashCache = ConfigurationManager.AppSettings["BulkUploadHashCache"];
            _appSettings.IdaAADInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
            _appSettings.IdaClientId = ConfigurationManager.AppSettings["ida:ClientId"];
            _appSettings.IdaClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
            _appSettings.IdaTenantId = ConfigurationManager.AppSettings["ida:TenantId"];
            _appSettings.IdaSubscriptionId = ConfigurationManager.AppSettings["ida:SubscriptionId"];
            _appSettings.AwsCurrentVersion = ConfigurationManager.AppSettings["AwsCurrentVersion"];
            _appSettings.OptionalEntropy = ConfigurationManager.AppSettings["OptionalEntropy"];
            _appSettings.Log4NetConfigPath = ConfigurationManager.AppSettings["log4net.Config"];

            _connectionStrings.Admin = ConfigurationManager.ConnectionStrings[CloudOdsDatabaseNames.Admin] != null ? ConfigurationManager.ConnectionStrings[CloudOdsDatabaseNames.Admin].ConnectionString : "";
            _connectionStrings.Security = ConfigurationManager.ConnectionStrings[CloudOdsDatabaseNames.Security] != null ? ConfigurationManager.ConnectionStrings[CloudOdsDatabaseNames.Security].ConnectionString : "";
            _connectionStrings.ProductionOds = ConfigurationManager.ConnectionStrings[CloudOdsDatabaseNames.ProductionOds] != null ? ConfigurationManager.ConnectionStrings[CloudOdsDatabaseNames.ProductionOds].ConnectionString : "";
            _connectionStrings.OdsEmpty = ConfigurationManager.ConnectionStrings["EdFi_Ods_Empty"] != null ? ConfigurationManager.ConnectionStrings["EdFi_Ods_Empty"].ConnectionString : "";
            _connectionStrings.AzureSql = ConfigurationManager.ConnectionStrings["AzureSql"] != null ? ConfigurationManager.ConnectionStrings["AzureSql"].ConnectionString : "";
        }

        public static AppSettings GetAppSettings()
        {
            return _appSettings;
        }

        public static void SetApiMode(string apiMode)
        {
            ConfigurationManager.AppSettings["apiStartup:type"] = apiMode;
            _appSettings.ApiStartupType = apiMode;
        }

        public static ConnectionStrings GetConnectionStrings()
        {
            return _connectionStrings;
        }

        public static string GetConnectionStringByName(string databaseName)
        {
            return ConfigurationManager.ConnectionStrings[databaseName]?.ConnectionString;
        }

        public static ConnectionStringSettingsCollection GetConnectionStringCollection()
        {
            return ConfigurationManager.ConnectionStrings;
        }
    }

    public class AppSettings
    {
        public string AppStartup { get; set; }
        public string DatabaseEngine { get; set; }
        public string ApplicationInsightsInstrumentationKey { get; set; }
        public string XsdFolder { get; set; }
        public string DefaultOdsInstance { get; set; }
        public string ProductionApiUrl { get; set; }
        public string SystemManagedSqlServer { get; set; }
        public string DbSetupEnabled { get; set; }
        public string SecurityMetadataCacheTimeoutMinutes { get; set; }
        public string ApiStartupType { get; set; }
        public string LocalEducationAgencyTypeValue { get; set; }
        public string SchoolTypeValue { get; set; }
        public string BulkUploadHashCache { get; set; }
        public string IdaAADInstance { get; set; }
        public string IdaClientId { get; set; }
        public string IdaClientSecret { get; set; }
        public string IdaTenantId { get; set; }
        public string IdaSubscriptionId { get; set; }
        public string AwsCurrentVersion { get; set; }
        public string OptionalEntropy { get; set; }
        public string Log4NetConfigPath { get; set; }
    }

    public class ConnectionStrings
    {
        public string Admin { get; set; }
        public string Security { get; set; }
        public string ProductionOds { get; set; }
        public string OdsEmpty { get; set; }
        public string AzureSql { get; set; }
    }
}
