// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
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

            Scoped<IUsersContext>(usersContext =>
            {
                var command = new GetVendorsQuery(usersContext);
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
                    VendorName = $"test vendor {vendorIndex+1}",
                    VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://testvendor.net" } }
                };
            }

            Save(vendors);

            Scoped<IUsersContext>(usersContext =>
            {
                var command = new GetVendorsQuery(usersContext);

                var offset = 0;
                var limit = 2;

                var vendorsAfterOffset = command.Execute(offset, limit);

                vendorsAfterOffset.ShouldNotBeEmpty();
                vendorsAfterOffset.Count.ShouldBe(2);

                vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 1");
                vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 2");

                offset = 2;

                vendorsAfterOffset = command.Execute(offset, limit);

                vendorsAfterOffset.ShouldNotBeEmpty();
                vendorsAfterOffset.Count.ShouldBe(2);

                vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 3");
                vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 4");
                offset = 4;

                vendorsAfterOffset = command.Execute(offset, limit);

                vendorsAfterOffset.ShouldNotBeEmpty();
                vendorsAfterOffset.Count.ShouldBe(1);

                vendorsAfterOffset.ShouldContain(v => v.VendorName == "test vendor 5");
            });
        }
    }
}
