// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Security.DataAccess.Contexts;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    [TestFixture]
    public class AzureFirstTimeSetupServiceTester
    {
        [Test]
        public void ShouldRetryDatabaseMigrations()
        {
            var mockOdsInstances = MockExtensions.EmptyMockDbSet<OdsInstance>();
            var mockVendors = MockExtensions.EmptyMockDbSet<Vendor>();
            var mockApps = MockExtensions.EmptyMockDbSet<Application>();
            var mockUsersContext = new Mock<IUsersContext>();
            mockUsersContext.Setup(c => c.Vendors).Returns(mockVendors.Object);
            mockUsersContext.Setup(c => c.Applications).Returns(mockApps.Object);

            int numCalls = 0;
            mockUsersContext.Setup(c => c.OdsInstances).Callback(() =>
            {
                ++numCalls;
                if (numCalls < 5)
                    throw new Exception("Simulated failure");
            }).Returns(mockOdsInstances.Object);

            var mockApplications = MockExtensions.EmptyMockDbSet<Security.DataAccess.Models.Application>();

            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(c => c.Applications).Returns(mockApplications.Object);

            var command = new AzureFirstTimeSetupService(mockUsersContext.Object, mockSecurityContext.Object, new TestHashProvider(), new TestHashConfigurationProvider());

            command.EnsureAdminDatabaseInitialized();

            command.RunsAttempted.ShouldBe(5);
        }

        [Test]
        public async Task ShouldSetupApplicationAndVendor()
        {
            var mockOdsInstances = MockExtensions.EmptyMockDbSet<OdsInstance>();
            var mockVendors = MockExtensions.EmptyMockDbSet<Vendor>();
            var mockApps = MockExtensions.EmptyMockDbSet<Application>();
            var mockUsersContext = new Mock<IUsersContext>();
            mockUsersContext.Setup(c => c.Vendors).Returns(mockVendors.Object);
            mockUsersContext.Setup(c => c.Applications).Returns(mockApps.Object);

            mockUsersContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);

            var mockApplications = MockExtensions.EmptyMockDbSet<Security.DataAccess.Models.Application>();

            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Object.Applications = mockApplications.Object;

            var command = new AzureFirstTimeSetupService(mockUsersContext.Object, mockSecurityContext.Object, new TestHashProvider(), new TestHashConfigurationProvider());

            var odsFirstTimeSetupConfiguration = GetFirstTimeSetupConfiguration();

            var result =
                (await command.CreateAdminAppInAdminDatabase(odsFirstTimeSetupConfiguration.ClaimSet.ClaimSetName,
                    odsFirstTimeSetupConfiguration.Name, odsFirstTimeSetupConfiguration.Version,
                    ApiMode.SharedInstance)).Application;

            var expectedApplicationName = odsFirstTimeSetupConfiguration.Name.GetAdminApplicationName();

            Assert.AreEqual(result.ApplicationName, expectedApplicationName);
            Assert.AreEqual(result.ClaimSetName, CloudOdsAdminApp.InternalAdminAppClaimSet);
            Assert.IsTrue(result.ApiClients.Count == 1);
            Assert.AreEqual(result.ApiClients.First(a => !a.UseSandbox).Name, expectedApplicationName);

            Assert.AreEqual(result.Vendor.VendorName, CloudOdsAdminApp.VendorName);
            Assert.AreEqual(result.Vendor.VendorNamespacePrefixes.First().NamespacePrefix, CloudOdsAdminApp.VendorNamespacePrefix);
        }

        [Test]
        public async Task ShouldRegisterInstance()
        {
            var mockOdsInstances = MockExtensions.EmptyMockDbSet<OdsInstance>();
            var mockVendors = MockExtensions.EmptyMockDbSet<Vendor>();
            var mockApps = MockExtensions.EmptyMockDbSet<Application>();
            var mockUsersContext = new Mock<IUsersContext>();
            mockUsersContext.Setup(c => c.Vendors).Returns(mockVendors.Object);
            mockUsersContext.Setup(c => c.Applications).Returns(mockApps.Object);

            mockUsersContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);

            var mockApplications = MockExtensions.EmptyMockDbSet<Security.DataAccess.Models.Application>();

            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Object.Applications = mockApplications.Object;

            var command = new AzureFirstTimeSetupService(mockUsersContext.Object, mockSecurityContext.Object,new TestHashProvider(),
                new TestHashConfigurationProvider());

            var odsFirstTimeSetupConfiguration = GetFirstTimeSetupConfiguration();

            var result =
                (await command.CreateAdminAppInAdminDatabase(odsFirstTimeSetupConfiguration.ClaimSet.ClaimSetName,
                    odsFirstTimeSetupConfiguration.Name, odsFirstTimeSetupConfiguration.Version,
                    ApiMode.SharedInstance)).Application;

            Assert.AreEqual(result.OdsInstance.InstanceType, "CloudOds");
            Assert.AreEqual(result.OdsInstance.IsExtended, false);
            Assert.AreEqual(result.OdsInstance.Name, odsFirstTimeSetupConfiguration.Name);
            Assert.AreEqual(result.OdsInstance.Status, CloudOdsStatus.Ok.DisplayName);
            Assert.AreEqual(result.OdsInstance.Version, odsFirstTimeSetupConfiguration.Version);
        }

        #region MockGetters

        private OdsFirstTimeSetupConfiguration GetFirstTimeSetupConfiguration()
        {
            return new OdsFirstTimeSetupConfiguration
            {
                ClaimSet = GetClaimSet()
            };
        }

        private CloudOdsClaimSet GetClaimSet()
        {
            return new CloudOdsClaimSet
            {
                ApplicationName = CloudOdsAdminApp.ApplicationName,
                ClaimSetName = CloudOdsAdminApp.InternalAdminAppClaimSet
            };
        }

        #endregion
    }
}