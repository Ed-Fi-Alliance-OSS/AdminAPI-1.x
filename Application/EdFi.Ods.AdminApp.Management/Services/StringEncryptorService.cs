// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Text;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public class StringEncryptorService : IStringEncryptorService
    {
        private readonly IEncryptionConfigurationProviderService _encryptionConfigurationProviderService;
        private const DataProtectionScope ProtectionScope = DataProtectionScope.LocalMachine;
        private const string EncryptionPrefix = "ENCRYPT::";

        public StringEncryptorService(IEncryptionConfigurationProviderService encryptionConfigurationProviderService)
        {
            _encryptionConfigurationProviderService = encryptionConfigurationProviderService;
        }

        public string Encrypt(string clearText)
        {
            if (IsCipherText(clearText))
                return clearText;
            var clearTextBytes = Encoding.UTF8.GetBytes(clearText);
            var unencodedCipherText = ProtectedData.Protect(clearTextBytes, _encryptionConfigurationProviderService.GetEntropy(), ProtectionScope);
            var encodedCipherText = Convert.ToBase64String(unencodedCipherText);
            return PrefixSettingWithEncryptionFlag(encodedCipherText);
        }

        public bool TryDecrypt(string cipherText, out string decryptedText)
        {
            if (!IsCipherText(cipherText))
            {
                decryptedText = cipherText;
                return false;
            }
            var decodedCipherText = Convert.FromBase64String(cipherText.Substring(EncryptionPrefix.Length));

            decryptedText = Encoding.UTF8.GetString(ProtectedData.Unprotect(decodedCipherText,
                _encryptionConfigurationProviderService.GetEntropy(), ProtectionScope));

            return true;
        }

        public bool IsCipherText(string value)
        {
            return value.StartsWith(EncryptionPrefix);
        }

        protected string PrefixSettingWithEncryptionFlag(string cipherText)
        {
            return EncryptionPrefix + cipherText;
        }
    }
}