// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EdFi.Ods.AdminApp.Management.Services
{
    // ReSharper disable once InconsistentNaming - AES is proper, not Aes
    public class AESEncryptorService : IStringEncryptorService
    {
        private readonly byte[] _initializationVector;
        private readonly byte[] _key;

        // ReSharper disable once InconsistentNaming - AES is proper, not Aes
        public AESEncryptorService(string key, string initializationVector = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(
                    $"Parameter {nameof(key)} cannot be null.", nameof(key));
            }

            if (key.Trim().Length == 0)
            {
                throw new ArgumentException(
                    $"Parameter {nameof(key)} cannot be an empty string.", nameof(key));
            }

            _key = Convert.FromBase64String(key);

            // Initialization cannot be an empty string, but null is allowed. Null will be replaced with a random value.
            if (initializationVector == null)
            {
                return;
            }

            if (initializationVector.Trim().Length == 0)
            {
                throw new ArgumentException(
                    $"Parameter {nameof(initializationVector)} cannot be an empty string.",
                    nameof(initializationVector));
            }

            _initializationVector = Convert.FromBase64String(initializationVector);
        }

        public string Encrypt(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(value)} cannot be null.", nameof(value));
            }

            // Do not extract this to a separate function unless you want to deal
            // with disposal of the object properly.
            using var encryptor = Aes.Create();

            if (encryptor == null)
            {
                throw new InvalidOperationException("Creation of an AES encryption object failed.");
            }

            var iv = Convert.ToBase64String(SetupKeyAndIV(encryptor));
            var encrypted = EncryptValue(encryptor);
            var signature = CalculateHashValue(encrypted);

            return $"{iv}.{encrypted}.{signature}";

            byte[] SetupKeyAndIV(Aes aes)
            {
                aes.Key = _key;
                var vector = _initializationVector;

                if (vector != null)
                {
                    aes.IV = vector;
                }
                else
                {
                    aes.GenerateIV();
                    vector = aes.IV;
                }

                return vector;
            }

            string EncryptValue(Aes aes)
            {
                using var memStream = new MemoryStream();

                using (var cryptoStream = new CryptoStream(
                    memStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var b = Encoding.Unicode.GetBytes(value);
                    cryptoStream.Write(b);
                }

                return Convert.ToBase64String(memStream.ToArray());
            }
        }

        public bool TryDecrypt(string value, out string decryptedValue)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(value)} cannot be null.", nameof(value));
            }

            if (value.Trim().Length == 0)
            {
                throw new ArgumentException(
                    $"Parameter {nameof(value)} cannot be an empty string.", nameof(value));
            }

            var components = ExtractComponents(value);
            ValidateSignature(components);
            decryptedValue = Decrypt(components);

            return true;

            // TODO: think about what conditions would cause False
            // Probably just try catch and return false
            // Must carefully detect Key or IV errors so that we can throw a proper exception rather than returning false

            (string iv, string encrypted, string signature) ExtractComponents(string localValue)
            {
                var split = localValue.Split('.');

                if (split.Length != 3)
                {
                    throw new InvalidOperationException(
                        "Unable to decrypt the string because it is not signed properly.");
                }

                return (split[0], split[1], split[2]);
            }

            void ValidateSignature((string iv, string encrypted, string signature) localComp)
            {
                if (CalculateHashValue(localComp.encrypted) != localComp.signature)
                {
                    throw new InvalidOperationException("Signatures do not match.");
                }
            }

            string Decrypt((string iv, string encrypted, string signature) localComp)
            {
                using var aes = Aes.Create();

                if (aes == null)
                {
                    throw new InvalidOperationException("Creation of an AES encryption object failed.");
                }

                aes.Key = _key;
                aes.IV = Convert.FromBase64String(components.iv);

                var b = Convert.FromBase64String(localComp.encrypted);

                using var memStream = new MemoryStream();

                using (var cryptoStream = new CryptoStream(
                    memStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(b, 0, b.Length);
                }

                return Encoding.Unicode.GetString(memStream.ToArray());
            }
        }

        private string CalculateHashValue(string value)
        {
            var textBytes = Encoding.Unicode.GetBytes(value);
            var keyBytes = _key;

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
