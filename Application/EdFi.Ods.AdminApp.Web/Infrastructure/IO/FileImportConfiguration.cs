// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class FileImportConfiguration: LoadTools.BulkLoadClient.Application.Configuration
    {
        public FileImportConfiguration(string dataDirectoryFullPath,
            string workingFolderPath, int? schoolYear = null, string apiBaseUrl = null, string clientKey = null,
            string clientSecret = null, string oauthUrl = null, string metadataUrl = null,
            string dependenciesUrl = null, string schemaPath = null, int maxSimultaneousRequests = 20)
        {
            Require(apiBaseUrl);
            Require(clientKey);
            Require(clientSecret);
            Require(oauthUrl);
            Require(metadataUrl);
            Require(dependenciesUrl);

            SetConfiguration(dataDirectoryFullPath, workingFolderPath, apiBaseUrl, clientKey, clientSecret, oauthUrl, metadataUrl, dependenciesUrl, schoolYear, schemaPath, maxSimultaneousRequests);
        }

        private void Require(string connectionInformationArgument)
        {
            if (string.IsNullOrEmpty(connectionInformationArgument))
            {
                throw new ArgumentNullException(nameof(connectionInformationArgument));
            }
        }

        private void SetConfiguration(string dataDirectoryFullPath, string workingFolderPath, string apiBaseUrl, string clientKey, string clientSecret, string oauthUrl, string metadataUrl, string dependenciesUrl, int? schoolYear, string schemaPath,
            int maxSimultaneousRequests)
        {
            DataFolder = dataDirectoryFullPath;
            WorkingFolder = workingFolderPath;
            ApiUrl = apiBaseUrl + "/";
            OauthKey = clientKey;
            OauthSecret = clientSecret;
            OauthUrl = oauthUrl;
            MetadataUrl = metadataUrl;
            XsdFolder = schemaPath;
            InterchangeOrderFolder = schemaPath;
            Retries = 3;
            SchoolYear = schoolYear;
            ConnectionLimit = maxSimultaneousRequests;
            TaskCapacity = maxSimultaneousRequests;
            MaxSimultaneousRequests = maxSimultaneousRequests;
            DependenciesUrl = dependenciesUrl;
        }
    }
}
