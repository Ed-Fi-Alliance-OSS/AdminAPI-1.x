// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management
{
    public class GetOdsSqlConfigurationQuery : IGetOdsSqlConfigurationQuery
    {
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;

        public GetOdsSqlConfigurationQuery(IOdsSecretConfigurationProvider odsSecretConfigurationProvider)
        {
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
        }

        public async Task<OdsSqlConfiguration> Execute()
        {
            return await _odsSecretConfigurationProvider.GetSqlConfiguration();
        }
    }
}