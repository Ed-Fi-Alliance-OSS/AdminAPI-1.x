// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using NUnit.Framework;
using Shouldly;
using VendorUser = EdFi.Admin.DataAccess.Models.User;
using EdFi.Ods.AdminApi.Infrastructure;
using Profile = EdFi.Admin.DataAccess.Models.Profile;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.DBTests.Database.CommandTests;

[TestFixture]
public class EditApplicationCommandTests : PlatformUsersContextTestBase
{
    private const int EdOrgId1 = 1234;
    private const int EdOrgId2 = 2345;
    private const int EdOrgId3 = 56666;

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
        _application.ApplicationEducationOrganizations.Add(_application.CreateApplicationEducationOrganization(EdOrgId1));
        _application.ApplicationEducationOrganizations.Add(_application.CreateApplicationEducationOrganization(EdOrgId2));

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
            EducationOrganizationIds = new List<int> { EdOrgId1, EdOrgId2 },
            ProfileId = null,
            VendorId = _vendor.VendorId
        };

        Transaction(usersContext =>
        {
            var command = new EditApplicationCommand(usersContext);
            command.Execute(editModel);
        });

        Transaction(usersContext =>
        {
            var persistedApplication = usersContext.Applications
            .Include(x => x.ApiClients)
            .Include(x => x.ApplicationEducationOrganizations)
            .Include(x => x.Profiles)
            .Single(a => a.ApplicationId == _application.ApplicationId);
            persistedApplication.ApplicationName.ShouldBe("Test Application");
            persistedApplication.ClaimSetName.ShouldBe("FakeClaimSet");
            persistedApplication.ApiClients.Count.ShouldBe(1);
            persistedApplication.ApiClients.First().Name.ShouldBe("Test Application");
            persistedApplication.ApiClients.First().ApplicationEducationOrganizations.ShouldAllBe(aeo => persistedApplication.ApplicationEducationOrganizations.Contains(aeo));
            persistedApplication.ApplicationEducationOrganizations.Count.ShouldBe(2);
            persistedApplication.ApplicationEducationOrganizations.ShouldAllBe(aeo => aeo.EducationOrganizationId == EdOrgId1 || aeo.EducationOrganizationId == EdOrgId2);
            persistedApplication.Profiles.Count.ShouldBe(0);
        });
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
            EducationOrganizationIds = new List<int> { EdOrgId2, EdOrgId3 },
            ProfileId = _otherProfile.ProfileId,
            VendorId = _otherVendor.VendorId
        };

        Transaction(usersContext =>
        {
            var command = new EditApplicationCommand(usersContext);
            command.Execute(editModel);
        });

        Transaction(usersContext =>
        {
            var persistedApplication = usersContext.Applications
            .Include(x => x.ApiClients)
            .Include(x => x.ApplicationEducationOrganizations)
            .Include(x => x.Profiles)
            .Single(a => a.ApplicationId == _application.ApplicationId);
            persistedApplication.ApplicationName.ShouldBe("New Application Name");
            persistedApplication.ClaimSetName.ShouldBe("DifferentFakeClaimSet");
            persistedApplication.ApiClients.Count.ShouldBe(1);
            persistedApplication.ApiClients.First().Name.ShouldBe("New Application Name");
            persistedApplication.ApiClients.First().ApplicationEducationOrganizations.ShouldAllBe(aeo => persistedApplication.ApplicationEducationOrganizations.Contains(aeo));
            persistedApplication.Profiles.Count.ShouldBe(1);
            persistedApplication.Profiles.First().ProfileName.ShouldBe("Other Test Profile");
            persistedApplication.ApplicationEducationOrganizations.Count.ShouldBe(2);
            persistedApplication.ApplicationEducationOrganizations.ShouldAllBe(aeo => aeo.EducationOrganizationId == EdOrgId2 || aeo.EducationOrganizationId == EdOrgId3);
        });
    }

        [Test]
        public void GivenAdditionalEdOrgThenItShouldBeConnectedToAllThreeEdOrgIds()
        {
            // Arrange
            SetupTestEntities();

            // Act
            var edOrgs = _application.ApplicationEducationOrganizations.Select(x => x.EducationOrganizationId).ToList().Append(EdOrgId3);

            var editApplication = new TestEditApplicationModel
            {
                ApplicationId = _application.ApplicationId,
                ApplicationName = _application.ApplicationName,
                ClaimSetName = _application.ClaimSetName,
                EducationOrganizationIds = edOrgs,
                ProfileId = _application.Profiles.FirstOrDefault()?.ProfileId,
                VendorId = _application.Vendor.VendorId
            };

            Transaction(usersContext =>
            {
                var command = new EditApplicationCommand(usersContext);
                command.Execute(editApplication);
            });

            // Assert
            Transaction(UsersContext =>
            {
                var aeos = UsersContext.ApplicationEducationOrganizations.ToList();
                aeos.Count.ShouldBe(3);
                aeos.ShouldContain(x => x.EducationOrganizationId == EdOrgId1);
                aeos.ShouldContain(x => x.EducationOrganizationId == EdOrgId2);
                aeos.ShouldContain(x => x.EducationOrganizationId == EdOrgId3);
            });
        }

        [Test]
        public async Task GivenChangedEdOrgIdThenItShouldBeConnectedToOnlyTheOneEdOrgid()
        {
            // Arrange
            SetupTestEntities();

            // Act
            var editApplication = new TestEditApplicationModel
            {
                ApplicationId = _application.ApplicationId,
                ApplicationName = _application.ApplicationName,
                ClaimSetName = _application.ClaimSetName,
                // Now connected to just one
                EducationOrganizationIds = new List<int> { EdOrgId3 },
                ProfileId = _application.Profiles.FirstOrDefault()?.ProfileId,
                VendorId = _application.Vendor.VendorId
            };

            Transaction(usersContext =>
            {
                var command = new EditApplicationCommand(usersContext);
                command.Execute(editApplication);
            });

            // Assert
            Transaction(usersContext =>
            {
                var aeos = usersContext.ApplicationEducationOrganizations.ToList();
                aeos.Count.ShouldBe(1);
                var first = aeos.First();
                first.EducationOrganizationId.ShouldBe(EdOrgId3);
            });

            // Not trusting Entity Framework for the following check - directly querying the database
            const string sql = "select count(1) from dbo.ApiClientApplicationEducationOrganizations";
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(sql, connection);
            var count = (int)await command.ExecuteScalarAsync();
            count.ShouldBe(1);
        }

    private class TestEditApplicationModel : IEditApplicationModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int VendorId { get; set; }
        public string ClaimSetName { get; set; }
        public int? ProfileId { get; set; }
        public IEnumerable<int> EducationOrganizationIds { get; set; }
    }
}
