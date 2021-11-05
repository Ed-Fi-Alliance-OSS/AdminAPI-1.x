// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public class OdsApiResult
    {
        public string ErrorMessage { get; set; }
        public bool Success => string.IsNullOrEmpty(ErrorMessage);

        public OdsApiResult() { }
        public OdsApiResult(IRestResponse response)
        {
            if (!response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<JObject>(response.Content);
                content.TryGetValue("message", out var contentMessage);

                ErrorMessage = !string.IsNullOrEmpty(response.ErrorMessage)
                    ? response.ErrorMessage
                    : contentMessage?.ToString() ?? $"ODS API failure with no message. Status Code: {response.StatusCode}";
            }
        }
    }
}
