// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Services;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IInferExtensionDetails
    {
        bool TpdmExtensionEnabled(string apiServerUrl);
    }

    public class InferExtensionDetails : IInferExtensionDetails
    {
        private readonly ISimpleGetRequest _getRequest;

        public InferExtensionDetails(ISimpleGetRequest getRequest) => _getRequest = getRequest;

        public bool TpdmExtensionEnabled(string apiServerUrl)
        {
            var response = JToken.Parse(_getRequest.DownloadString(apiServerUrl));

            if (response["dataModels"] is JArray dataModels)
            {
                foreach (var dataModel in dataModels)
                {
                    if (dataModel is JObject && dataModel["name"] is JValue nameToken)
                        if (nameToken.ToString().ToUpper() == "TPDM")
                            if (dataModel["version"] is JValue versionToken)
                                return new Version(versionToken.ToString()) >= new Version("1.0.0");
                }
            }

            return false;
        }
    }
}
