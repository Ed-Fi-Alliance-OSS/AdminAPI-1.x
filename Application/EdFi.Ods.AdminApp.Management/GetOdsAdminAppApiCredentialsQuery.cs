// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management
{
    public class GetOdsAdminAppApiCredentialsQuery : IGetOdsAdminAppApiCredentialsQuery
    {
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly InstanceContext _instanceContext;

        public GetOdsAdminAppApiCredentialsQuery(IOdsSecretConfigurationProvider odsSecretConfigurationProvider, InstanceContext instanceContext)
        {
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _instanceContext = instanceContext;
        }

        public async Task<OdsAdminAppApiCredentials> Execute()
        {
            var configuration = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);

            return configuration.GetOdsAdminAppApiCredentials();
        }
    }
}