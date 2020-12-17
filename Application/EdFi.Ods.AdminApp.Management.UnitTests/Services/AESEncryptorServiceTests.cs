// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
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
        // TODO: make sure the key & iv is the right length and is base 64

        public const string TestKey = "bEnFYNociET2R1Wua3DHzwfU5u/Fa47N5fw0PXD0OSI=";
        public const string TestIv = "gpkAmCb03SJVbfBF0k798g==";

        public const string EmptyString = "";
        public const string EmptyStringEncrypted =
            "gpkAmCb03SJVbfBF0k798g==.oByW0GY03625pqMhLnYhaA==.+7d9P9AdOgGbUCUut2JccVJm6nipi1CNcocz+fPBFhY=";
        public const string EncryptMe = "encrypt me";
        public const string EncryptMeEncrypted =
            "gpkAmCb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQvo=.KWELi015RE6yNDMctJxALrugYldkEdDTYie6r/f7dQI=";

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
                    Action act = () => { new AESEncryptorService(TestKey, TestIv).Encrypt(null); };

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
                        new AESEncryptorService(TestKey, TestIv).TryDecrypt(
                            "     ", out var decryptedValue);
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
                        var result = new AESEncryptorService(TestKey).TryDecrypt(
                            encrypted, out var decrypted);

                        result.ShouldBeTrue();
                        decrypted.ShouldBe(input);
                    }
                }
            }
        }
    }
}
