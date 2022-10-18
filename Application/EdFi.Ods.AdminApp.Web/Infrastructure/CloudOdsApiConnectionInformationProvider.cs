// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class CloudOdsApiConnectionInformationProvider : IOdsApiConnectionInformationProvider
    {
        private readonly IGetOdsAdminAppApiCredentialsQuery _getOdsAdminAppApiCredentialsQuery;
        private readonly InstanceContext _instanceContext;
        private readonly ICloudOdsAdminAppSettingsApiModeProvider _apiModeProvider;

        public CloudOdsApiConnectionInformationProvider(IGetOdsAdminAppApiCredentialsQuery getOdsAdminAppApiCredentialsQuery, InstanceContext instanceContext, ICloudOdsAdminAppSettingsApiModeProvider apiModeProvider)
        {
            _getOdsAdminAppApiCredentialsQuery = getOdsAdminAppApiCredentialsQuery;
            _instanceContext = instanceContext;
            _apiModeProvider = apiModeProvider;
        }

        public async Task<OdsApiConnectionInformation> GetConnectionInformationForEnvironment()
        {
            var apiCredentials = await _getOdsAdminAppApiCredentialsQuery.Execute();

            if (ApiCredentialAreCorrupted())
            {
                ThrowSecretCorruptionException();
            }

            return GetConnectionInformationForEnvironment(apiCredentials.ProductionApiCredential, _instanceContext.Name, _apiModeProvider.GetApiMode());

            bool ApiCredentialAreCorrupted()
            {
                return apiCredentials == null || apiCredentials.ProductionApiCredential == null;
            }

            void ThrowSecretCorruptionException()
            {
                const string message =
                    "This application's API stored key and secret have been corrupted. See <a href=\"https://techdocs.ed-fi.org/display/AP/Troubleshooting\">Ed-Fi Tech Docs</a> for help.";
                throw new InvalidOperationException(message);
            }
        }

        public static OdsApiConnectionInformation GetConnectionInformationForEnvironment(OdsApiCredential apiCredentials, string instanceName, ApiMode apiMode)
        {
            if (apiCredentials == null)
            {
                throw new ArgumentNullException(nameof(apiCredentials));
            }
            if (string.IsNullOrWhiteSpace(apiCredentials.Key))
            {
                throw new ArgumentException($"{nameof(apiCredentials.Key)} in {nameof(apiCredentials)} cannot be null or whitespace");
            }
            if (string.IsNullOrWhiteSpace(apiCredentials.Secret))
            {
                throw new ArgumentException($"{nameof(apiCredentials.Secret)} in {nameof(apiCredentials)} cannot be null or whitespace");
            }

            return ConnectionInformationForEnvironment(CloudOdsAdminAppSettings.Instance.ProductionApiUrl, apiCredentials, instanceName, apiMode);
        }

        private static OdsApiConnectionInformation ConnectionInformationForEnvironment(string apiUrl, OdsApiCredential apiCredentials, string instanceName, ApiMode apiMode)
        {
            var connectionInformation = new OdsApiConnectionInformation(instanceName, apiMode)
            {
                ApiServerUrl = $"{apiUrl}",
                ClientKey = apiCredentials.Key,
                ClientSecret = apiCredentials.Secret,
                OAuthUrl = $"{apiUrl}/"
            };
            return connectionInformation;
        }
    }
}
