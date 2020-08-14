// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetVendorByIdQueryTests : AdminDataTestBase
    {
        [Test]
        public void ShouldGetVendorById()
        {
            var query = new GetVendorByIdQuery(TestContext);
            query.Execute(0).ShouldBeNull();

            var vendor = new Vendor {VendorName = "test vendor"};
            Save(vendor);
            var results = query.Execute(vendor.VendorId);
            results.VendorId.ShouldBe(vendor.VendorId);
            results.VendorName.ShouldBe("test vendor");
        }
    }
}
