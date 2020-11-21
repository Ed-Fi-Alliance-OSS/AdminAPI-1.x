// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [TestFixture]
    public class GetOdsStatusQueryTester
    {
        [Test]
        public void ShouldReturnInstanceNotRegisteredIfRecordDoesNotExist()
        {
            var mockOdsInstances = MockExtensions.EmptyMockDbSet<OdsInstance>();
            
            var mockContext = new Mock<IUsersContext>();
            mockContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);

            var query = new GetOdsStatusQuery(mockContext.Object);
            var status = query.Execute("Foo");
            status.ShouldBe(CloudOdsStatus.InstanceNotRegistered);
        }

        [Test]
        public void ShouldReturnStatusIfRecordExists()
        {
            var odsInstanceData = new List<OdsInstance>
            {
                new OdsInstance
                {
                    Name = "Foo",
                    Status = CloudOdsStatus.Ok.DisplayName
                }
            };

            var mockOdsInstances = MockExtensions.MockDbSet(odsInstanceData);

            var mockContext = new Mock<IUsersContext>();
            mockContext.Setup(c => c.OdsInstances).Returns(mockOdsInstances.Object);

            var query = new GetOdsStatusQuery(mockContext.Object);
            var status = query.Execute("Foo");
            status.ShouldBe(CloudOdsStatus.Ok);
        }
    }
}
