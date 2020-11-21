// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public class FakeOdsSecretConfigurationProvider : IOdsSecretConfigurationProvider
    {
        private OdsSecretConfiguration _odsSecretConfig;
        private readonly OdsSqlConfiguration _odsSqlConfig;

        public FakeOdsSecretConfigurationProvider()
        {
            _odsSecretConfig = new OdsSecretConfiguration();
            _odsSqlConfig = new OdsSqlConfiguration();
        }

        public Task<OdsSqlConfiguration> GetSqlConfiguration()
        {
            return Task.FromResult(_odsSqlConfig);
        }

        public Task<OdsSecretConfiguration> GetSecretConfiguration(int? odsInstanceId = null)
        {
            return Task.FromResult(_odsSecretConfig);
        }

        public async Task SetSecretConfiguration(OdsSecretConfiguration configuration, int? odsInstanceId = null)
        {
            await Task.Run(() => { });

            _odsSecretConfig = configuration;
        }
    }
}