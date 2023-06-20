// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetApplicationByOdsInstanceIdQueryTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldGetBasicApplicationData()
    {
        var vendor = new Vendor { VendorName = "test vendor" };
        var odsInstance = new OdsInstance
        {
            OdsInstanceId = 1,
            Name = "Test Instance",
            InstanceType = "Ods",
            IsExtended = false,
            Status = "OK",
            Version = "1.0.0"
        };

        var application = new Application
        {
            ApplicationName = "test application",
            ClaimSetName = "test claim set",
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
            OdsInstance = odsInstance
        };
        
        Save(odsInstance, vendor, application);

        Transaction(usersContext =>
        {
            var getApplicationsByOdsInstanceIdQuery = new GetApplicationsByOdsInstanceIdQuery(usersContext);
            var results = getApplicationsByOdsInstanceIdQuery.Execute(odsInstance.OdsInstanceId);
            results.Single().ApplicationName.ShouldBe("test application");
            results.Single().ClaimSetName.ShouldBe("test claim set");
        });
    }

    [Test]
    public void ShouldGetApplicationEducationOrganization()
    {
        var vendor = new Vendor { VendorName = "test vendor" };
        var odsInstance = new OdsInstance
        {
            OdsInstanceId = 1,
            Name = "Test Instance",
            InstanceType = "Ods",
            IsExtended = false,
            Status = "OK",
            Version = "1.0.0"
        };
        var application = new Application
        {
            ApplicationName = "test application",
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
            OdsInstance = odsInstance
        };

        var applicationOrganization = new ApplicationEducationOrganization { Application = application };

        application.ApplicationEducationOrganizations.Add(applicationOrganization);
        
        Save(odsInstance, vendor, application);

        var organizationId = applicationOrganization.ApplicationEducationOrganizationId;
        organizationId.ShouldBeGreaterThan(0);

        Transaction(usersContext =>
        {
            var getApplicationsByOdsInstanceIdQuery = new GetApplicationsByOdsInstanceIdQuery(usersContext);
            var results = getApplicationsByOdsInstanceIdQuery.Execute(odsInstance.OdsInstanceId);
            results.Single().ApplicationEducationOrganizations.Single().ApplicationEducationOrganizationId.ShouldBe(organizationId);
        });
    }

    [Test]
    public void ShouldGetApplicationProfile()
    {
        var vendor = new Vendor { VendorName = "test vendor" };
        var odsInstance = new OdsInstance
        {
            OdsInstanceId = 1,
            Name = "Test Instance",
            InstanceType = "Ods",
            IsExtended = false,
            Status = "OK",
            Version = "1.0.0"
        };
        var application = new Application
        {
            ApplicationName = "test application",
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
            OdsInstance = odsInstance
        };
        var profile = new Profile
        {
            Applications = new List<Application> { application },
            ProfileName = "test profile"
        };
        application.Profiles.Add(profile);

        Save(odsInstance, vendor, application);
        var profileId = profile.ProfileId;
        profileId.ShouldBeGreaterThan(0);

        Transaction(usersContext =>
        {
            var getApplicationsByOdsInstanceIdQuery = new GetApplicationsByOdsInstanceIdQuery(usersContext);
            var results = getApplicationsByOdsInstanceIdQuery.Execute(odsInstance.OdsInstanceId);
            results.Single().Profiles.Single().ProfileId.ShouldBe(profileId);
        });
    }

    [Test]
    public void ShouldThrowWhenOdsInstanceIdIsInvalid()
    {
        Transaction(usersContext =>
        {
            var getApplicationsByOdsInstanceIdQuery = new GetApplicationsByOdsInstanceIdQuery(usersContext);
            Should.Throw<NotFoundException<int>>(() =>
            {
                getApplicationsByOdsInstanceIdQuery.Execute(int.MaxValue);
            });
        });
    }
}
