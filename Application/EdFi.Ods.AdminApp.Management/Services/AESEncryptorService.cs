using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public class AESEncryptorService : IStringEncryptorService
    {
        private readonly string _key;

        public AESEncryptorService(string key)
        {
            _key = key ?? throw new ArgumentNullException(
                $"Parameter {nameof(key)} cannot be null.", nameof(key));

            if (key.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(key)} cannot be an empty string.", nameof(key));
            }
        }

        public string Encrypt(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(value)} cannot be null.", nameof(value));
            }

            return "b";
        }

        public bool TryDecrypt(string value, out string decryptedValue)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(value)} cannot be null.", nameof(value));
            }

            if (value.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(value)} cannot be an empty string.", nameof(value));
            }

            var split = value.Split('.');

            if (split.Length != 2)
            {
                throw new InvalidOperationException("Unable to decrypt the string because it is not signed properly.");
            }

            var encrypted = split[0];
            var signature = split[1];

            if (HashValue(encrypted) != signature)
            {
                throw new InvalidOperationException("Signatures do not match.");
            }

            decryptedValue = "a";
            return false;
        }

        private string HashValue(string value)
        {
            var encoding = Encoding.UTF8;

            var textBytes = encoding.GetBytes(value);
            var keyBytes = encoding.GetBytes(_key);

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
