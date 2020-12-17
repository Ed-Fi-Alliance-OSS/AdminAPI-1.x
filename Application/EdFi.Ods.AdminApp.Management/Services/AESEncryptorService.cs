using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EdFi.Ods.AdminApp.Management.Services
{
    // ReSharper disable once InconsistentNaming - AES is proper, not Aes
    public class AESEncryptorService : IStringEncryptorService
    {
        private readonly byte[] _key;
        private readonly string _initializationVector;

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
                throw new ArgumentException($"Parameter {nameof(key)} cannot be an empty string.", nameof(key));
            }

            _key = GetBytes(key);

            // Null is acceptable here - if value is not passed in then we'll generate a random one.
            _initializationVector = initializationVector;
        }

        public string Encrypt(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Parameter {nameof(value)} cannot be null.", nameof(value));
            }

            // Do not extract this to a separate function unless you want to deal
            // with disposal of the object properly.
            using var aes = Aes.Create();
            if (aes == null)
            {
                throw new InvalidOperationException("Creation of an AES encryption object failed.");
            }

            aes.Key = _key;

            var iv = _initializationVector;

            if (iv != null)
            {
                aes.IV = GetBytes(iv);
            }
            else
            {
                aes.GenerateIV();
                iv = GetString(aes.IV);
            }

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(GetBytes(value));
            }

            var encrypted = GetString(memStream.ToArray());
            var signature = CalculateHashValue(encrypted);

            return $"{iv}.{encrypted}.{signature}";
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

            var split = value.Split('.');

            if (split.Length != 3)
            {
                throw new InvalidOperationException(
                    "Unable to decrypt the string because it is not signed properly.");
            }

            var iv = split[0];
            var encrypted = split[1];
            var signature = split[2];

            if (CalculateHashValue(encrypted) != signature)
            {
                throw new InvalidOperationException("Signatures do not match.");
            }

            using var aes = Aes.Create();
            if (aes == null)
            {
                throw new InvalidOperationException("Creation of an AES encryption object failed.");
            }

            aes.Key = _key;
            aes.IV = GetBytes(iv);

            var decryptor = aes.CreateDecryptor();
            var memStream = new MemoryStream(GetBytes(value));
            using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);

            decryptedValue =reader.ReadToEnd();
            return true;

            // TODO: think about what conditions would cause False 
        }

        private string CalculateHashValue(string value)
        {
            var textBytes = GetBytes(value);
            var keyBytes = _key;

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);
            return GetString(hashBytes);
        }

        private byte[] GetBytes(string value)
        {
            // TODO: do I need to be concerned about spaces and plus sign?
            return Encoding.ASCII.GetBytes(value);
        }

        private string GetString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
