// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
using EdFi.Common.Security;

namespace EdFi.Ods.AdminApp.Management
{
    public class ApiClientFactory
    {
        private readonly ISecurePackedHashProvider _securePackedHashProvider;
        private readonly IHashConfigurationProvider _hashConfigurationProvider;

        public class ApiWithCredentials
        {
            public ApiClient ApiClient { get; set; }
            public OdsApiCredential ApiCredentials { get; set; }
        }

        public ApiClientFactory(ISecurePackedHashProvider securePackedHashProvider,
            IHashConfigurationProvider hashConfigurationProvider)
        {
            _securePackedHashProvider = securePackedHashProvider;
            _hashConfigurationProvider = hashConfigurationProvider;
        }

        public ApiWithCredentials GetApiClientAndCredentials(string applicationName)
        {
            var apiClient = CreateProductionApiClient(applicationName);

            // Due to a bug with using GetHashCode() to verify the client secret hashes between apps,
            // we can't use secure secrets until the hashing code is fixed. The workaround for this
            // problem is to store secrets in plain text until the ODS hashing takes over when a secret
            // is used; Follow ODS-2679 for a complete resolution.
            //
            // var apiCredentials = new OdsApiCredential(apiClient.Key,
            //    apiClient.GenerateSecureClientSecret(_securePackedHashProvider, _hashConfigurationProvider));

            var apiCredentials = new OdsApiCredential(apiClient.Key, apiClient.Secret);

            return new ApiWithCredentials
            {
                ApiClient = apiClient,
                ApiCredentials = apiCredentials
            };
        }
       
        private static ApiClient CreateProductionApiClient(string applicationName)
        {
            var apiClient = new ApiClient(generateKey: true)
            {
                Name = applicationName,
                KeyStatus = "Active",
                IsApproved = true,
                UseSandbox = false
            };

            return apiClient;
        }
    }
}
