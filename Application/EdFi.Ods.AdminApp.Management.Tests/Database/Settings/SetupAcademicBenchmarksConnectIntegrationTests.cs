// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.Common.Security;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Settings
{
    [TestFixture]
    public class SetupAcademicBenchmarksConnectIntegrationTests : AdminDataTestBase
    {
        public SetupAcademicBenchmarksConnectIntegrationTests()
        {
            CheckpointPolicy = CheckpointPolicyOptions.BeforeAnyTest;
        }

        [Test]
        public async Task ShouldSaveNewApplication()
        {
            var odsInstanceName = $"TestInstance_{Guid.NewGuid().ToString()}";
            var hashConfigProvider = new Mock<IHashConfigurationProvider>();
            var instanceContext = new InstanceContext {Id = 1, Name = odsInstanceName};
            var applicationName = $"{odsInstanceName}_{CloudsOdsAcademicBenchmarksConnectApp.ApplicationName}";

            hashConfigProvider.Setup(x => x.GetHashConfiguration()).Returns(new HashConfiguration
            {
                Algorithm = "Test",
                Iterations = 1,
                SaltSize = 256
            });

            var hashProvider = new Mock<ISecurePackedHashProvider>();
            hashProvider
                .Setup(x => x.ComputePackedHashString(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<int>())).Returns("hashedTestValue");

            var secretConfigProvider = new Mock<IOdsSecretConfigurationProvider>();
            secretConfigProvider.Setup(x => x.GetSecretConfiguration(1)).ReturnsAsync(new OdsSecretConfiguration());

            var service = new SetupAcademicBenchmarksConnectService(TestContext, hashProvider.Object,
                hashConfigProvider.Object, secretConfigProvider.Object, instanceContext);

            await service.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            TestContext.SaveChanges();

            var newApplication = TestContext.Applications.FirstOrDefault(x =>
                x.ApplicationName == applicationName);

            newApplication.ShouldNotBeNull();
        }

        [Test]
        public async Task ShouldSaveNewApplicationWithApplicationEdOrgAssociationOnDistrictSpecificMode()
        {
            var odsInstanceName = $"{Guid.NewGuid().ToString()}_255901";
            var hashConfigProvider = new Mock<IHashConfigurationProvider>();
            var instanceContext = new InstanceContext { Id = 1, Name = odsInstanceName };
            var applicationName = $"{odsInstanceName}_{CloudsOdsAcademicBenchmarksConnectApp.ApplicationName}";

            hashConfigProvider.Setup(x => x.GetHashConfiguration()).Returns(new HashConfiguration
            {
                Algorithm = "Test",
                Iterations = 1,
                SaltSize = 256
            });

            var hashProvider = new Mock<ISecurePackedHashProvider>();
            hashProvider
                .Setup(x => x.ComputePackedHashString(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<int>())).Returns("hashedTestValue");

            var secretConfigProvider = new Mock<IOdsSecretConfigurationProvider>();
            secretConfigProvider.Setup(x => x.GetSecretConfiguration(1)).ReturnsAsync(new OdsSecretConfiguration());

            var service = new SetupAcademicBenchmarksConnectService(TestContext, hashProvider.Object,
                hashConfigProvider.Object, secretConfigProvider.Object, instanceContext);

            await service.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.DistrictSpecific);

            TestContext.SaveChanges();

            var newApplication = TestContext.Applications.FirstOrDefault(x =>
                x.ApplicationName == applicationName);

            newApplication.ShouldNotBeNull();
            newApplication.ApplicationEducationOrganizations.ShouldNotBeEmpty();
            newApplication.ApplicationEducationOrganizations.FirstOrDefault().ShouldNotBeNull();
            newApplication.ApplicationEducationOrganizations.First().EducationOrganizationId.ShouldBe(255901);
        }
    }
}
