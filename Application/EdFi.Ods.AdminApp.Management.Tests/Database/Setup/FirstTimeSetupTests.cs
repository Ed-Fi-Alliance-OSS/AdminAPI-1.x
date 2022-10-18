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
using EdFi.Common.Security;
using EdFi.Ods.AdminApp.Management.OnPrem;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Setup
{
    [TestFixture]
    public class FirstTimeSetupTests
    {
        [Test]
        public async Task ShouldRegisterInstance()
        {
            var odsInstances = new List<OdsInstance>();
            var service = GetFirstTimeSetupService(odsInstances);

            await service.CreateAdminAppInAdminDatabase("Test Claim Set", "Test Instance", "0.0", ApiMode.SharedInstance);

            odsInstances.Count.ShouldBe(1);
        }

        [Test]
        public async Task ShouldNotRegisterInstanceMoreThanOnce()
        {
            var odsInstances = new List<OdsInstance>();

            var service = GetFirstTimeSetupService(odsInstances);

            await service.CreateAdminAppInAdminDatabase("Test Claim Set", "Test Instance", "0.0", ApiMode.SharedInstance);
            odsInstances.Count.ShouldBe(1);

            await service.CreateAdminAppInAdminDatabase("Test Claim Set", "Test Instance", "0.0", ApiMode.SharedInstance);
            odsInstances.Count.ShouldBe(1);
        }

        [Test]
        public async Task ShouldRegisterInstanceAndApplicationWithoutEdOrgAssociationNonDistrictSpecificMode()
        {
            var odsInstances = new List<OdsInstance>();
            var applications = new List<Application>();

            var service = GetFirstTimeSetupService(odsInstances, applications);

            await service.CreateAdminAppInAdminDatabase("Test Claim Set", "Test Instance_2019", "0.0",
                ApiMode.YearSpecific);
            odsInstances.Count.ShouldBe(1);
            applications.Count.ShouldBe(1);
            applications.FirstOrDefault()?.ApplicationEducationOrganizations.Count.ShouldBe(0);
        }

        [Test]
        public async Task ShouldRegisterInstanceWithApplicationEdOrgAssociationOnDistrictSpecificMode()
        {
            var odsInstances = new List<OdsInstance>();
            var applications = new List<Application>();

            var service = GetFirstTimeSetupService(odsInstances, applications);

            await service.CreateAdminAppInAdminDatabase("Test Claim Set", "Test Instance_255901", "0.0",
                ApiMode.DistrictSpecific);
            odsInstances.Count.ShouldBe(1);
            applications.Count.ShouldBe(1);
            applications.FirstOrDefault()?.ApplicationEducationOrganizations.Count.ShouldBe(1);
        }

        private static OnPremFirstTimeSetupService GetFirstTimeSetupService(List<OdsInstance> instances, List<Application> applications = null)
        {
            var mockApplications = applications != null ? MockExtensions.MockDbSet(applications) : MockExtensions.EmptyMockDbSet<Application>();
           
            var mockOdsInstance = MockExtensions.MockDbSet(instances);
            var mockVendors = MockExtensions.EmptyMockDbSet<Vendor>();

            var usersContext = new Mock<IUsersContext>();
            usersContext.Setup(x => x.Applications).Returns(mockApplications.Object);
            usersContext.Setup(x => x.Vendors).Returns(mockVendors.Object);
            usersContext.Setup(x => x.OdsInstances).Returns(mockOdsInstance.Object);

            var securePackedHashProvider = new Mock<ISecurePackedHashProvider>();
            var hashConfigProvider = new Mock<IHashConfigurationProvider>();

            return new OnPremFirstTimeSetupService(usersContext.Object, securePackedHashProvider.Object, hashConfigProvider.Object);
        }
    }
}
