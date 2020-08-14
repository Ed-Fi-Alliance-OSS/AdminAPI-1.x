// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    public class TestOdsApiConnectionInformationProvider : IOdsApiConnectionInformationProvider
    {
        public Task<OdsApiConnectionInformation> GetConnectionInformationForEnvironment(CloudOdsEnvironment environment)
        {
            var apiCredentials = ApiCredentialForEnvironment(environment);

            if (environment == CloudOdsEnvironment.Production)
            {
                return ConnectionInformationForEnvironment(AzureTestSettingsProvider.GetTestConfigVariable("ProductionApiUrl"), apiCredentials);
            }

            throw new InvalidOperationException($"Cannot provide connection information for '{environment.DisplayName}' environment");
        }

        public Task<OdsApiConnectionInformation> GetConnectionInformationForEnvironment()
        {
            return GetConnectionInformationForEnvironment(CloudOdsEnvironment.Production);
        }

        private static OdsApiCredential ApiCredentialForEnvironment(CloudOdsEnvironment environment)
        {
            if (environment == CloudOdsEnvironment.Production)
            {
                return new OdsApiCredential(AzureTestSettingsProvider.GetTestConfigVariable("ProductionApiKey"), AzureTestSettingsProvider.GetTestConfigVariable("ProductionApiSecret"));
            }

            throw new InvalidOperationException($"Cannot provide API credentials for '{environment.DisplayName}' environment");
        }

        private static Task<OdsApiConnectionInformation> ConnectionInformationForEnvironment(string apiUrl, OdsApiCredential apiCredentials)
        {
            return Task.FromResult(
                new OdsApiConnectionInformation ("Ods Instance", ApiMode.Sandbox)
                {
                    ApiServerUrl = $"{apiUrl}",
                    ClientKey = apiCredentials.Key,
                    ClientSecret = apiCredentials.Secret,
                    OAuthUrl = $"{apiUrl}/"
                });
        }
    }
}