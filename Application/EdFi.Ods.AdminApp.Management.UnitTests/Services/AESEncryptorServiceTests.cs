using System;
using System.Runtime.InteropServices.ComTypes;
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

        public const string TestKey = "98c1c51317be80fefda9472e61465c06";
        public const string TestIv = "115d035ef1dd60ba";

        public const string EmptyString = "";
        public const string EmptyStringEncrypted =
            "115d035ef1dd60ba.cb7a31011a52fb26b62428593bca05fc.4ee89ac5e034d3b980fe945d1f63e36409d685e64770810b1b0396f12ed4ac9e";
        public const string EncryptMe = "encrypt me";
        public const string EncryptMeEncrypted =
            "115d035ef1dd60ba.f142ab79d72e81412a014a1e1f898a53.13ff7957290bf2469ffb5b1cc36e22c21437e51982a1b4ac2a0bc8eebc7ecc0f";

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
                public void ThenThrowException()
                {
                    Action act = () => new AESEncryptorService(TestKey, null);

                    act.ShouldThrow<ArgumentNullException>();
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
