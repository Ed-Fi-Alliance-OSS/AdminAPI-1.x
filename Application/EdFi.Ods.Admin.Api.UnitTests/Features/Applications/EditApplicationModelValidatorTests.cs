using EdFi.Ods.Admin.Api.Features.Applications;
using EdFi.Ods.Admin.Api.Infrastructure.Commands;
using FakeItEasy;

namespace EdFi.Ods.Admin.Api.UnitTests.Features.Applications
{
    [TestFixture]
    public class EditApplicationModelValidatorTests
    {

        [Test]
        public void ShouldNotEditIfNameNotWithinApplicationNameMaxLength()
        {
            const string applicationName = "Test Application Test Application Test ApplicationT";

            var editApplication = A.Fake<IEditApplicationModel>();

            A.CallTo(() => editApplication.ApplicationId).Returns(2233);
            A.CallTo(() => editApplication.ApplicationName).Returns(applicationName);
            A.CallTo(() => editApplication.ClaimSetName).Returns("FakeClaimSet");
            A.CallTo(() => editApplication.ProfileId).Returns(null);
            A.CallTo(() => editApplication.VendorId).Returns(1);
            A.CallTo(() => editApplication.EducationOrganizationIds).Returns(new List<int> { 12345 });

            new EditApplicationModelValidator()
                .ShouldNotValidate<IEditApplicationModel>(editApplication, $"The Application Name {applicationName} would be too long for Admin App to set up necessary Application records. Consider shortening the name by 1 character(s).");
        }
    }
}
