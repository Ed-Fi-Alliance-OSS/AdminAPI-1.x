// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IInferOdsApiVersion
    {
       Task<string> Version(string apiServerUrl);
       Task<string> EdFiStandardVersion(string apiServerUrl);
    }

    public class InferOdsApiVersion : IInferOdsApiVersion
    {
        private readonly IHttpClientFactory _clientFactory;

        public InferOdsApiVersion(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> Version(string apiServerUrl)
        {
            var httpClient = _clientFactory.CreateClient();

            using (var response = await httpClient.GetAsync(apiServerUrl))
            {
                using (var content = response.Content)
                {
                    var contentAsString = await content.ReadAsStringAsync();

                    var parsedContent = JToken.Parse(contentAsString);

                    var apiVersion = parsedContent["version"]?.ToString();

                    return apiVersion;
                }
            }
        }

        public async Task<string> EdFiStandardVersion(string apiServerUrl)
        {
            var httpClient = _clientFactory.CreateClient();

            using (var response = await httpClient.GetAsync(apiServerUrl))
            {
                using (var content = response.Content)
                {
                    var contentAsString = await content.ReadAsStringAsync();

                    var parsedContent = JToken.Parse(contentAsString);

                    var dataModels = parsedContent["dataModels"];

                    if (dataModels != null)
                    {
                        foreach (var dataModel in dataModels)
                            if (dataModel["name"]?.ToString() == "Ed-Fi")
                                return dataModel["version"]?.ToString();
                    }

                    throw new Exception("The Ed-Fi Standard version could not be determined for this ODS/API instance.");
                }
            }
        }
    }
}
