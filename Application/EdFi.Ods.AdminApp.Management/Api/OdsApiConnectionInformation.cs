// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Instances;
using Flurl;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public class OdsApiConnectionInformation
    {
        private readonly string _instanceName;
        private readonly ApiMode _apiMode;

        public OdsApiConnectionInformation(string instanceName, ApiMode apiMode)
        {
            _instanceName = instanceName;
            _apiMode = apiMode;     
        }

        public string ApiServerUrl { get; set; }
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public string OAuthUrl { get; set; }

        public string ApiBaseUrl
        {
            get
            {
                var uri = ApiServerUrl.AppendPathSegments("data", "v3");

                if (_apiMode == ApiMode.YearSpecific)
                {
                    uri = uri.AppendPathSegment(_instanceName.ExtractNumericInstanceSuffix());
                }

                return uri;
            }
        }

        public string MetadataUrl
        {
            get
            {
                var uri = ApiServerUrl.AppendPathSegment("metadata");

                if (_apiMode == ApiMode.YearSpecific)
                {
                    uri = uri.AppendPathSegment(_instanceName.ExtractNumericInstanceSuffix());
                }

                return uri;
            }
        }

        public string DependenciesUrl
        {
            get
            {
                var uri = ApiServerUrl.AppendPathSegments("metadata", "data", "v3");

                if (_apiMode == ApiMode.YearSpecific)
                {
                    uri = uri.AppendPathSegment(_instanceName.ExtractNumericInstanceSuffix());
                }

                uri = uri.AppendPathSegment("dependencies");

                return uri;
            }
        }

        public string DescriptorsUrl
        {
            get
            {
                var uri = ApiServerUrl.AppendPathSegments("metadata", "data", "v3");

                if (_apiMode == ApiMode.YearSpecific)
                {
                    uri = uri.AppendPathSegment(_instanceName.ExtractNumericInstanceSuffix());
                }

                uri = uri.AppendPathSegment("descriptors");

                return uri;
            }
        }
    }
}