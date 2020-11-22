// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Tests.Infrastructure.CloudOdsApiConnectionInformationProviderTests
{
    [TestFixture]
    public class WhenGettingApiCredentials
    {
        private Mock<IGetOdsAdminAppApiCredentialsQuery> _mockQuery;
        private Mock<ICloudOdsAdminAppSettingsApiModeProvider> _mockApiProvider;
        private CloudOdsApiConnectionInformationProvider _system;

        [SetUp]
        public void SetUp()
        {
            _mockQuery = new Mock<IGetOdsAdminAppApiCredentialsQuery>();
            _mockApiProvider = new Mock<ICloudOdsAdminAppSettingsApiModeProvider>();
            _system = new CloudOdsApiConnectionInformationProvider(_mockQuery.Object, new InstanceContext(), _mockApiProvider.Object);
        }

        protected Task<OdsApiConnectionInformation> Run()
        {
            return _system.GetConnectionInformationForEnvironment();
        }

        protected OdsApiConnectionInformation Run(OdsApiCredential apiCredentials)
        {
            return CloudOdsApiConnectionInformationProvider.GetConnectionInformationForEnvironment(apiCredentials, "Ods Instance", ApiMode.Sandbox);
        }

        [Test]
        public void GivenNoCredentialsExist_ThenThrowInvalidOperationException()
        {
            // Arrange
            _mockQuery.Setup(x => x.Execute()).ReturnsAsync(null as OdsAdminAppApiCredentials);

            // Act
            Task Act() => Run();

            // Assert
            Should.ThrowAsync<InvalidOperationException>(Act);
        }

        [Test]
        public void ForProductionEnvironment_GivenProductionCredentialsAreMissing_ThenThrowInvalidOperationException()
        {
            // Arrange
            _mockQuery.Setup(x => x.Execute()).ReturnsAsync(new OdsAdminAppApiCredentials());

            // Act
            Task Act() => Run();

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(Act);
        }

        [Test]
        public async Task ForProductionEnvironment_GivenProductionCredentialsExist_ThenReturnApiConnectionInformation()
        {
            // Arrange
            const string key = "asdfasdf";
            const string secret = "89798798";
            _mockQuery.Setup(x => x.Execute()).ReturnsAsync(new OdsAdminAppApiCredentials
            {
                ProductionApiCredential = new OdsApiCredential
                {
                    Key = key,
                    Secret = secret
                }

            });

            _mockApiProvider
                .Setup(x => x.GetApiMode())
                .Returns(ApiMode.Sandbox);

            // Act
            var actual = await Run();

            // Assert
            actual.ShouldNotBeNull();
            actual.ClientKey.ShouldBe(key);
            actual.ClientSecret.ShouldBe(secret);
            actual.ApiBaseUrl.ShouldNotBeNullOrWhiteSpace();
            actual.OAuthUrl.ShouldNotBeNullOrWhiteSpace();
            actual.ApiServerUrl.ShouldBeNullOrWhiteSpace();
        }

        [Test]
        public void ForProductionEnvironment_GivenCredentialsAreProvided_ThenReturnApiConnectionInformation()
        {
            // Arrange
            const string key = "asdfasdf";
            const string secret = "89798798";
            var apiCredentials = new OdsApiCredential
            {
                Key = key,
                Secret = secret
            };

            // Act
            var actual = Run(apiCredentials);

            // Assert
            actual.ShouldNotBeNull();
            actual.ClientKey.ShouldBe(key);
            actual.ClientSecret.ShouldBe(secret);
            actual.ApiBaseUrl.ShouldNotBeNullOrWhiteSpace();
            actual.OAuthUrl.ShouldNotBeNullOrWhiteSpace();
            actual.ApiServerUrl.ShouldBeNullOrWhiteSpace();
        }

        [Test]
        public void ForAnyEnvironment_GivenCredentialsAreProvidedButObjectIsNull_ThenThrowArgumentNullException()
        {
            // Act
            void Act() => Run(null);

            // Assert
            Should.Throw<ArgumentNullException>((Action)Act);
        }

        [Test]
        public void ForAnyEnvironment_GivenCredentialsAreProvidedButSecretIsNull_ThenThrowArgumentException()
        {
            // Act
            void Act() => Run(new OdsApiCredential { Key = "as" });

            // Assert
            Should.Throw<ArgumentException>((Action)Act);
        }

        [Test]
        public void ForAnyEnvironment_GivenCredentialsAreProvidedButSecretIsWhitespace_ThenThrowArgumentException()
        {
            // Act
            void Act() => Run(new OdsApiCredential { Key = "as", Secret = "    " });

            // Assert
            Should.Throw<ArgumentException>((Action)Act);
        }

        [Test]
        public void ForAnyEnvironment_GivenCredentialsAreProvidedButKeyIsNull_ThenThrowArgumentException()
        {
            // Act
            void Act() => Run(new OdsApiCredential { Secret = "as" });

            // Assert
            Should.Throw<ArgumentException>((Action)Act);
        }

        [Test]
        public void ForAnyEnvironment_GivenCredentialsAreProvidedButKeyIsWhitespace_ThenThrowArgumentException()
        {
            // Act
            void Act() => Run(new OdsApiCredential { Secret = "as", Key = "    " });

            // Assert
            Should.Throw<ArgumentException>((Action)Act);
        }
    }
}
