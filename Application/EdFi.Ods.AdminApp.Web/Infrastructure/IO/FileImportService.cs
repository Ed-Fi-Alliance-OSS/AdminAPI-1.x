// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Threading.Tasks;
using EdFi.LoadTools.BulkLoadClient;
using EdFi.Ods.AdminApp.Management.Api;
using Microsoft.Extensions.Configuration;
using OdsVersion = System.Version;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class FileImportService
    {
        private readonly IConfiguration _configuration;
        private readonly IInferOdsApiVersion _inferOdsApiVersion;
        private readonly ITokenRetriever _tokenRetriever;
        private static readonly string[] UnsupportedInterchanges = {"InterchangeDescriptors"};

        public FileImportService(IConfiguration configuration, string instanceName, IInferOdsApiVersion inferOdsApiVersion)
        {
            _configuration = configuration;
            _inferOdsApiVersion = inferOdsApiVersion;

            _tokenRetriever = new TokenRetriever(new OdsApiConnectionInformation(instanceName, CloudOdsAdminAppSettings.Instance.Mode)
            {
                OAuthUrl = _configuration.GetValue<string>("OdsApi:OAuthUrl"),
                ClientKey = _configuration.GetValue<string>("OdsApi:Key"),
                ClientSecret = _configuration.GetValue<string>("OdsApi:Secret")
            });
        }

        public BulkLoadValidationResult Validate()
        {
            var xsdFolderPath = _configuration.GetValue<string>("Folders:Xsd");

            var xsdDirectoryExistWithFiles =
                Directory.Exists(xsdFolderPath) && Directory.GetFiles(xsdFolderPath).Length > 0;
            var odsVersion = new OdsVersion(_inferOdsApiVersion.Version(_configuration.GetValue<string>("OdsApi:Url")).GetAwaiter().GetResult());
            var requiredVersion = new OdsVersion("5.1.0");
            if(odsVersion < requiredVersion)
            {
                _configuration["OdsApi:Url"] = string.Empty;
            }
            if (xsdDirectoryExistWithFiles)
            {
                _configuration["ForceMetadata"] = "false";
            }
            var result = LoadProcess.ValidateXmlFile(_configuration, UnsupportedInterchanges).GetAwaiter()
                .GetResult();

            return result;
        }

        public int Execute()
        {
            _configuration["ForceMetadata"] = "false";
            _configuration["OdsApi:OAuthUrl"] = Path.Combine(_configuration["OdsApi:OAuthUrl"], "oauth/token");
            var result = LoadProcess.Run(_configuration).GetAwaiter().GetResult();
            return result;
        }

        public string Authenticate()
        {
            var result = _tokenRetriever.ObtainNewBearerToken();
            return result;
        }
    }   
}
