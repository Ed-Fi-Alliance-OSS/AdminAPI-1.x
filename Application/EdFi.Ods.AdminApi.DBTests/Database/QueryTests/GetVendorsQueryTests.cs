// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetVendorsQueryTests : PlatformUsersContextTestBase
{
    [Test]
    public void Should_retrieve_vendors()
    {
        var newVendor = new Vendor
        {
            VendorName = "test vendor",
            VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://testvendor.net" } },
        };

        Save(newVendor);

        Transaction(usersContext =>
        {
            var command = new GetVendorsQuery(usersContext, Testing.GetAppSettings());
            var allVendors = command.Execute();

            allVendors.ShouldNotBeEmpty();

            var vendor = allVendors.Single(v => v.VendorId == newVendor.VendorId);
            vendor.VendorName.ShouldBe("test vendor");
            vendor.VendorNamespacePrefixes.First().NamespacePrefix.ShouldBe("http://testvendor.net");
        });
    }

    [Test]
    public void Should_retrieve_vendors_with_offset_and_limit()
    {
        var vendors = new Vendor[5];

        for (var vendorIndex = 0; vendorIndex < 5; vendorIndex++)
        {
            vendors[vendorIndex] = new Vendor
            {
                VendorName = $"test vendor {vendorIndex + 1}",
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://testvendor.net" } }
            };
        }

        Save(vendors);

        Transaction(usersContext =>
        {
            var command = new GetVendorsQuery(usersContext, Testing.GetAppSettings());
            var commonQueryParams = new CommonQueryParams(0, 2);

            var vendorsAfterOffset = command.Execute(commonQueryParams);

            vendorsAfterOffset.ShouldNotBeEmpty();
            vendorsAfterOffset.Count.ShouldBe(2);

            vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 1");
            vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 2");

            commonQueryParams.Offset = 2;

            vendorsAfterOffset = command.Execute(commonQueryParams);

            vendorsAfterOffset.ShouldNotBeEmpty();
            vendorsAfterOffset.Count.ShouldBe(2);

            vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 3");
            vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 4");
            commonQueryParams.Offset = 4;

            vendorsAfterOffset = command.Execute(commonQueryParams);

            vendorsAfterOffset.ShouldNotBeEmpty();
            vendorsAfterOffset.Count.ShouldBe(1);

            vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 5");
        });
    }
}
