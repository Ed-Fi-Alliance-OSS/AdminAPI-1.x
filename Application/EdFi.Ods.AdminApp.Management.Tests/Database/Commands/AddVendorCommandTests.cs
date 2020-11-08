// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Commands;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class AddVendorCommandTests : AdminDataTestBase
    {
        [Test]
        public void ShouldAddVendor()
        {
            var newVendor = new Mock<IAddVendorModel>();
            newVendor.Setup(x => x.Company).Returns("test vendor");
            newVendor.Setup(x => x.NamespacePrefix).Returns("http://www.test.com/");
            newVendor.Setup(x => x.ContactName).Returns("test user");
            newVendor.Setup(x => x.ContactEmailAddress).Returns("test@test.com");

            int id = 0;
            Scoped<IUsersContext>(usersContext =>
            {
                var command = new AddVendorCommand(usersContext);

                id = command.Execute(newVendor.Object);
                id.ShouldBeGreaterThan(0);
            });

            Transaction(usersContext =>
            {
                var vendor = usersContext.Vendors.Single(v => v.VendorId == id);
                vendor.VendorName.ShouldBe("test vendor");
                vendor.VendorNamespacePrefixes.First().NamespacePrefix.ShouldBe("http://www.test.com/");
                vendor.Users.Single().FullName.ShouldBe("test user");
                vendor.Users.Single().Email.ShouldBe("test@test.com");
            });
        }
    }
}
