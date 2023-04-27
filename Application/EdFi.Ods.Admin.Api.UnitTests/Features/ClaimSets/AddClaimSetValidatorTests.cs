using EdFi.Ods.Admin.Api.Features.ClaimSets;
using EdFi.Ods.Admin.Api.Infrastructure.Queries;
using FakeItEasy;
using Shouldly;

namespace EdFi.Ods.Admin.Api.UnitTests.Features.ClaimSets
{
    [TestFixture]
    internal class AddClaimSetValidatorTests
    {
        [Test]
        public void ShouldNotAddClaimSetIfNameNotUnique()
        {
            // Arrange
            const string claimSetName = "Not Unique";
            var newClaimSet = new AddClaimSet.Request { Name = claimSetName };

            // ... return an existing claimset with the same name.
            var fakeGetAllQuery = A.Fake<IGetAllClaimSetsQuery>();
            A.CallTo(() => fakeGetAllQuery.Execute()).Returns(new[] { new ClaimSet { Name = claimSetName } });

            var fakeGetResourceClaimsQuery = A.Fake<IGetResourceClaimsAsFlatListQuery>();
            A.CallTo(() => fakeGetResourceClaimsQuery.Execute()).Returns(new List<ResourceClaim>());

            var fakeAuthStrategyQuery = A.Fake<IGetAllAuthorizationStrategiesQuery>();
            A.CallTo(() => fakeAuthStrategyQuery.Execute()).Returns(new List<AuthorizationStrategy>());

            // Act
            var result = new AddClaimSet.Validator(fakeGetAllQuery, fakeGetResourceClaimsQuery, fakeAuthStrategyQuery).Validate(newClaimSet);

            // Assert
            result.IsValid.ShouldBe(false);
            result.Errors.Single().ErrorMessage.ShouldBe("A claim set with this name already exists in the database. Please enter a unique name.");
        }

        [Test]
        public void ShouldNotAddClaimSetIfNameEmpty()
        {
            // Arrange
            var newClaimSet = new AddClaimSet.Request { Name = string.Empty };

            // ... return an existing claimset with the same name.
            var fakeGetAllQuery = A.Fake<IGetAllClaimSetsQuery>();
            A.CallTo(() => fakeGetAllQuery.Execute()).Returns(new List<ClaimSet>());

            var fakeGetResourceClaimsQuery = A.Fake<IGetResourceClaimsAsFlatListQuery>();
            A.CallTo(() => fakeGetResourceClaimsQuery.Execute()).Returns(new List<ResourceClaim>());

            var fakeAuthStrategyQuery = A.Fake<IGetAllAuthorizationStrategiesQuery>();
            A.CallTo(() => fakeAuthStrategyQuery.Execute()).Returns(new List<AuthorizationStrategy>());

            // Act
            var result = new AddClaimSet.Validator(fakeGetAllQuery, fakeGetResourceClaimsQuery, fakeAuthStrategyQuery).Validate(newClaimSet);

            // Assert
            result.IsValid.ShouldBe(false);
            result.Errors.Single().ErrorMessage.ShouldBe("'Name' must not be empty.");

        }

        [Test]
        public void ShouldNotAddClaimSetIfNameLengthGreaterThan255Characters()
        {
            // Arrange
            var newClaimSet = new AddClaimSet.Request { Name = "T".PadLeft(256, '\0') };

            // ... return an existing claimset with the same name.
            var fakeGetAllQuery = A.Fake<IGetAllClaimSetsQuery>();
            A.CallTo(() => fakeGetAllQuery.Execute()).Returns(new List<ClaimSet>());

            var fakeGetResourceClaimsQuery = A.Fake<IGetResourceClaimsAsFlatListQuery>();
            A.CallTo(() => fakeGetResourceClaimsQuery.Execute()).Returns(new List<ResourceClaim>());

            var fakeAuthStrategyQuery = A.Fake<IGetAllAuthorizationStrategiesQuery>();
            A.CallTo(() => fakeAuthStrategyQuery.Execute()).Returns(new List<AuthorizationStrategy>());

            // Act
            var result = new AddClaimSet.Validator(fakeGetAllQuery, fakeGetResourceClaimsQuery, fakeAuthStrategyQuery).Validate(newClaimSet);

            // Assert
            result.IsValid.ShouldBe(false);
            result.Errors.Single().ErrorMessage.ShouldBe("The claim set name must be less than 255 characters.");
        }
    }
}
