using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Web.Helpers.StringExtensions;

namespace EdFi.Ods.AdminApp.Management.Tests.Helpers
{
    class StringExtensionsTests
    {
        [Test]
        public void ShouldParseUserFacingCategoryNameFromDescriptorPath()
        {
            "/ed-fi/builtInDescriptors".GetDescriptorCategoryName().ShouldBe("BuiltInDescriptor");
            "/tpdm/extensionDescriptors".GetDescriptorCategoryName().ShouldBe("ExtensionDescriptor [Tpdm]");

            //Resilience against inputs that don't conform to the basic expected path naming convention:
            "/ed-fi/builtInDescriptor".GetDescriptorCategoryName().ShouldBe("BuiltInDescriptor");
            "/sample/extension/Unexpected/Long/Path".GetDescriptorCategoryName().ShouldBe("Extension/Unexpected/Long/Path [Sample]");

            //Fail safe, passing through the original path for inputs that cannot parse at all.
            "/unexpectedlyShortPath".GetDescriptorCategoryName().ShouldBe("/unexpectedlyShortPath");
        }
    }
}
