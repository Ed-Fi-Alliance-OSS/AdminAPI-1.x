using EdFi.Ods.Admin.Api.Features.Applications;
using EdFi.Ods.Admin.Api.Infrastructure.Commands;
using FakeItEasy;

namespace EdFi.Ods.Admin.Api.UnitTests.Features.Applications
{
    [TestFixture]
    public class AddApplicationModelValidatorTests
    {
        [Test]
        public void ShouldNotAddIfNameNotWithinApplicationNameMaxLength()
        {
            const string applicationName = "Test Application Test Application Test Application T";

            var newApplication = A.Fake<IAddApplicationModel>();

            A.CallTo(() => newApplication.ApplicationName).Returns(applicationName);
            A.CallTo(() => newApplication.ClaimSetName).Returns("FakeClaimSet");
            A.CallTo(() => newApplication.ProfileId).Returns(null);
            A.CallTo(() => newApplication.VendorId).Returns(1);
            A.CallTo(() => newApplication.EducationOrganizationIds).Returns(new List<int> { 12345, 67890 });

            new AddApplicationModelValidator()
                .ShouldNotValidate(newApplication, $"The Application Name {applicationName} would be too long for Admin App to set up necessary Application records. Consider shortening the name by 2 character(s).");
        }
    }
}
