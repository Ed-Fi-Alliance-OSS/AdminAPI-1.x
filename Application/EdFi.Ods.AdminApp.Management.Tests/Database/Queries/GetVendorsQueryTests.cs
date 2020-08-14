// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetVendorsQueryTests : AdminDataTestBase
    {
        [Test]
        public void Should_retreive_vendors()
        {
            var newVendor = new Vendor
            {
                VendorName = "test vendor",
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://testvendor.net" } },
            };
            TestContext.Vendors.Add(newVendor);
            TestContext.SaveChanges();

            var command = new GetVendorsQuery(TestContext);
            var allVendors = command.Execute();
            allVendors.ShouldNotBeEmpty();

            var vendor = allVendors.Single(v => v.VendorId == newVendor.VendorId);
            vendor.VendorName.ShouldBe("test vendor");
            vendor.VendorNamespacePrefixes.First().NamespacePrefix.ShouldBe("http://testvendor.net");
        }
    }
}
