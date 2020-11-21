// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.OdsInstanceSettingsController
{
    [TestFixture]
    public class WhenUpdatingLearningStandards
    {

        [TestFixture]
        public class GivenNotUsingYearSpecificMode : OdsInstanceSettingsControllerFixture
        {
            private const string ProductionUrl = "http://example.com";

            protected override void AdditionalSetup()
            {
                ApiModeProvider
                    .Setup(x => x.GetApiMode())
                    .Returns(ApiMode.Sandbox);

                ApiConnectionInformationProvider
                    .Setup(x => x.GetConnectionInformationForEnvironment(CloudOdsEnvironment.Production))
                    .ReturnsAsync(new OdsApiConnectionInformation("Ods Instance", ApiMode.Sandbox) { ApiServerUrl = ProductionUrl });

                LearningStandardsJob.Setup(x => x.EnqueueJob(It.IsAny<LearningStandardsJobContext>()));

            }

            [Test]
            public async Task ThenItShouldRespondWithStatusCode200()
            {
                // Act
                var result = await SystemUnderTest.UpdateLearningStandards();

                // Assert
                result.ShouldBeOfType<OkResult>();
                ((OkResult)result).StatusCode.ShouldBe(200);
            }


            [Test]
            public async Task ThenItShouldHaveEnqueuedALearningStandardsSyncJobWithoutYear()
            {
                // Act
                var _ = await SystemUnderTest.UpdateLearningStandards();

                // Assert
                Func<LearningStandardsJobContext, bool> learningStandardsJobEnqueueVerifier = actual =>
                {
                    actual.ShouldSatisfyAllConditions(
                        () => actual.Environment.ShouldBe(CloudOdsEnvironment.Production.Value),
                        () => actual.ApiUrl.ShouldBe(ProductionUrl),
                        () => actual.SchoolYear.ShouldBeNull()
                    );
                    return true;
                };
                LearningStandardsJob.Verify(
                    x => x.EnqueueJob(It.Is<LearningStandardsJobContext>(y => learningStandardsJobEnqueueVerifier(y))),
                    Times.Once);
            }
        }

        [TestFixture]
        public class GivenIsUsingYearSpecificMode : OdsInstanceSettingsControllerFixture
        {
            private const string ProductionUrl = "http://example.com";
            private const int Year = 1234;
            private readonly InstanceContext _instanceContext = new InstanceContext
            {
                Id = 1,
                Name = "Ed_Fi_Ods_1234"
            };

            protected override void AdditionalSetup()
            {
                InstanceContext.Id = _instanceContext.Id;
                InstanceContext.Name = _instanceContext.Name;

                ApiModeProvider
                    .Setup(x => x.GetApiMode())
                    .Returns(ApiMode.YearSpecific);

                ApiConnectionInformationProvider
                    .Setup(x => x.GetConnectionInformationForEnvironment(CloudOdsEnvironment.Production))
                    .ReturnsAsync(new OdsApiConnectionInformation (_instanceContext.Name, ApiMode.YearSpecific) { ApiServerUrl = ProductionUrl});

                LearningStandardsJob.Setup(x => x.EnqueueJob(It.IsAny<LearningStandardsJobContext>()));

            }

            [Test]
            public async Task ThenItShouldRespondWithStatusCode200()
            {
                // Act
                var result = await SystemUnderTest.UpdateLearningStandards();

                // Assert
                result.ShouldBeOfType<OkResult>();
                ((OkResult)result).StatusCode.ShouldBe(200);
            }


            [Test]
            public async Task ThenItShouldHaveEnqueuedALearningStandardsSyncJobWithYear()
            {
                // Act
                var _ = await SystemUnderTest.UpdateLearningStandards();

                // Assert
                Func<LearningStandardsJobContext, bool> learningStandardsJobEnqueueVerifier = actual =>
                {
                    actual.ShouldSatisfyAllConditions(
                        () => actual.Environment.ShouldBe(CloudOdsEnvironment.Production.Value),
                        () => actual.ApiUrl.ShouldBe(ProductionUrl),
                        () => actual.SchoolYear.ShouldBe(Year)
                    );
                    return true;
                };
                LearningStandardsJob.Verify(
                    x => x.EnqueueJob(It.Is<LearningStandardsJobContext>(y => learningStandardsJobEnqueueVerifier(y))),
                    Times.Once);
            }
        }

        [TestFixture]
        public class GivenUsingMultiInstanceMode : OdsInstanceSettingsControllerFixture
        {
            private const string ProductionUrl = "http://example.com";
            private readonly InstanceContext _instanceContext = new InstanceContext
            {
                Id = 1234,
                Name = "Ed_Fi_Ods_1234"
            };

            protected override void AdditionalSetup()
            {
                InstanceContext.Id = _instanceContext.Id;
                InstanceContext.Name = _instanceContext.Name;

                ApiModeProvider
                    .Setup(x => x.GetApiMode())
                    .Returns(ApiMode.DistrictSpecific);

                ApiConnectionInformationProvider
                    .Setup(x => x.GetConnectionInformationForEnvironment(CloudOdsEnvironment.Production))
                    .ReturnsAsync(new OdsApiConnectionInformation (_instanceContext.Name, ApiMode.DistrictSpecific) { ApiServerUrl = ProductionUrl });

                LearningStandardsJob.Setup(x => x.EnqueueJob(It.IsAny<LearningStandardsJobContext>()));

            }

            [Test]
            public async Task ThenItShouldRespondWithStatusCode200()
            {
                // Act
                var result = await SystemUnderTest.UpdateLearningStandards();

                // Assert
                result.ShouldBeOfType<OkResult>();
                ((OkResult)result).StatusCode.ShouldBe(200);
            }


            [Test]
            public async Task ThenItShouldHaveEnqueuedALearningStandardsSyncJobWithOdsInstanceId()
            {
                // Act
                var _ = await SystemUnderTest.UpdateLearningStandards();

                // Assert
                Func<LearningStandardsJobContext, bool> learningStandardsJobEnqueueVerifier = actual =>
                {
                    actual.ShouldSatisfyAllConditions(
                        () => actual.Environment.ShouldBe(CloudOdsEnvironment.Production.Value),
                        () => actual.ApiUrl.ShouldBe(ProductionUrl),
                        () => actual.OdsInstanceId.ShouldBe(_instanceContext.Id)
                    );
                    return true;
                };
                LearningStandardsJob.Verify(
                    x => x.EnqueueJob(It.Is<LearningStandardsJobContext>(y => learningStandardsJobEnqueueVerifier(y))),
                    Times.Once);
            }
        }
    }
}
