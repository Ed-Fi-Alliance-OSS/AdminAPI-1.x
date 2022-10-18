// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetApplicationsByVendorIdQueryTests : PlatformUsersContextTestBase
    {
        [Test]
        public void ShouldGetBasicApplicationData()
        {
            var vendor = new Vendor { VendorName = "test vendor" };
            var application = new Application
            {
                ApplicationName = "test application",
                ClaimSetName = "test claim set",
                Vendor = vendor,
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri
            };
            vendor.Applications.Add(application);
            Save(vendor);

            Scoped<IUsersContext>(usersContext =>
            {
                var getApplicationsByVendorIdQuery = new GetApplicationsByVendorIdQuery(usersContext);
                var results = getApplicationsByVendorIdQuery.Execute(vendor.VendorId);
                results.Single().ApplicationName.ShouldBe("test application");
                results.Single().ClaimSetName.ShouldBe("test claim set");
            });
        }

        [Test]
        public void ShouldGetApplicationEducationOrganization()
        {
            var vendor = new Vendor { VendorName = "test vendor" };
            var application = new Application
            {
                ApplicationName = "test application",
                Vendor = vendor,
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri
            };

            var applicationOrganization = new ApplicationEducationOrganization { Application = application };

            application.ApplicationEducationOrganizations.Add(applicationOrganization);
            vendor.Applications.Add(application);
            Save(vendor);
            var organizationId = applicationOrganization.ApplicationEducationOrganizationId;
            organizationId.ShouldBeGreaterThan(0);

            Scoped<IUsersContext>(usersContext =>
            {
                var getApplicationsByVendorIdQuery = new GetApplicationsByVendorIdQuery(usersContext);
                var results = getApplicationsByVendorIdQuery.Execute(vendor.VendorId);
                results.Single().ApplicationEducationOrganizations.Single().ApplicationEducationOrganizationId.ShouldBe(organizationId);
            });
        }

        [Test]
        public void ShouldGetApplicationProfile()
        {
            var vendor = new Vendor { VendorName = "test vendor" };
            var application = new Application {ApplicationName = "test application", OperationalContextUri = OperationalContext.DefaultOperationalContextUri };
            var profile = new Profile
            {
                Applications = new List<Application> { application },
                ProfileName = "test profile"
            };
            application.Profiles.Add(profile);
            vendor.Applications.Add(application);
            Save(vendor);
            var profileId = profile.ProfileId;
            profileId.ShouldBeGreaterThan(0);

            Scoped<IUsersContext>(usersContext =>
            {
                var getApplicationsByVendorIdQuery = new GetApplicationsByVendorIdQuery(usersContext);
                var results = getApplicationsByVendorIdQuery.Execute(vendor.VendorId);
                results.Single().Profiles.Single().ProfileId.ShouldBe(profileId);
            });
        }

        [Test]
        public void ShouldThrowWhenVendorIdIsInvalid()
        {
            Scoped<IUsersContext>(usersContext =>
            {
                var getApplicationsByVendorIdQuery = new GetApplicationsByVendorIdQuery(usersContext);
                Should.Throw<NotFoundException<int>>(() =>
                {
                    getApplicationsByVendorIdQuery.Execute(int.MaxValue);
                });
            });
        }
    }
}
