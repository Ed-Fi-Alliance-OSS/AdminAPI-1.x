// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.LoadTools.BulkLoadClient;
using EdFi.Ods.AdminApp.Management.Api;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class FileImportService
    {
        private readonly FileImportConfiguration _configuration;
        private readonly ITokenRetriever _tokenRetriever;
        private static readonly string[] UnsupportedInterchanges = {"InterchangeDescriptors"};

        public FileImportService(FileImportConfiguration configuration, string instanceName)
        {
            _configuration = configuration;
            _tokenRetriever = new TokenRetriever(new OdsApiConnectionInformation(instanceName, CloudOdsAdminAppSettings.Instance.Mode)
            {
                OAuthUrl = configuration.OauthUrl,
                ClientKey = configuration.OauthKey,
                ClientSecret = configuration.OauthSecret
            });
        }

        public BulkLoadValidationResult Validate()
        {
            try
            {
                return LoadProcess.ValidateXmlFile(_configuration, UnsupportedInterchanges); 
            }
            catch (Exception ex)
            {
                return new BulkLoadValidationResult($"An error occured during file validation: {ex.Message}");
            }
        }

        public int Execute()
        {
            var result = LoadProcess.Run(_configuration);
            return result;
        }

        public string Authenticate()
        {
            var result = _tokenRetriever.ObtainNewBearerToken();
            return result;
        }
    }   
}