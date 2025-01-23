// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using System;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetApiClientIdByApplicationIdQueryTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldGetApiClientIdByApplicationIdData()
    {
        var vendor = new Vendor { VendorName = "test vendor" };
        var odsInstance = new OdsInstance
        {
            Name = "Test Instance",
            InstanceType = "Ods",
            ConnectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False"
        };

        var application = new Application
        {
            ApplicationName = "test application",
            ClaimSetName = "test claim set",
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
        };

        var apiClient = new ApiClient
        {
            Application = application,
            Key = "key",
            Secret = "secret",
            Name = application.ApplicationName,
            IsApproved = true,
            UseSandbox = false,
            KeyStatus = "Active"
        };
        Save(odsInstance, vendor, application, apiClient);
        int applicationId = application.ApplicationId;
        Transaction(usersContext =>
        {
            var getApiClientIdByApplicationIdQuery = new GetApiClientIdByApplicationIdQuery(usersContext);
            var results = getApiClientIdByApplicationIdQuery.Execute(applicationId);
            results.ShouldNotBeNull();
        });
    }

    [Test]
    public void ShouldNotGetApiClientIdWithDifferentApplicationId()
    {
        var vendor = new Vendor { VendorName = "test vendor" };
        var odsInstance = new OdsInstance
        {
            Name = "Test Instance",
            InstanceType = "Ods",
            ConnectionString = "Data Source=(local);Initial Catalog=EdFi_Ods;Integrated Security=True;Encrypt=False"
        };

        var application = new Application
        {
            ApplicationName = "test application",
            ClaimSetName = "test claim set",
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
        };

        var apiClient = new ApiClient
        {
            Application = application,
            Key = "key",
            Secret = "secret",
            Name = application.ApplicationName,
            IsApproved = true,
            UseSandbox = false,
            KeyStatus = "Active"
        };
        Save(odsInstance, vendor, application, apiClient);
        int applicationId = 999;
        Transaction(usersContext =>
        {
            var getApiClientIdByApplicationIdQuery = new GetApiClientIdByApplicationIdQuery(usersContext);
            Should.Throw<NotFoundException<int>>(() =>
            {
                getApiClientIdByApplicationIdQuery.Execute(applicationId);
            });
        });
    }
}
