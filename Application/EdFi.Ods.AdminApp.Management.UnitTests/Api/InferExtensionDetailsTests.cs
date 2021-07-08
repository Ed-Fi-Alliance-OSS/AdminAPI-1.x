// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

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

            public string DownloadString(string address)
            {
                address.ShouldBe(_expectedAddress);
                return _jsonResponse;
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

        private static string TpdmExtensionVersion(string dataModel, string version)
        {
            var rootDocument = ExampleOdsRootDocument(dataModel, version);

            return TpdmExtensionVersion(rootDocument);
        }

        private static string TpdmExtensionVersion(string rootDocument)
        {
            var getRootDocument = new StubGetRequest(OdsRootUri, rootDocument);

            var extensionDetails = new InferExtensionDetails(getRootDocument);

            return extensionDetails.TpdmExtensionVersion(OdsRootUri);
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsNotPresent()
        {
            TpdmExtensionVersion("GrandBend", "1.0.0").ShouldBe("");
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsPresentAtExactlyMinimumSupportedVersion()
        {
            TpdmExtensionVersion("TPDM", "1.0.0").ShouldBe("1.0.0");
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsPresentAndNewerThanMinimumSupportedVersion()
        {
            TpdmExtensionVersion("TPDM", "1.0.1").ShouldBe("1.0.1");
            TpdmExtensionVersion("TPDM", "1.1.0").ShouldBe("1.1.0");
            TpdmExtensionVersion("TPDM", "9.8.7").ShouldBe("9.8.7");
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsPresentButInsufficientVersionToSupport()
        {
            TpdmExtensionVersion("TPDM", "0.999.999").ShouldBe("");
            TpdmExtensionVersion("TPDM", "0.8.0").ShouldBe("");
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsUsingCaseInsensitiveModuleNameComparisons()
        {
            TpdmExtensionVersion("tPdM", "1.0.0").ShouldBe("1.0.0");
        }

        [Test]
        public void DegradesGracefullyForMalformedOdsRootDocument()
        {
            TpdmExtensionVersion("{}").ShouldBe(""); //Missing expected key.
            TpdmExtensionVersion(@"{""dataModels"":5}").ShouldBe(""); //Expected key has unexpected simple value.
            TpdmExtensionVersion(@"{""dataModels"":{""A"": 1}}").ShouldBe(""); //Expected key has unexpeted object value.
            TpdmExtensionVersion(@"{""dataModels"":[1]}").ShouldBe(""); //Expected key has expected array but unexpected item type.
            TpdmExtensionVersion(@"{""dataModels"":[{""name"": ""TPDM""}]}").ShouldBe(""); //Expected array lacks expected name.
            TpdmExtensionVersion(@"{""dataModels"":[{""version"": ""1.0.0""}]}").ShouldBe(""); //Expected array lacks expected version.
        }
    }
}
