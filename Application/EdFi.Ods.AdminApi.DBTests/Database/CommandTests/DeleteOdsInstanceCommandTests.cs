// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApi.DBTests.Database.CommandTests;

[TestFixture]
public class DeleteOdsInstanceCommandTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldDeleteOdsInstance()
    {
        var newOdsInstance = new OdsInstance()
        {
            Name = "test",
            InstanceType = "type",
            Status = "status",
            Version = "version"
        };
        Save(newOdsInstance);
        var odsInstanceId = newOdsInstance.OdsInstanceId;

        Transaction(usersContext =>
        {
            var deleteOdsInstanceCommand = new DeleteOdsInstanceCommand(usersContext);
            deleteOdsInstanceCommand.Execute(odsInstanceId);
        });

        Transaction(usersContext => usersContext.OdsInstances.Where(v => v.OdsInstanceId == odsInstanceId).ToArray()).ShouldBeEmpty();
    }
}
