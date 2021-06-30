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

        private static bool TpdmExtensionEnabled(string dataModel, string version)
        {
            var rootDocument = ExampleOdsRootDocument(dataModel, version);

            return TpdmExtensionEnabled(rootDocument);
        }

        private static bool TpdmExtensionEnabled(string rootDocument)
        {
            var getRootDocument = new StubGetRequest(OdsRootUri, rootDocument);

            var extensionDetails = new InferExtensionDetails(getRootDocument);

            return extensionDetails.TpdmExtensionEnabled(OdsRootUri);
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsNotPresent()
        {
            TpdmExtensionEnabled("GrandBend", "1.0.0").ShouldBeFalse();
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsPresentAtExactlyMinimumSupportedVersion()
        {
            TpdmExtensionEnabled("TPDM", "1.0.0").ShouldBeTrue();
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsPresentAndNewerThanMinimumSupportedVersion()
        {
            TpdmExtensionEnabled("TPDM", "1.0.1").ShouldBeTrue();
            TpdmExtensionEnabled("TPDM", "1.1.0").ShouldBeTrue();
            TpdmExtensionEnabled("TPDM", "9.8.7").ShouldBeTrue();
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsPresentButInsufficientVersionToSupport()
        {
            TpdmExtensionEnabled("TPDM", "0.999.999").ShouldBeFalse();
            TpdmExtensionEnabled("TPDM", "0.8.0").ShouldBeFalse();
        }

        [Test]
        public void CanDetectWhenTpdmModuleIsUsingCaseInsensitiveModuleNameComparisons()
        {
            TpdmExtensionEnabled("tPdM", "1.0.0").ShouldBeTrue();
        }

        [Test]
        public void DegradesGracefullyForMalformedOdsRootDocument()
        {
            TpdmExtensionEnabled("{}").ShouldBeFalse(); //Missing expected key.
            TpdmExtensionEnabled(@"{""dataModels"":5}").ShouldBeFalse(); //Expected key has unexpected simple value.
            TpdmExtensionEnabled(@"{""dataModels"":{""A"": 1}}").ShouldBeFalse(); //Expected key has unexpeted object value.
            TpdmExtensionEnabled(@"{""dataModels"":[1]}").ShouldBeFalse(); //Expected key has expected array but unexpected item type.
            TpdmExtensionEnabled(@"{""dataModels"":[{""name"": ""TPDM""}]}").ShouldBeFalse(); //Expected array lacks expected name.
            TpdmExtensionEnabled(@"{""dataModels"":[{""version"": ""1.0.0""}]}").ShouldBeFalse(); //Expected array lacks expected version.
        }
    }
}
