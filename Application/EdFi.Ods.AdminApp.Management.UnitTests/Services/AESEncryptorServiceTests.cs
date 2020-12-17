using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using EdFi.Ods.AdminApp.Management.Services;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Services
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming - AES is proper, not Aes
    public class AESEncryptorServiceTests
    {
        // TODO: will need to have one test where the IV is calculated at random,
        // and we won't know what the actual result is.

        // TODO: make sure the key is the right length

        public const string TestKey = "bEnFYNociET2R1Wua3DHzwfU5u/Fa47N5fw0PXD0OSI=";
        public const string TestIv = "gpkAmCb03SJVbfBF0k798g==";

        public const string EmptyString = "";
        public const string EmptyStringEncrypted =
            "gpkAmCb03SJVbfBF0k798g==.oByW0GY03625pqMhLnYhaA==.+7d9P9AdOgGbUCUut2JccVJm6nipi1CNcocz+fPBFhY=";
        public const string EncryptMe = "encrypt me";
        public const string EncryptMeEncrypted =
            "gpkAmCb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQvo=.KWELi015RE6yNDMctJxALrugYldkEdDTYie6r/f7dQI=";

        [Test]
        public void Whatever()
        {
            string original = EncryptMe;

            var key = Convert.FromBase64String(TestKey);
            var iv = Convert.FromBase64String(TestIv);

            // Encrypt the string to an array of bytes.
            byte[] encrypted = EncryptStringToBytes_Aes(original, key, iv);

            // Decrypt the bytes to a string.
            string roundtrip = DecryptStringFromBytes_Aes(encrypted, key, iv);

            roundtrip.ShouldBe(original);

        }
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }



        [TestFixture]
        public class WhenInitializingTheService
        {
            [TestFixture]
            public class GivenNullKey
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () => new AESEncryptorService(null, TestIv);

                    act.ShouldThrow<ArgumentNullException>();
                }
            }

            [TestFixture]
            public class GivenEmptyStringKey
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () => new AESEncryptorService("     ", TestIv);

                    act.ShouldThrow<ArgumentException>();
                }
            }

            [TestFixture]
            public class GivenNullInitializationVector
            {
                [Test]
                public void ThenAcceptIt()
                {
                    Action act = () => new AESEncryptorService(TestKey, null);

                    act.ShouldNotThrow();
                }
            }

            [TestFixture]
            public class GivenEmptyStringInitializationVector
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () => new AESEncryptorService(TestKey, "     ");

                    act.ShouldThrow<ArgumentException>();
                }
            }
        }

        [TestFixture]
        public class WhenEncryptingAString
        {

            [TestFixture]
            public class GivenNullInput
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () =>
                    {
                        new AESEncryptorService(TestKey, TestIv).Encrypt(null);
                    };

                    act.ShouldThrow<ArgumentNullException>();
                }
            }

            [TestFixture]
            public class GivenValidInput
            {
                [TestCase(EmptyString, EmptyStringEncrypted)]
                [TestCase(EncryptMe, EncryptMeEncrypted)]
                public void ThenItShouldEncryptTheValueAndAppendSignature(string input, string expected)
                {
                    var result = new AESEncryptorService(TestKey, TestIv).Encrypt(input);

                    result.ShouldBe(expected);
                }
            }
        }

        [TestFixture]
        public class WhenDecryptingAString
        {
            [TestFixture]
            public class GivenNullInput
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () =>
                    {
                        new AESEncryptorService(TestKey, TestIv).TryDecrypt(null, out var decryptedValue);
                    };

                    act.ShouldThrow<ArgumentNullException>();
                }
            }

            [TestFixture]
            public class GivenEmptyString
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () =>
                    {
                        new AESEncryptorService(TestKey, TestIv).TryDecrypt("     ", out var decryptedValue);
                    };

                    act.ShouldThrow<ArgumentException>();
                }
            }

            [TestFixture]
            public class GivenValueWithoutSignature
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () =>
                    {
                        new AESEncryptorService(TestKey, TestIv).TryDecrypt(
                            "The actual value here doesn't matter so long as there is no period",
                            out var decryptedValue);
                    };

                    act.ShouldThrow<InvalidOperationException>();
                }
            }

            [TestFixture]
            public class GivenTooManyPeriods
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () =>
                    {
                        new AESEncryptorService(TestKey, TestIv).TryDecrypt(
                            "The actual value here doesn't matter so long as.there are.multiple periods",
                            out var decryptedValue);
                    };

                    act.ShouldThrow<InvalidOperationException>();
                }
            }

            [TestFixture]
            public class GivenSignatureExists
            {
                [TestFixture]
                public class AndValueIsNotBase64Encoded
                {
                    [Test]
                    public void ThenThrowException()
                    {
                        Action act = () =>
                        {
                            new AESEncryptorService(TestKey, TestIv).TryDecrypt(
                                // missing the padding for base 64
                                "aW52YWxpZCBzaWduYXR1cmU.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                                out var decryptedValue);
                        };

                        act.ShouldThrow<InvalidOperationException>();
                    }
                }

                [TestFixture]
                public class AndValueDoesNotMatchSignature
                {
                    [Test]
                    public void ThenThrowException()
                    {
                        Action act = () =>
                        {
                            new AESEncryptorService(TestKey, TestIv).TryDecrypt(

                                // Made up signature
                                "aW52YWxpZCBzaWduYXR1cmU=.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                                out var decryptedValue);
                        };

                        act.ShouldThrow<InvalidOperationException>();
                    }
                }

                [TestFixture]
                public class AndValueMatchesSignature
                {
                    [TestCase(EmptyString, EmptyStringEncrypted)]
                    [TestCase(EncryptMe, EncryptMeEncrypted)]
                    public void ThenItShouldEncryptTheValueAndAppendSignature(string input, string encrypted)
                    {
                        var result = new AESEncryptorService(TestKey).TryDecrypt(encrypted, out var decrypted);

                        result.ShouldBeTrue();
                        decrypted.ShouldBe(input);
                    }
                }
            }
        }
    }
}
