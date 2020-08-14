// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetOdsInstanceQueryTester
    {
        [Test]
        public void ShouldReturnOdsInstance()
        {
            var odsInstanceData = new List<OdsInstance>
            {
                new OdsInstance
                {
                    Name = "Foo",
                    InstanceType = "Cloud",
                    Status = CloudOdsStatus.Ok.DisplayName,
                    IsExtended = false,
                    Version = "0.0.1"
                }
            };

            var mockOdsInstances = MockExtensions.MockDbSet(odsInstanceData);

            var mockContext = new Mock<IUsersContext>();
            mockContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);

            var query = new GetOdsInstanceQuery(mockContext.Object);
            var odsInstance = query.Execute("Foo");
            odsInstance.Name.ShouldBe("Foo");
            odsInstance.InstanceType.ShouldBe("Cloud");
            odsInstance.Status.ShouldBe(CloudOdsStatus.Ok.DisplayName);
            odsInstance.IsExtended.ShouldBe(false);
            odsInstance.Version.ShouldBe("0.0.1");
        }
    }
}
