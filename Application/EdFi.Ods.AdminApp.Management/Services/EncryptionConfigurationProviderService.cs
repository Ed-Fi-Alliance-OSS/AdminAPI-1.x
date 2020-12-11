// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public class EncryptionConfigurationProviderService : IEncryptionConfigurationProviderService
    {
        private readonly AppSettings _appSettings;

        public EncryptionConfigurationProviderService(IOptions<AppSettings> appSettingsAccessor)
        {
            _appSettings = appSettingsAccessor.Value;
        }

        public byte[] GetEntropy()
        {
            var optionalEntropyValue = _appSettings.OptionalEntropy;
            var entropy = string.IsNullOrEmpty(optionalEntropyValue)
                ? null
                : Encoding.ASCII.GetBytes(optionalEntropyValue);
            return entropy;
        }
    }
}