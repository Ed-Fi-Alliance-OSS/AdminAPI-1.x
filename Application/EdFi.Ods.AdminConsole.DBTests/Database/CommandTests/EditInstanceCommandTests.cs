// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class EditInstanceCommandTests : PlatformUsersContextTestBase
{
    private int _odsInstanceId;

    [SetUp]
    public void Init()
    {
        var originalOdsInstance = new Instance
        {
            TenantId = 1,
            OdsInstanceId = 1,
            Document = "{\"name\": \"Instance #3 - 2024\",\"instanceType\": null}\r\n"
        };


        Save(originalOdsInstance);
        _odsInstanceId = originalOdsInstance.OdsInstanceId;
    }

    [Test]
    public void ShouldEditInstance()
    {
        var TenantId = 1;
        var Document = "{\"name\": \"Instance #4 - 2024\",\"instanceType\": null}\r\n";

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        ExpandoObject documentExpandObject = JsonSerializer.Deserialize<ExpandoObject>(Document, options);

        var newInstanceData = new Mock<IEditInstanceModel>();
        newInstanceData.Setup(v => v.TenantId).Returns(TenantId);
        newInstanceData.Setup(v => v.Document).Returns(documentExpandObject);

        var encryptionService = new EncryptionService();
        var encryptionKey = Testing.GetEncryptionKeyResolver().GetEncryptionKey();

        Transaction(async dbContext =>
        {
            var repository = new CommandRepository<Instance>(dbContext);
            var command = new EditInstanceCommand(repository, Testing.GetEncryptionKeyResolver(), encryptionService);

            var result = await command.Execute(_odsInstanceId, newInstanceData.Object);
        });

        Transaction(dbContext =>
        {
            var persistedInstance = dbContext.Instances;
            persistedInstance.Count().ShouldBe(1);
            persistedInstance.First().TenantId.ShouldBe(1);
            persistedInstance.First().OdsInstanceId.ShouldBe(1);
            persistedInstance.First().Document.ShouldBe(Document);
        });
    }
}
