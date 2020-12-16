using System;
using EdFi.Ods.AdminApp.Management.Services;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Services
{
    [TestFixture]
    public class AESEncryptorServiceTests
    {
        public const string TestKey = "I am the test key";

        [TestFixture]
        public class WhenInitializingTheService
        {
            [TestFixture]
            public class GivenNullKey
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () => new AESEncryptorService(null);

                    act.ShouldThrow<ArgumentNullException>();
                }
            }

            [TestFixture]
            public class GivenEmptyStringKey
            {
                [Test]
                public void ThenThrowException()
                {
                    Action act = () => new AESEncryptorService("     ");

                    act.ShouldThrow<ArgumentException>();
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
                        new AESEncryptorService(TestKey).Encrypt(null);
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
                        new AESEncryptorService("    ").TryDecrypt(
                            "The actual value here doesn't matter so long as there is no period",
                            out var decryptedValue);
                    };

                    act.ShouldThrow<InvalidOperationException>();
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
                        new AESEncryptorService(TestKey).TryDecrypt(
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
                        new AESEncryptorService(TestKey).TryDecrypt(
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
                            new AESEncryptorService(TestKey).TryDecrypt(
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
                            new AESEncryptorService(TestKey).TryDecrypt(

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
                    private const string Input = "a.b";
                    private const string Value = "c";
                    private bool _result;
                    private string _decryptedValue;

                    [SetUp]
                    public void Fixture()
                    {
                        _result = new AESEncryptorService(TestKey).TryDecrypt(Input, out _decryptedValue);
                    }

                    [Test]
                    public void ThenIsSuccessful()
                    {
                        _result.ShouldBeTrue();
                    }

                    [Test]
                    public void ThenDecryptsValue()
                    {
                        _decryptedValue.ShouldBe(Value);
                    }
                }
            }
        }
    }
}
