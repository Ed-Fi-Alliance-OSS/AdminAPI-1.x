// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Infrastructure.IO
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

        private readonly BulkUploadJobContext _bulkUploadJobContext = new BulkUploadJobContext();
        private readonly FileImportConfiguration _fileImportConfiguration = new FileImportConfiguration();
        private const string Expected = "abc";

        [SetUp]
        public void SetUp()
        {
            _bulkUploadJobContext.ApiBaseUrl = _connectionInfo.ApiBaseUrl;
            _bulkUploadJobContext.ClientKey = _connectionInfo.ClientKey;
            _bulkUploadJobContext.ClientSecret = _connectionInfo.ClientSecret;
            _bulkUploadJobContext.OauthUrl = _connectionInfo.OAuthUrl;
            _bulkUploadJobContext.MetadataUrl = _connectionInfo.MetadataUrl;
            _bulkUploadJobContext.DependenciesUrl = _connectionInfo.DependenciesUrl;
        }

        [Test]
        public void GivenNullConnectionInfo_ThenThrowArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _fileImportConfiguration.SetConfiguration(new BulkUploadJobContext(), string.Empty);
            });
        }

        [Test]
        public void GivenConnectionInfo_ThenOauthKeyPropertyShouldReturnClientKey()
        {
            _connectionInfo.ClientKey = Expected;
            _bulkUploadJobContext.ClientKey = _connectionInfo.ClientKey;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)
                ["OdsApi:Key"].ShouldBe(Expected);
        }

        [Test]
        public void GivenConnectionInfo_ThenOauthSecretPropertyShouldReturnClientSecret()
        {
            _connectionInfo.ClientSecret = Expected;
            _bulkUploadJobContext.ClientSecret = _connectionInfo.ClientSecret;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:Secret"]
                .ShouldBe(Expected);
        }

        [Test]
        public void GivenConnectionInfo_ThenOauthUrlPropertyShouldReturnOAuthUrl()
        {
            _connectionInfo.OAuthUrl = Expected;

            _bulkUploadJobContext.OauthUrl = _connectionInfo.OAuthUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:OAuthUrl"]
                .ShouldBe(Expected);
        }

        [Test]
        public void GivenDataDirectoryFullPathConstructorArg_ThenDataFolderPropertyShouldTakeThatValue()
        {
            _bulkUploadJobContext.DataDirectoryFullPath = Expected;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["Folders:Data"]
                .ShouldBe(Expected);
        }

        [Test]
        public void GivenWorkingFolderPathConstructorArg_ThenWorkingFolderPropertyShouldTakeThatValue()
        {
            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, Expected)["Folders:Working"]
                .ShouldBe(Expected);
        }

        [Test]
        public void GivenMaxSimultaneousRequestsConstructorArg_ThenAllThrottlingPropertiesShouldTakeThatValue()
        {
            const int ExpectedValue = 13;
            _bulkUploadJobContext.MaxSimultaneousRequests = ExpectedValue;

            var configuration = _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, Expected);
            configuration.GetValue<int>("Concurrency:ConnectionLimit").ShouldBe(ExpectedValue);
            configuration.GetValue<int>("Concurrency:MaxSimultaneousApiRequests").ShouldBe(ExpectedValue);
            configuration.GetValue<int>("Concurrency:TaskCapacity").ShouldBe(ExpectedValue);
        }

        [Test]
        public void GivenConnectionInfo_ThenMetadataUrlPropertyReturnMetadataUrl()
        {
            const string ApiServerUrl = "http://abc";
            const string OauthUrl = "http://abc";
            const string ExpectedUrl = "http://abc/metadata";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = ApiServerUrl,
                OAuthUrl = OauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };
            _bulkUploadJobContext.MetadataUrl = _connectionInfo.MetadataUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:MetadataUrl"]
                .ShouldBe(ExpectedUrl);
        }

        [Test]
        public void GivenConnectionInfoIsUsingYearSpecificMode_ThenMetadataUrlShouldBeBaseUrlPlusMetadataRoutePlusYear()
        {
            const string ApiServerUrl = "http://abc";
            const string OauthUrl = "http://abc";
            const string ExpectedUrl = "http://abc/metadata/123";

            _connectionInfo = new OdsApiConnectionInformation("Ed_Fi_Ods_123", ApiMode.YearSpecific)
            {
                ApiServerUrl = ApiServerUrl,
                OAuthUrl = OauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };
            _bulkUploadJobContext.MetadataUrl = _connectionInfo.MetadataUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:MetadataUrl"]
                .ShouldBe(ExpectedUrl);
        }

        [Test]
        public void GivenConnectionInformation_ThenApiUrlShouldBeTheCompleteUrlWithVersionNumber()
        {
            const string ApiServerUrl = "abc";
            const string OauthUrl = "abc";
            const string ExpectedUrl = "abc/data/v3/";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = ApiServerUrl,
                OAuthUrl = OauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            _bulkUploadJobContext.ApiBaseUrl = _connectionInfo.ApiBaseUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:ApiUrl"]
                .ShouldBe(ExpectedUrl);
        }

        [Test]
        public void GivenConnectionInformationIsUsingYearSpecificMode_ThenSchoolYearShouldReturnTheYearSpecificSchoolYear()
        {
             int? year = 1234;
             Startup.ConfigurationAppSettings.ApiStartupType = "YearSpecific";
            _bulkUploadJobContext.OdsInstanceName = "Ed_Fi_Ods_1234";

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:SchoolYear"]
                .ShouldBe(year.ToString());
        }

        [Test]
        public void GivenConnectionInformationIsNotUsingYearSpecificMode_ThenSchoolYearShouldReturnEmpty()
        {
            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:SchoolYear"]
                .ShouldBe(string.Empty);
        }

        [Test]
        public void GivenConnectionInfo_ThenDependenciesUrlShouldBeTheCompleteUrlWithExpectedSegments()
        {
            const string ApiServerUrl = "http://abc";
            const string OauthUrl = "http://abc";
            const string ExpectedUrl = "http://abc/metadata/data/v3/dependencies";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = ApiServerUrl,
                OAuthUrl = OauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            _bulkUploadJobContext.DependenciesUrl = _connectionInfo.DependenciesUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:DependenciesUrl"]
                .ShouldBe(ExpectedUrl);
        }

        [Test]
        public void GivenConnectionInfoIsNotUsingYearSpecificMode_ThenDependenciesUrlShouldNotContainYear()
        {
            const string ApiServerUrl = "http://abc";
            const string OauthUrl = "http://abc";
            const string ExpectedUrl = "http://abc/metadata/data/v3/dependencies";

            _connectionInfo = new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = ApiServerUrl,
                OAuthUrl = OauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            _bulkUploadJobContext.DependenciesUrl = _connectionInfo.DependenciesUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:DependenciesUrl"]
                .ShouldBe(ExpectedUrl);
        }

        [Test]
        public void GivenConnectionInfoIsUsingYearSpecificMode_ThenDependenciesUrlShouldContainYear()
        {
            const string ApiServerUrl = "http://abc";
            const string OauthUrl = "http://abc";
            const string ExpectedUrl = "http://abc/metadata/data/v3/2019/dependencies";

            _connectionInfo = new OdsApiConnectionInformation("Ed_Fi_Ods_2019", ApiMode.YearSpecific)
            {
                ApiServerUrl = ApiServerUrl,
                OAuthUrl = OauthUrl,
                ClientKey = "key",
                ClientSecret = "secret"
            };

            _bulkUploadJobContext.DependenciesUrl = _connectionInfo.DependenciesUrl;

            _fileImportConfiguration.SetConfiguration(_bulkUploadJobContext, string.Empty)["OdsApi:DependenciesUrl"]
                .ShouldBe(ExpectedUrl);
        }
    }
}
