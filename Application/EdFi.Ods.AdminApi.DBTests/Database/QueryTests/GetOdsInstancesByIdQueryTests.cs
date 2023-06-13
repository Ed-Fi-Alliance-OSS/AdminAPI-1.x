// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetOdsInstanceByIdQueryTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldGetInstanceById()
    {
       
        Transaction(usersContext =>
        {
            var odsInstance = new OdsInstance
            {
                IsExtended = true,
                InstanceType = "test type",
                Name = "test ods instance 1",
                Status = "test status",
                Version = "v6.1"
            };
            Save(odsInstance);
            var command = new GetOdsInstanceQuery(usersContext);
            var result = command.Execute(odsInstance.OdsInstanceId);
            result.OdsInstanceId.ShouldBe(odsInstance.OdsInstanceId);
            result.Name.ShouldBe("test ods instance 1");
        });
    }
}
