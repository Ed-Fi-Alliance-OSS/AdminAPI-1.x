// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class FileImportConfiguration
    {
        private void Require(string connectionInformationArgument)
        {
            if (string.IsNullOrEmpty(connectionInformationArgument))
            {
                throw new ArgumentNullException(nameof(connectionInformationArgument));
            }
        }

        public IConfiguration SetConfiguration(BulkUploadJobContext bulkUploadJobContext, string workingFolderPath)
        {
            Require(bulkUploadJobContext.ApiBaseUrl);
            Require(bulkUploadJobContext.ClientKey);
            Require(bulkUploadJobContext.ClientSecret);
            Require(bulkUploadJobContext.OauthUrl);
            Require(bulkUploadJobContext.MetadataUrl);
            Require(bulkUploadJobContext.DependenciesUrl);

            var builder = new ConfigurationBuilder();
            var configuration =
                builder.Add(new BulkUploadConfigurationSource())
                    .Build(); 

            configuration["OdsApi:Url"] = bulkUploadJobContext.ApiServerUrl;
            configuration["Folders:Data"] = bulkUploadJobContext.DataDirectoryFullPath;
            configuration["Folders:Working"] = workingFolderPath;
            configuration["OdsApi:ApiUrl"] = bulkUploadJobContext.ApiBaseUrl + "/";
            configuration["OdsApi:Key"] = bulkUploadJobContext.ClientKey;
            configuration["OdsApi:Secret"] = bulkUploadJobContext.ClientSecret;
            configuration["OdsApi:OAuthUrl"] = bulkUploadJobContext.OauthUrl;
            configuration["OdsApi:MetadataUrl"] = bulkUploadJobContext.MetadataUrl;
            configuration["Folders:Xsd"] = bulkUploadJobContext.SchemaPath;
            configuration["Folders:Interchange"] = bulkUploadJobContext.SchemaPath;
            configuration["Concurrency:MaxRetries"] = "3";
            configuration["OdsApi:SchoolYear"] = bulkUploadJobContext.SchoolYear.ToString();

            configuration["Concurrency:ConnectionLimit"] =
                bulkUploadJobContext.MaxSimultaneousRequests.ToString();

            configuration["Concurrency:MaxSimultaneousApiRequests"] =
                bulkUploadJobContext.MaxSimultaneousRequests.ToString();

            configuration["Concurrency:TaskCapacity"] =
                bulkUploadJobContext.MaxSimultaneousRequests.ToString();

            configuration["OdsApi:DependenciesUrl"] = bulkUploadJobContext.DependenciesUrl;
            configuration["ValidateSchema"] = "false";
            configuration["ForceMetadata"] = "true";

            return configuration;
        }
    }

    public class BulkUploadConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new MemoryConfigurationProvider(new MemoryConfigurationSource());
    }
}
