// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetOdsInstancesQueryTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldGetAllInstances()
    {
        Transaction(usersContext =>
        {
            CreateMultiple(2);
            var command = new GetOdsInstancesQuery(usersContext);
            var results = command.Execute();
            results.Count.ShouldBe(2);
        });
    }

    [Test]
    public void ShouldGetAllInstancesWithOffsetAndLimit()
    {
        Transaction(usersContext =>
        {
            CreateMultiple();
            var offset = 0;
            var limit = 2;

            var command = new GetOdsInstancesQuery(usersContext);
            var odsInstancesAfterOffset = command.Execute(offset, limit);

            odsInstancesAfterOffset.ShouldNotBeEmpty();
            odsInstancesAfterOffset.Count.ShouldBe(2);

            odsInstancesAfterOffset.ShouldContain(odsI => odsI.Name == "test ods instance 1");
            odsInstancesAfterOffset.ShouldContain(odsI => odsI.Name == "test ods instance 2");

            offset = 2;

            odsInstancesAfterOffset = command.Execute(offset, limit);
            
            odsInstancesAfterOffset.ShouldNotBeEmpty();
            odsInstancesAfterOffset.Count.ShouldBe(2);
            odsInstancesAfterOffset.ShouldContain(odsI => odsI.Name == "test ods instance 3");
            odsInstancesAfterOffset.ShouldContain(odsI => odsI.Name == "test ods instance 4");

            offset = 4;

            odsInstancesAfterOffset = command.Execute(offset, limit);

            odsInstancesAfterOffset.ShouldNotBeEmpty();
            odsInstancesAfterOffset.Count.ShouldBe(1);
            odsInstancesAfterOffset.ShouldContain(odsI => odsI.Name == "test ods instance 5");
        });
    }

    private static void CreateMultiple(int total = 5)
    {
        var odsInstances = new OdsInstance[total];

        for (var odsIndex = 0; odsIndex < total; odsIndex++)
        {
            odsInstances[odsIndex] = new OdsInstance
            {
                IsExtended = true,
                InstanceType = "test type",
                Name = $"test ods instance {odsIndex + 1}",
                Status = "test status",
                Version = "v6.1"
            };
        }
        Save(odsInstances);
    }
}
