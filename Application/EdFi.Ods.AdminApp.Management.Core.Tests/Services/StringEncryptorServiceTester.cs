// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text;
using EdFi.Ods.AdminApp.Management.Services;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Services
{
    [TestFixture]
    public class StringEncryptorServiceTester
    {
        private readonly byte[] _optionalEntropyValue;
        private const string StringValueForEncryption = "stringValueForEncryption";
        private Mock<IEncryptionConfigurationProviderService> _encryptionConfigurationProviderServiceWithEntropy;
        private Mock<IEncryptionConfigurationProviderService> _encryptionConfigurationProviderServiceWithNoEntropy;
        private StringEncryptorService _stringEncryptorServiceWithEntropy;
        private StringEncryptorService _stringEncryptorServiceWithNoEntropy;

        public StringEncryptorServiceTester()
        {
            _optionalEntropyValue = Encoding.ASCII.GetBytes("OptionalEntropyValue");
        }

        [SetUp]
        public void Setup()
        {
            _encryptionConfigurationProviderServiceWithEntropy = new Mock<IEncryptionConfigurationProviderService>();
            _encryptionConfigurationProviderServiceWithEntropy.Setup(x => x.GetEntropy()).Returns(_optionalEntropyValue);
            _stringEncryptorServiceWithEntropy = new StringEncryptorService(_encryptionConfigurationProviderServiceWithEntropy.Object);

            _encryptionConfigurationProviderServiceWithNoEntropy = new Mock<IEncryptionConfigurationProviderService>();
            _encryptionConfigurationProviderServiceWithNoEntropy.Setup(x => x.GetEntropy()).Returns((byte[])null);
            _stringEncryptorServiceWithNoEntropy = new StringEncryptorService(_encryptionConfigurationProviderServiceWithNoEntropy.Object);
        }

        [Test]
        public void ShouldValidateEntropyMocks()
        {
            _encryptionConfigurationProviderServiceWithEntropy.Object.GetEntropy().ShouldBe(_optionalEntropyValue);
            _encryptionConfigurationProviderServiceWithNoEntropy.Object.GetEntropy().ShouldBe(null);
        }

        [Test]
        public void ShouldWriteEncryptReadDecryptWithEntropy()
        {
            var encryptedValue = _stringEncryptorServiceWithEntropy.Encrypt(StringValueForEncryption);
            string decryptedValue;
            var result = _stringEncryptorServiceWithEntropy.TryDecrypt(encryptedValue, out decryptedValue);

            encryptedValue.ShouldNotBe(StringValueForEncryption);
            decryptedValue.ShouldBe(StringValueForEncryption);
        }

        [Test]
        public void ShouldWriteEncryptReadDecryptWithNoEntropy()
        {
            var encryptedValue = _stringEncryptorServiceWithNoEntropy.Encrypt(StringValueForEncryption);
            string decryptedValue;
            var result = _stringEncryptorServiceWithNoEntropy.TryDecrypt(encryptedValue, out decryptedValue);

            encryptedValue.ShouldNotBe(StringValueForEncryption);
            decryptedValue.ShouldBe(StringValueForEncryption);
        }

        [Test]
        public void ShouldWriteEncryptWithAndWithNoEntropy()
        {
            var encryptedValueWithEntropy = _stringEncryptorServiceWithEntropy.Encrypt(StringValueForEncryption);
            var encryptedValueWithNoEntropy = _stringEncryptorServiceWithNoEntropy.Encrypt(StringValueForEncryption);

            encryptedValueWithEntropy.ShouldNotBe(encryptedValueWithNoEntropy);
        }
    }
}
