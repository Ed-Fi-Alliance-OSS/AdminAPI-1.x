// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Services;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IInferExtensionDetails
    {
        Task<TpdmExtensionDetails> TpdmExtensionVersion(string apiServerUrl);
    }

    public class InferExtensionDetails : IInferExtensionDetails
    {
        private readonly ISimpleGetRequest _getRequest;

        public InferExtensionDetails(ISimpleGetRequest getRequest) => _getRequest = getRequest;

        public async Task<TpdmExtensionDetails> TpdmExtensionVersion(string apiServerUrl)
        {
            return await GetVersion(apiServerUrl);          
        }

        private async Task<TpdmExtensionDetails> GetVersion(string apiServerUrl)
        {
            var response = JToken.Parse(await _getRequest.DownloadString(apiServerUrl));
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

                            var isTpdmCore = dataModel["informationalVersion"] is JValue informationalVersionToken
                                             && informationalVersionToken.ToString(CultureInfo.InvariantCulture).ToLower()
                                                 .Equals("tpdm-core");
                            versionDetails.IsTpdmCommunityVersion = dataModel["informationalVersion"] == null || !isTpdmCore;
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
