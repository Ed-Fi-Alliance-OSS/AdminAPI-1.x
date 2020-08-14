// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.Common.Security;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Settings
{
    [TestFixture]
    public class SetupAcademicBenchmarksConnectServiceTests
    {
        private const string OdsInstanceName = "TestInstance";
        private readonly string _applicationName = $"{OdsInstanceName}_{CloudsOdsAcademicBenchmarksConnectApp.ApplicationName}";

        private readonly Mock<IUsersContext> _usersContext = new Mock<IUsersContext>();

        [Test]
        public async Task ShouldCreateAcademicBenchmarksConnectVendorAndApplication()
        {
            var setupService = GetSetupServiceWithOdsInstance();

            _usersContext.Object.Applications.ShouldBeEmpty();

            await setupService.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            var application = _usersContext.Object.Applications.First(app =>
                app.ApplicationName == _applicationName);

            application.ApplicationName.ShouldBe(_applicationName);
            application.OdsInstance.Name.ShouldBe(OdsInstanceName);
        }

        [Test]
        public async Task ShouldNotCreateAcademicBenchmarksConnectVendorTwice()
        {
            var setupService = GetSetupServiceWithOdsInstance();

            await setupService.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            _usersContext.Object.Applications.Count(app => app.ApplicationName == _applicationName)
                .ShouldBe(1);

            await setupService.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            _usersContext.Object.Applications.Count(app => app.ApplicationName == _applicationName)
                .ShouldBe(1);
        }

        [Test]
        public async Task ShouldCreateVendorConfiguredForCertica()
        {
            var setupService = GetSetupServiceWithOdsInstance();

            _usersContext.Object.Vendors.ShouldBeEmpty();

            await setupService.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            var certicaVendor = _usersContext.Object.Applications.Select(x => x.Vendor).FirstOrDefault(x =>
                x.VendorName == CloudsOdsAcademicBenchmarksConnectApp.VendorName);

            certicaVendor.ShouldNotBeNull();
            certicaVendor.VendorNamespacePrefixes.Count.ShouldBe(1);
            certicaVendor.VendorNamespacePrefixes.First().NamespacePrefix
                .ShouldBe(CloudsOdsAcademicBenchmarksConnectApp.VendorNamespacePrefix);
        }

        [Test]
        public async Task ShouldNotCreateCerticaVendorTwice()
        {
            var setupService = GetSetupServiceWithOdsInstance();

            await setupService.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            _usersContext.Object.Applications.Select(x => x.Vendor).Count(x => x.VendorName == CloudsOdsAcademicBenchmarksConnectApp.VendorName).ShouldBe(1);

            await setupService.CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode.SharedInstance);

            _usersContext.Object.Applications.Select(x => x.Vendor).Count(x => x.VendorName == CloudsOdsAcademicBenchmarksConnectApp.VendorName).ShouldBe(1);
        }

        private SetupAcademicBenchmarksConnectService GetSetupServiceWithOdsInstance()
        {
            var odsInstanceList = new List<OdsInstance>
            {
                new OdsInstance {Name = OdsInstanceName}
            };
            var instanceContext = new InstanceContext { Id = 1, Name = OdsInstanceName };
            var mockOdsInstances = MockExtensions.MockDbSet(odsInstanceList);
            var mockVendors = MockExtensions.EmptyMockDbSet<Vendor>();
            var mockApps = MockExtensions.EmptyMockDbSet<Application>();

            _usersContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);
            _usersContext.Setup(c => c.Vendors).Returns(mockVendors.Object);
            _usersContext.Setup(c => c.Applications).Returns(mockApps.Object);

            var hashConfigProvider = new Mock<IHashConfigurationProvider>();
            hashConfigProvider.Setup(x => x.GetHashConfiguration()).Returns(new HashConfiguration
            {
                Algorithm = "Test",
                Iterations = 1,
                SaltSize = 256
            });

            var hashProvider = new Mock<ISecurePackedHashProvider>();
            hashProvider
                .Setup(x => x.ComputePackedHashString(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<int>())).Returns("hashedTestValuex");

            var odsSecretConfig = new Mock<IOdsSecretConfigurationProvider>();
            odsSecretConfig.Setup(x => x.GetSecretConfiguration(1)).ReturnsAsync(new OdsSecretConfiguration());

            return new SetupAcademicBenchmarksConnectService(_usersContext.Object, hashProvider.Object, hashConfigProvider.Object, odsSecretConfig.Object, instanceContext);
        }
    }
}
