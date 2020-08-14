// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IInferOdsApiVersion
    {
       string Version(string apiServerUrl);
       string EdFiStandardVersion(string apiServerUrl);
    }

    public class InferOdsApiVersion : IInferOdsApiVersion
    {
        public string Version(string apiServerUrl)
        {
            using (var client = new WebClient())
            {
                var rawApis = client.DownloadString(apiServerUrl);

                var response = JToken.Parse(rawApis);

                var apiVersion = response["version"].ToString();

                return apiVersion;
            }
        }

        public string EdFiStandardVersion(string apiServerUrl)
        {
            using (var client = new WebClient())
            {
                var rawApis = client.DownloadString(apiServerUrl);

                var response = JToken.Parse(rawApis);

                foreach (var dataModel in response["dataModels"])
                    if (dataModel["name"].ToString() == "Ed-Fi")
                        return dataModel["version"].ToString();

                throw new Exception("The Ed-Fi Standard version could not be determined for this ODS/API instance.");
            }
        }
    }
}
