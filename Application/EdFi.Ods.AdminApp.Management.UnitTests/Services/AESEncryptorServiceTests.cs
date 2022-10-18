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
        public const string TestKey = "bEnFYNociET2R1Wua3DHzwfU5u/Fa47N5fw0PXD0OSI=";
        public const string TestKeyWithoutEqualSign = "bEnFYNociET2R1Wua3DHzwfU5u/Fa47N5fw0PXD0OSI";
        public const string TestIv = "gpkAmCb03SJVbfBF0k798g==";
        public const string TestIvWithoutEqual = "gpkAmCb03SJVbfBF0k798g=";

        public const string EmptyString = "";
        public const string EmptyStringEncrypted =
            "gpkAmCb03SJVbfBF0k798g==.oByW0GY03625pqMhLnYhaA==.VV3+VeKNqx7G3wncK5hkRk0adh2IQvC1Pv/AjSzpljw=";
        public const string EncryptMe = "encrypt me";
        public const string EncryptMeEncrypted =
            "gpkAmCb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQvo=.NjnA2fW8eLeb91duEacfniHX8Mdxc8B0Gi1VDcOK6ro=";

        public const string EncryptMeTamperedIv =
            "hrkAmDb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQvo=.NjnA2fW8eLeb91duEacfniHX8Mdxc8B0Gi1VDcOK6ro=";
        public const string EncryptMeTamperedIvResigned =
            "hrkAmDb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQvo=.CgIafIx3LugLec+IDUi4OUokI8qNl9J5ESqI/oIlNsA=";
        public const string EncryptMeTamperedEncryptedValue =
            "gpkAmCb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQv==.NjnA2fW8eLeb91duEacfniHX8Mdxc8B0Gi1VDcOK6ro=";
        public const string EncryptMeTamperedEncryptedValueResigned =
            "gpkAmCb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQv==.douF1+uirC3PCgrOAAaZokepWH+WEXvZNGvhgzijN9w=";
        public const string EncryptMeTamperedSignature =
            "gpkAmCb03SJVbfBF0k798g==.y2QnYoan80w8G8iDWRVxsEJqyqRGqwUwzCiiOH/NQvo=.NjnA2fW8eLeb91duEacfniHX8Mdxc8B0Gi1VDcOK6r==";

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
            public class GivenKeyIsNotAValidValue
            {
                [TestCase("    ")]
                [TestCase("")]
                [TestCase("This is not a valid key")]
                [TestCase(TestKeyWithoutEqualSign)]
                [TestCase(TestIv)]
                public void ThenThrowException(string key)
                {
                    Action act = () => new AESEncryptorService(key, TestIv);

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

            [TestFixture]
            public class GivenInitializationVectorIsNotAValidValue
            {
                [TestCase("    ")]
                [TestCase("")]
                [TestCase("This is not a IV")]
                [TestCase(TestKey)]
                [TestCase(TestIvWithoutEqual)]
                public void ThenThrowException(string iv)
                {
                    Action act = () => new AESEncryptorService(TestKey, iv);

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
                            "The actual value here doesn't matter so long as.there are.more.than.two.periods",
                            out var decryptedValue);
                    };

                    act.ShouldThrow<InvalidOperationException>();
                }
            }

            [TestFixture]
            public class GivenValidLookingInput
            {
                [TestFixture]
                public class AndStringHasBeenTamperedWith
                {
                    [TestCase(EncryptMeTamperedIv)]
                    [TestCase(EncryptMeTamperedEncryptedValue)]
                    [TestCase(EncryptMeTamperedSignature)]
                    [TestCase(EncryptMeTamperedEncryptedValueResigned)]
                    public void ThenReturnFalse(string encrypted)
                    {
                        var result =
                            new AESEncryptorService(TestKey).TryDecrypt(encrypted, out var decryptedValue);

                        result.ShouldBeFalse(decryptedValue);
                    }
                }

                [TestFixture]
                public class AndIVHasBeenChangedAndSigned
                {
                    [Test]
                    public void ThenTheWrongValueIsDecrypted()
                    {
                        // If someone gets hold of the key and is thus able to re-sign contents then a modified IV
                        // will cause the decrypted value to be incorrect - but there's no way to know in code.
                        // Of course if they have the key then they're in the clear anyway! Only preserving
                        // this test to acknowledge how the mechanism works.

                        var result =
                            new AESEncryptorService(TestKey).TryDecrypt(
                                EncryptMeTamperedIvResigned, out var decryptedValue);

                        result.ShouldBeTrue();
                        decryptedValue.ShouldNotBe(EncryptMe);
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

        [TestFixture]
        public class WhenTestingEndToEnd
        {
            [Test]
            public void ThenEncryptDecryptWorkTogetherSuccessfully()
            {
                var input = "What’s in a name? A rose by any other name would smell as sweet.";

                var service = new AESEncryptorService(TestKey);

                var encrypted = service.Encrypt(input);

                var result = service.TryDecrypt(encrypted, out string decrypted);

                result.ShouldBeTrue();
                decrypted.ShouldBe(input);
            }
        }
    }
}
