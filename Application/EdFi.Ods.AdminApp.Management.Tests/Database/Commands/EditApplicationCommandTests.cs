// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using NUnit.Framework;
using Shouldly;
using VendorUser = EdFi.Admin.DataAccess.Models.User;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class EditApplicationCommandTests : AdminDataTestBase
    {
        private Vendor _vendor;
        private Vendor _otherVendor;
        private VendorUser _user;
        private VendorUser _otherUser;
        private Profile _profile;
        private Profile _otherProfile;
        private ApiClient _apiClient;
        private Application _application;


        private void SetupTestEntities()
        {
            _vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            _otherVendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Different Integration Tests"
            };

            _user = new VendorUser
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = _vendor
            };

            _otherUser = new VendorUser
            {
                Email = "nowhere@nobody.com",
                FullName = "Different Integration Tests",
                Vendor = _otherVendor
            };

            _profile = new Profile
            {
                ProfileName = "Test Profile"
            };

            _otherProfile = new Profile
            {
                ProfileName = "Other Test Profile"
            };

            _apiClient = new ApiClient(true)
            {
                Name = "Integration Test",
                UseSandbox = false,
            };

            _application = new Application
            {
                ApplicationName = "Test Application",
                ClaimSetName = "FakeClaimSet",
                Vendor = _vendor,
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri
            };

            _application.ApiClients.Add(_apiClient);
            _application.Profiles.Add(_profile);
            _application.ApplicationEducationOrganizations.Add(_application.CreateApplicationEducationOrganization(12345));
            _application.ApplicationEducationOrganizations.Add(_application.CreateApplicationEducationOrganization(67890));

            Save(_vendor, _otherVendor, _user, _otherUser, _profile, _otherProfile, _application);
        }

        [Test]
        public void ShouldRemoveProfileIfNull()
        {
            SetupTestEntities();

            var editModel = new TestEditApplicationModel
            {
                ApplicationId = _application.ApplicationId,
                ApplicationName = _application.ApplicationName,
                ClaimSetName = _application.ClaimSetName,
                EducationOrganizationIds = new List<int> { 12345, 67890 },
                Environment = CloudOdsEnvironment.Production,
                ProfileId = null,
                VendorId = _vendor.VendorId
            };


            var command = new EditApplicationCommand(SetupContext);
            command.Execute(editModel);

            var persistedApplication = TestContext.Applications.Single(a => a.ApplicationId == _application.ApplicationId);

            persistedApplication.ApplicationName.ShouldBe(CloudOdsApplicationName.GetPersistedName("Test Application", CloudOdsEnvironment.Production));
            persistedApplication.ClaimSetName.ShouldBe("FakeClaimSet");

            persistedApplication.ApiClients.Count.ShouldBe(1);
            persistedApplication.ApiClients.First().Name.ShouldBe(CloudOdsApplicationName.GetPersistedName("Test Application", CloudOdsEnvironment.Production));
            persistedApplication.ApiClients.First().ApplicationEducationOrganizations.ShouldAllBe(aeo => persistedApplication.ApplicationEducationOrganizations.Contains(aeo));

            persistedApplication.ApplicationEducationOrganizations.Count.ShouldBe(2);
            persistedApplication.ApplicationEducationOrganizations.ShouldAllBe(aeo => aeo.EducationOrganizationId == 12345 || aeo.EducationOrganizationId == 67890);

            persistedApplication.Profiles.Count.ShouldBe(0);
        }

        [Test]
        public void ShouldUpdateAllEntitiesProperly()
        {
            SetupTestEntities();

            var editModel = new TestEditApplicationModel
            {
                ApplicationId = _application.ApplicationId,
                ApplicationName = "New Application Name",
                ClaimSetName = "DifferentFakeClaimSet",
                EducationOrganizationIds = new List<int> { 23456, 78901 },
                Environment = CloudOdsEnvironment.Production,
                ProfileId = _otherProfile.ProfileId,
                VendorId = _otherVendor.VendorId
            };


            var command = new EditApplicationCommand(SetupContext);
            command.Execute(editModel);

            var persistedApplication = TestContext.Applications.Single(a => a.ApplicationId == _application.ApplicationId);

            persistedApplication.ApplicationName.ShouldBe(CloudOdsApplicationName.GetPersistedName("New Application Name", CloudOdsEnvironment.Production));
            persistedApplication.ClaimSetName.ShouldBe("DifferentFakeClaimSet");

            persistedApplication.ApiClients.Count.ShouldBe(1);
            persistedApplication.ApiClients.First().Name.ShouldBe(CloudOdsApplicationName.GetPersistedName("New Application Name", CloudOdsEnvironment.Production));
            persistedApplication.ApiClients.First().ApplicationEducationOrganizations.ShouldAllBe(aeo => persistedApplication.ApplicationEducationOrganizations.Contains(aeo));

            persistedApplication.Profiles.Count.ShouldBe(1);
            persistedApplication.Profiles.First().ProfileName.ShouldBe("Other Test Profile");

            persistedApplication.ApplicationEducationOrganizations.Count.ShouldBe(2);
            persistedApplication.ApplicationEducationOrganizations.ShouldAllBe(aeo => aeo.EducationOrganizationId == 23456 || aeo.EducationOrganizationId == 78901);
        }

        private class TestEditApplicationModel : IEditApplicationModel
        {
            public int ApplicationId { get; set; }
            public CloudOdsEnvironment Environment { get; set; }
            public string ApplicationName { get; set; }
            public int VendorId { get; set; }
            public string ClaimSetName { get; set; }
            public int? ProfileId { get; set; }
            public IEnumerable<int> EducationOrganizationIds { get; set; }
        }
    }
}
