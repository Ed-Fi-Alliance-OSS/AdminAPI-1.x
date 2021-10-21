// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Globalization;
using EdFi.Ods.AdminApp.Management.Services;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IInferExtensionDetails
    {
        TpdmExtensionDetails TpdmExtensionVersion(string apiServerUrl);        
    }

    public class InferExtensionDetails : IInferExtensionDetails
    {
        private readonly ISimpleGetRequest _getRequest;

        public InferExtensionDetails(ISimpleGetRequest getRequest) => _getRequest = getRequest;

        public TpdmExtensionDetails TpdmExtensionVersion(string apiServerUrl)
        {
            return GetVersion(apiServerUrl);          
        }

        private TpdmExtensionDetails GetVersion(string apiServerUrl)
        {
            var response = JToken.Parse(_getRequest.DownloadString(apiServerUrl));
            var versionDetails = new TpdmExtensionDetails();
            if (response["dataModels"] is JArray dataModels)
            {
                foreach (var dataModel in dataModels)
                {
                    if (dataModel is JObject && dataModel["name"] is JValue nameToken)
                        if (nameToken.ToString(CultureInfo.InvariantCulture).ToUpper() == "TPDM")
                        {
                            if (dataModel["version"] is JValue versionToken)
                            {
                                var version = versionToken.ToString(CultureInfo.InvariantCulture);
                                versionDetails.TpdmVersion = new Version(version) >= new Version("1.0.0") ? version : string.Empty;
                            }
                            versionDetails.IsTpdmCommunityVersion = dataModel["informationalVersion"] == null
                                    || !(dataModel["informationalVersion"] is JValue informationalVersionToken
                                    && informationalVersionToken.ToString(CultureInfo.InvariantCulture).ToLower().Equals("tpdm-core"));
                        }
                }
            }

            return versionDetails;
        }
    }

    public class TpdmExtensionDetails
    {
        public string TpdmVersion { get; set; }

        public bool IsTpdmCommunityVersion { get; set; }       
    }
}
