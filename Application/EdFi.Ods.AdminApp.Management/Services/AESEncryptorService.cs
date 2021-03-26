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
    public class AESEncryptorService : IStringEncryptorService
    {
        private const int ExpectedKeyArrayLength = 32;
        private const int ExpectedIvArrayLength = 16;
        private const string InvalidKey =
            "Invalid encryption key: must be a 256 bit byte array encoded as a Base64 string";
        private const string InvalidIV =
            "Invalid encryption initialization vector: must be a 128 bit byte array encoded as a Base64 string";

        private readonly byte[] _initializationVector;
        private readonly byte[] _key;

        public AESEncryptorService(string key, string initializationVector = null)
        {
            _key = ValidateKey(key);
            _initializationVector = ValidateIV(initializationVector);

            static byte[] ValidateKey(string key1)
            {
                if (key1 == null)
                {
                    throw new ArgumentNullException(
                        $"Parameter {nameof(key1)} cannot be null.", nameof(key1));
                }

                if (key1.Trim().Length == 0)
                {
                    throw new ArgumentException(
                        $"Parameter {nameof(key1)} cannot be an empty string.", nameof(key1));
                }

                try
                {
                    var k = Convert.FromBase64String(key1);

                    if (k.Length != ExpectedKeyArrayLength)
                    {
                        throw new ArgumentException(InvalidKey);
                    }

                    return k;
                }
                catch (FormatException)
                {
                    throw new ArgumentException(InvalidKey);
                }
            }

            static byte[] ValidateIV(string s)
            {
                // Initialization cannot be an empty string, but null is allowed. Null will be replaced with a random value.
                if (s == null)
                {
                    return null;
                }

                if (s.Trim().Length == 0)
                {
                    throw new ArgumentException(
                        $"Parameter {nameof(s)} cannot be an empty string.",
                        nameof(s));
                }

                try
                {
                    var iv = Convert.FromBase64String(s);

                    if (iv.Length != ExpectedIvArrayLength)
                    {
                        throw new ArgumentException(InvalidIV);
                    }

                    return iv;
                }
                catch (FormatException)
                {
                    throw new ArgumentException(InvalidIV);
                }
            }
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

            var ivPlusEncrypted = $"{iv}.{encrypted}";
            var signature = CalculateSignature(ivPlusEncrypted);

            return $"{ivPlusEncrypted}.{signature}";

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

            if (SignatureDoesNotMatch(components))
            {
                decryptedValue = value;
                return false;
            }

            try
            {
                decryptedValue = Decrypt(components);
                return true;
            }
            catch (CryptographicException)
            {
                // If we make it this far then there was some severe tampering. We don't want to
                // show the end-user anything, so swallow the exception and just return false.
                decryptedValue = string.Empty;
                return false;
            }

            static (string iv, string encrypted, string signature) ExtractComponents(string localValue)
            {
                var split = localValue.Split('.');

                return split.Length != 3
                    ? (string.Empty, localValue, string.Empty)
                    : (split[0], split[1], split[2]);
            }

            bool SignatureDoesNotMatch((string iv, string encrypted, string signature) localComp)
            {
                return CalculateSignature($"{localComp.iv}.{localComp.encrypted}") != localComp.signature;
            }

            string Decrypt((string iv, string encrypted, string _) localComp)
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

        private string CalculateSignature(string value)
        {
            var textBytes = Encoding.Unicode.GetBytes(value);
            var keyBytes = _key;

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
