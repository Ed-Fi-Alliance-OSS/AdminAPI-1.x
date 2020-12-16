// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Application;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.TestingHelper;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class AddApplicationCommandTests : PlatformUsersContextTestBase
    {
        [Test]
        public void ShouldFailForInvalidVendor()
        {
            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> {new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            var user = new Admin.DataAccess.Models.User
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = vendor
            };

            Save(vendor, user);

            Scoped<IUsersContext>(usersContext =>
            {
                var command = new AddApplicationCommand(usersContext, new InstanceContext());
                var newApplication = new TestApplication
                {
                    ApplicationName = "Production-Test Application",
                    ClaimSetName = "FakeClaimSet",
                    ProfileId = 0,
                    VendorId = 0
                };

                Assert.Throws<InvalidOperationException>(() => command.Execute(newApplication));
            });
        }

        [Test]
        public void ShouldNotAddIfNameNotWithinApplicationNameMaxLength()
        {
            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            var user = new Admin.DataAccess.Models.User
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = vendor
            };

            Save(vendor, user);

            var applicationName = Sample("Test Application", 51);

            var newApplication = new AddApplicationModel
            {
                ApplicationName = applicationName,
                ClaimSetName = "FakeClaimSet",
                ProfileId = null,
                VendorId = vendor.VendorId,
                EducationOrganizationIds = new List<int> { 12345, 67890 }
            };

            new AddApplicationModelValidator()
                .ShouldNotValidate(newApplication, $"The Application Name {applicationName} would be too long for Admin App to set up necessary Application records. Consider shortening the name by 1 character(s).");
        }

        [Test]
        public void ProfileShouldBeOptional()
        {
            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            var user = new Admin.DataAccess.Models.User
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = vendor
            };

            Save(vendor, user);

            AddApplicationResult result = null;

            Scoped<IUsersContext>(usersContext =>
            {
                var command = new AddApplicationCommand(usersContext, new InstanceContext());
                var newApplication = new TestApplication
                {
                    ApplicationName = "Test Application",
                    ClaimSetName = "FakeClaimSet",
                    ProfileId = null,
                    VendorId = vendor.VendorId,
                    EducationOrganizationIds = new List<int> { 12345, 67890 }
                };

                result = command.Execute(newApplication);
            });

            Transaction(usersContext =>
            {
                var persistedApplication = usersContext.Applications.Single(a => a.ApplicationId == result.ApplicationId);

                persistedApplication.ClaimSetName.ShouldBe("FakeClaimSet");
                persistedApplication.Profiles.Count.ShouldBe(0);
                persistedApplication.ApplicationEducationOrganizations.Count.ShouldBe(2);
                persistedApplication.ApplicationEducationOrganizations.All(o => o.EducationOrganizationId == 12345 || o.EducationOrganizationId == 67890).ShouldBeTrue();

                persistedApplication.Vendor.VendorId.ShouldBeGreaterThan(0);
                persistedApplication.Vendor.VendorId.ShouldBe(vendor.VendorId);

                persistedApplication.ApiClients.Count.ShouldBe(1);
                var apiClient = persistedApplication.ApiClients.First();
                apiClient.Name.ShouldBe("Test Application");
                apiClient.ApplicationEducationOrganizations.All(o => o.EducationOrganizationId == 12345 || o.EducationOrganizationId == 67890).ShouldBeTrue();
                apiClient.Key.ShouldBe(result.Key);
                apiClient.Secret.ShouldBe(result.Secret);
            });
        }
        
        [Test]
        public void ShouldExecute()
        {
            const string odsInstanceName = "Test Instance";
            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            var user = new Admin.DataAccess.Models.User
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = vendor
            };

            var profile = new Profile
            {
                ProfileName = "Test Profile"
            };

            var odsInstance = new OdsInstance
            {
                Name = odsInstanceName,
                InstanceType = "Ods",
                IsExtended = false,
                Status = "OK",
                Version = "1.0.0"
            };

            Save(vendor, user, profile, odsInstance);

            var instanceContext = new InstanceContext
            {
                Id = 1,
                Name = odsInstanceName
            };

            AddApplicationResult result = null;
            Scoped<IUsersContext>(usersContext =>
            {
                var command = new AddApplicationCommand(usersContext, instanceContext);
                var newApplication = new TestApplication
                {
                    ApplicationName = "Test Application",
                    ClaimSetName = "FakeClaimSet",
                    ProfileId = profile.ProfileId,
                    VendorId = vendor.VendorId,
                    EducationOrganizationIds = new List<int> { 12345, 67890 }
                };

                result = command.Execute(newApplication);
            });

            Transaction(usersContext =>
            {
                var persistedApplication = usersContext.Applications.Single(a => a.ApplicationId == result.ApplicationId);

                persistedApplication.ClaimSetName.ShouldBe("FakeClaimSet");
                persistedApplication.Profiles.Count.ShouldBe(1);
                persistedApplication.Profiles.First().ProfileName.ShouldBe("Test Profile");
                persistedApplication.ApplicationEducationOrganizations.Count.ShouldBe(2);
                persistedApplication.ApplicationEducationOrganizations.All(o => o.EducationOrganizationId == 12345 || o.EducationOrganizationId == 67890).ShouldBeTrue();

                persistedApplication.Vendor.VendorId.ShouldBeGreaterThan(0);
                persistedApplication.Vendor.VendorId.ShouldBe(vendor.VendorId);

                persistedApplication.ApiClients.Count.ShouldBe(1);
                var apiClient = persistedApplication.ApiClients.First();
                apiClient.Name.ShouldBe("Test Application");
                apiClient.ApplicationEducationOrganizations.All(o => o.EducationOrganizationId == 12345 || o.EducationOrganizationId == 67890).ShouldBeTrue();
                apiClient.Key.ShouldBe(result.Key);
                apiClient.Secret.ShouldBe(result.Secret);

                persistedApplication.OdsInstance.ShouldNotBeNull();
                persistedApplication.OdsInstance.Name.ShouldBe(odsInstanceName);
            });
        }

        private class TestApplication : IAddApplicationModel
        {
            public string ApplicationName { get; set; }
            public int VendorId { get; set; }
            public string ClaimSetName { get; set; }
            public int? ProfileId { get; set; }
            public IEnumerable<int> EducationOrganizationIds { get; set; }
        }
    }
}
