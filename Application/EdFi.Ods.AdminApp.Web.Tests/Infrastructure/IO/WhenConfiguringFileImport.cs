// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Web.Tests.Infrastructure.IO
{
    [TestFixture]
    public class WhenConfiguringFileImport
    {
        private OdsApiConnectionInformation _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox )
        {
            ApiServerUrl = "abc",
            OAuthUrl = "abc",
            ClientKey = "key",
            ClientSecret = "secret"
        };

        [Test]
        public void GivenNullConnectionInfo_ThenThrowArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var _ = new FileImportConfiguration(string.Empty, string.Empty);
            });
        }

        [Test]
        public void GivenConnectionInfo_ThenOauthKeyPropertyShouldReturnClientKey()
        {
            const string expected = "abc";
            _connectionInfo.ClientKey = expected;

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .OauthKey
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInfo_ThenOauthSecretPropertyShouldReturnClientSecret()
        {
            const string expected = "abc";
            _connectionInfo.ClientSecret = expected;

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .OauthSecret
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInfo_ThenOauthUrlPropertyShouldReturnOAuthUrl()
        {
            const string expected = "abc";
            _connectionInfo.OAuthUrl = expected;

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .OauthUrl
                .ShouldBe(expected);
        }

        [Test]
        public void GivenDataDirectoryFullPathConstructorArg_ThenDataFolderPropertyShouldTakeThatValue()
        {
            const string expected = "abc";

            new FileImportConfiguration(expected, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .DataFolder
                .ShouldBe(expected);
        }

        [Test]
        public void GivenWorkingFolderPathConstructorArg_ThenWorkingFolderPropertyShouldTakeThatValue()
        {
            const string expected = "abc";

            new FileImportConfiguration(string.Empty, expected, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .WorkingFolder
                .ShouldBe(expected);
        }

        [Test]
        public void GivenMaxSimultaneousRequestsConstructorArg_ThenAllThrottlingPropertiesShouldTakeThatValue()
        {
            const int expected = 13;

            var configuration = new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl,
                maxSimultaneousRequests: expected);

            configuration.TaskCapacity.ShouldBe(expected);
            configuration.ConnectionLimit.ShouldBe(expected);
            configuration.MaxSimultaneousRequests.ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInfo_ThenMetadataUrlPropertyReturnMetadataUrl()
        {
            const string apiServerUrl = "http://abc";
            const string oauthUrl = "http://abc";
            const string expected = "http://abc/metadata";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = apiServerUrl,
                OAuthUrl = oauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .MetadataUrl
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInfoIsUsingYearSpecificMode_ThenMetadataUrlShouldBeBaseUrlPlusMetadataRoutePlusYear()
        {
            const string apiServerUrl = "http://abc";
            const string oauthUrl = "http://abc";
            const int year = 123;
            const string expected = "http://abc/metadata/123";

            _connectionInfo = new OdsApiConnectionInformation("Ed_Fi_Ods_123", ApiMode.YearSpecific)
            {
                ApiServerUrl = apiServerUrl,
                OAuthUrl = oauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .MetadataUrl
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInformation_ThenApiUrlShouldBeTheCompleteUrlWithVersionNumber()
        {
            const string apiServerUrl = "abc";
            const string oauthUrl = "abc";
            const string expected = "abc/data/v3/";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = apiServerUrl,
                OAuthUrl = oauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .ApiUrl
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInformationIsUsingYearSpecificMode_ThenSchoolYearShouldReturnTheYearSpecificSchoolYear()
        {
            const int year = 1234;

            _connectionInfo = new OdsApiConnectionInformation("Ed_Fi_Ods_1234", ApiMode.YearSpecific)
            {
                ApiServerUrl = "abc",
                OAuthUrl = "abc",
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, year, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .SchoolYear
                .ShouldBe(year);
        }

        [Test]
        public void GivenConnectionInformationIsNotUsingYearSpecificMode_ThenSchoolYearShouldReturnNull()
        {
            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .SchoolYear
                .ShouldBeNull();
        }

        [Test]
        public void GivenConnectionInfo_ThenDependenciesUrlShouldBeTheCompleteUrlWithExpectedSegments()
        {
            const string apiServerUrl = "http://abc";
            const string oauthUrl = "http://abc";
            const string expected = "http://abc/metadata/data/v3/dependencies";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = apiServerUrl,
                OAuthUrl = oauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .DependenciesUrl
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInfoIsNotUsingYearSpecificMode_ThenDependenciesUrlShouldNotContainYear()
        {
            const string apiServerUrl = "http://abc";
            const string oauthUrl = "http://abc";
            const string expected = "http://abc/metadata/data/v3/dependencies";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = apiServerUrl,
                OAuthUrl = oauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .DependenciesUrl
                .ShouldBe(expected);
        }

        [Test]
        public void GivenConnectionInfoIsUsingYearSpecificMode_ThenDependenciesUrlShouldContainYear()
        {
            const string apiServerUrl = "http://abc";
            const string oauthUrl = "http://abc";
            const string expected = "http://abc/metadata/data/v3/2019/dependencies";

            _connectionInfo = new OdsApiConnectionInformation("Ed_Fi_Ods_2019", ApiMode.YearSpecific)
            {
                ApiServerUrl = apiServerUrl,
                OAuthUrl = oauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            new FileImportConfiguration(string.Empty, string.Empty, null, _connectionInfo.ApiBaseUrl, _connectionInfo.ClientKey, _connectionInfo.ClientSecret, _connectionInfo.OAuthUrl, _connectionInfo.MetadataUrl, _connectionInfo.DependenciesUrl)
                .DependenciesUrl
                .ShouldBe(expected);
        }
    }
}
