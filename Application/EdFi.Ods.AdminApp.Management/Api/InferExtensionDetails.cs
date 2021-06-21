// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IInferExtensionDetails
    {
        bool TpdmExtensionEnabled(string apiServerUrl);
    }

    public class InferExtensionDetails : IInferExtensionDetails
    {
        public bool TpdmExtensionEnabled(string apiServerUrl)
        {
            using (var client = new WebClient())
            {
                var rawApis = client.DownloadString(apiServerUrl);

                var response = JToken.Parse(rawApis);

                foreach (var dataModel in response["dataModels"])
                    if (dataModel["name"].ToString().ToUpper() == "TPDM")
                        return true;

                return false;
            }
        }
    }
}
