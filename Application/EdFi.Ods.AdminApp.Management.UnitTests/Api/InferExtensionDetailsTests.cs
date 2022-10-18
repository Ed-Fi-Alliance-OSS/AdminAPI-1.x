// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Services;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Api
{
    [TestFixture]
    public class InferExtensionDetailsTests
    {
        private const string OdsRootUri = "https://ods.example.com";

        class StubGetRequest : ISimpleGetRequest
        {
            private readonly string _expectedAddress;
            private readonly string _jsonResponse;

            public StubGetRequest(string expectedAddress, string jsonResponse)
            {
                _expectedAddress = expectedAddress;
                _jsonResponse = jsonResponse;
            }

            public Task<string> DownloadString(string address)
            {
                address.ShouldBe(_expectedAddress);
                return Task.FromResult(_jsonResponse);
            }
        }

        private static string ExampleOdsRootDocument(string dataModel, string version)
        {
            return @"{
                ""version"": ""5.2"",
                ""informationalVersion"": ""5.2"",
                ""suite"": ""3"",
                ""build"": ""5.2.14406.0"",
                ""apiMode"": ""Sandbox"",
                ""dataModels"": [
                    { ""name"": ""Ed-Fi"", ""version"": ""3.3.0-a"" },
                    { ""name"": """+dataModel+ @""", ""version"": """+version+@""" }
                ],
                ""urls"": {
                    ""dependencies"": ""https://api.ed-fi.org/v5.2/api/metadata/data/v3/dependencies"",
                    ""openApiMetadata"": ""https://api.ed-fi.org/v5.2/api/metadata/"",
                    ""oauth"": ""https://api.ed-fi.org/v5.2/api/oauth/token"",
                    ""dataManagementApi"": ""https://api.ed-fi.org/v5.2/api/data/v3/"",
                    ""xsdMetadata"": ""https://api.ed-fi.org/v5.2/api/metadata/xsd""
                }
            }";
        }

        private static string ExampleOdsRootDocumentWithInformationalVersion(string dataModel, string version, string infoVersion)
        {
            return @"{
                ""version"": ""5.2"",
                ""informationalVersion"": ""5.2"",
                ""suite"": ""3"",
                ""build"": ""5.2.14406.0"",
                ""apiMode"": ""Sandbox"",
                ""dataModels"": [
                    { ""name"": ""Ed-Fi"", ""version"": ""3.3.0-a"" },
                    { ""name"": """ + dataModel + @""", ""version"": """ + version + @""", ""informationalVersion"": """ + infoVersion + @"""}
                ],
                ""urls"": {
                    ""dependencies"": ""https://api.ed-fi.org/v5.2/api/metadata/data/v3/dependencies"",
                    ""openApiMetadata"": ""https://api.ed-fi.org/v5.2/api/metadata/"",
                    ""oauth"": ""https://api.ed-fi.org/v5.2/api/oauth/token"",
                    ""dataManagementApi"": ""https://api.ed-fi.org/v5.2/api/data/v3/"",
                    ""xsdMetadata"": ""https://api.ed-fi.org/v5.2/api/metadata/xsd""
                }
            }";
        }

        private static async Task<string> TpdmExtensionVersion(string dataModel, string version)
        {
            var rootDocument = ExampleOdsRootDocument(dataModel, version);

            return (await TpdmExtensionVersion(rootDocument)).TpdmVersion;
        }

        private static async Task<TpdmExtensionDetails> TpdmCore(string dataModel, string version)
        {
            var rootDocument = ExampleOdsRootDocumentWithInformationalVersion(dataModel, version, "TPDM-Core");

            return await TpdmExtensionVersion(rootDocument);
        }

        private static async Task<TpdmExtensionDetails> TpdmCommunity(string dataModel, string version)
        {
            var rootDocument = ExampleOdsRootDocumentWithInformationalVersion(dataModel, version, "TPDM-Community");

            return await TpdmExtensionVersion(rootDocument);
        }

        private static async Task<TpdmExtensionDetails> TpdmExtensionVersion(string rootDocument)
        {
            var getRootDocument = new StubGetRequest(OdsRootUri, rootDocument);

            var extensionDetails = new InferExtensionDetails(getRootDocument);

            return  await extensionDetails.TpdmExtensionVersion(OdsRootUri);
        }

        [Test]
        public async Task CanDetectWhenTpdmModuleIsNotPresent()
        {
            (await TpdmExtensionVersion("GrandBend", "1.0.0")).ShouldBeNull();
        }

        [Test]
        public async Task CanDetectWhenTpdmModuleIsPresentAtExactlyMinimumSupportedVersion()
        {
            (await TpdmExtensionVersion("TPDM", "1.0.0")).ShouldBe("1.0.0");
        }

        [Test]
        public async Task CanDetectWhenTpdmModuleIsPresentAndNewerThanMinimumSupportedVersion()
        {
            (await TpdmExtensionVersion("TPDM", "1.0.1")).ShouldBe("1.0.1");
            (await TpdmExtensionVersion("TPDM", "1.1.0")).ShouldBe("1.1.0");
            (await TpdmExtensionVersion("TPDM", "9.8.7")).ShouldBe("9.8.7");
        }

        [Test]
        public async Task CanDetectWhenTpdmModuleIsPresentButInsufficientVersionToSupport()
        {
            (await TpdmExtensionVersion("TPDM", "0.999.999")).ShouldBe("");
            (await TpdmExtensionVersion("TPDM", "0.8.0")).ShouldBe("");
        }

        [Test]
        public async Task CanDetectWhenTpdmModuleIsUsingCaseInsensitiveModuleNameComparisons()
        {
            (await TpdmExtensionVersion("tPdM", "1.0.0")).ShouldBe("1.0.0");
        }

        [Test]
        public async Task DegradesGracefullyForMalformedOdsRootDocument()
        {
            (await TpdmExtensionVersion("{}")).TpdmVersion.ShouldBeNull(); //Missing expected key.
            (await TpdmExtensionVersion(@"{""dataModels"":5}")).TpdmVersion.ShouldBeNull(); //Expected key has unexpected simple value.
            (await TpdmExtensionVersion(@"{""dataModels"":{""A"": 1}}")).TpdmVersion.ShouldBeNull(); //Expected key has unexpeted object value.
            (await TpdmExtensionVersion(@"{""dataModels"":[1]}")).TpdmVersion.ShouldBeNull(); //Expected key has expected array but unexpected item type.
            (await TpdmExtensionVersion(@"{""dataModels"":[{""name"": ""TPDM""}]}")).TpdmVersion.ShouldBeNull(); //Expected array lacks expected name.
            (await TpdmExtensionVersion(@"{""dataModels"":[{""version"": ""1.0.0""}]}")).TpdmVersion.ShouldBeNull(); //Expected array lacks expected version.
        }

        [Test]
        public async Task CanDetectWhenTpdmCoreModuleIsPresentWithSupportedVersion()
        {
            var expectedVersion = "1.1.0";
            var tpdmExtension = await TpdmCore("TPDM", expectedVersion);
            tpdmExtension.TpdmVersion.ShouldBe(expectedVersion);
            tpdmExtension.IsTpdmCommunityVersion.ShouldBeFalse();
        }

        [Test]
        public async Task CanDetectWhenTpdmCommunityModuleIsPresentWithSupportedVersion()
        {
            var expectedVersion = "1.0.0";
            var tpdmExtension = await TpdmCommunity("TPDM", expectedVersion);
            tpdmExtension.TpdmVersion.ShouldBe(expectedVersion);
            tpdmExtension.IsTpdmCommunityVersion.ShouldBeTrue();
        }

        [Test]
        public async Task CanDetectWhenTpdmCommunityModuleIsPresentWithSupportedVersionNoInformationalVersion()
        {
            var expectedVersion = "1.0.0";
            var rootDocument = ExampleOdsRootDocument("TPDM", expectedVersion);
            var tpdmExtension = await TpdmExtensionVersion(rootDocument);
            tpdmExtension.TpdmVersion.ShouldBe(expectedVersion);
            tpdmExtension.IsTpdmCommunityVersion.ShouldBeTrue();
        }
    }
}
