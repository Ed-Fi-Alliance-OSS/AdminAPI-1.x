// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class DeleteInstanceCommandTests : PlatformUsersContextTestBase
{
    [Test]
    public void ShouldDeleteInstance()
    {
        var newInstance = new Instance
        {
            TenantId = 1,
            OdsInstanceId = 1,
            Document = "{\"name\": \"Instance #3 - 2024\",\"instanceType\": null}\r\n",
            ApiCredentials = "{\"clientId\": \"test\",\"clientSecret\": \"testSecret\"}\r\n"
        };

        Save(newInstance);
        var odsInstanceId = newInstance.OdsInstanceId;

        Transaction(async dbContext =>
        {
            var commandRepository = new CommandRepository<Instance>(dbContext);
            var queryRepository = new QueriesRepository<Instance>(dbContext);
            var deleteInstanceCommand = new DeleteInstanceCommand(commandRepository, queryRepository);
            await deleteInstanceCommand.Execute(odsInstanceId);
        });


        Transaction(dbContext =>
        {
            var instances = dbContext.Instances.Where(oi => oi.OdsInstanceId == odsInstanceId).ToArray();
            instances.ShouldBeEmpty();
        });
    }
}
