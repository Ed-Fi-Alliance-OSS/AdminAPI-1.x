// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Helpers
{
    public class AppSettings
    {
        public string AppStartup { get; set; }
        public string DatabaseEngine { get; set; }
        public string ApplicationInsightsInstrumentationKey { get; set; }
        public string XsdFolder { get; set; }
        public string DefaultOdsInstance { get; set; }
        public string ProductionApiUrl { get; set; }
        public string ApiExternalUrl { get; set; }
        public string SecurityMetadataCacheTimeoutMinutes { get; set; }
        public string ApiStartupType { get; set; }
        public string LocalEducationAgencyTypeValue { get; set; }
        public string PostSecondaryInstitutionTypeValue { get; set; }
        public string SchoolTypeValue { get; set; }
        public string BulkUploadHashCache { get; set; }
        public string IdaAADInstance { get; set; }
        public string IdaClientId { get; set; }
        public string IdaClientSecret { get; set; }
        public string IdaTenantId { get; set; }
        public string IdaSubscriptionId { get; set; }
        public string AwsCurrentVersion { get; set; }
        public string Log4NetConfigPath { get; set; }
        public string EncryptionKey { get; set; }
        public bool EnableProductImprovementSettings { get; set; }

        public string GoogleAnalyticsMeasurementId { get; set; }
        public string ProductRegistrationUrl { get; set; }
    }
}
