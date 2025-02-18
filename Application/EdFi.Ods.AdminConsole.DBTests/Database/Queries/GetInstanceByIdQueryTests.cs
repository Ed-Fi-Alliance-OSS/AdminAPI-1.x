// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class GetInstanceByIdQueryTests : PlatformUsersContextTestBase
{
    private IOptions<AppSettings> _options { get; set; }

    [OneTimeSetUp]
    public virtual async Task FixtureSetup()
    {
        AdminConsoleSettings appSettings = new AdminConsoleSettings();
        await Task.Yield();
    }

    [Test]
    public void ShouldExecute()
    {
        var instanceDocument = "{\"instanceId\":\"DEF456\",\"tenantId\":\"def456\",\"instanceName\":\"Mock Instance 2\",\"instanceType\":\"Type B\",\"connectionType\":\"Type Y\",\"clientId\":\"CLIENT321\",\"clientSecret\":\"SECRET456\",\"baseUrl\":\"https://localhost/api\",\"authenticationUrl\":\"https://localhost/api/oauth/token\",\"resourcesUrl\":\"https://localhost/api\",\"schoolYears\":[2024,2025],\"isDefault\":false,\"verificationStatus\":null,\"provider\":\"Local\"}";

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        ExpandoObject documentExpandObject = JsonSerializer.Deserialize<ExpandoObject>(instanceDocument, options);

        Instance result = null;

        var newInstance = new TestInstance
        {
            OdsInstanceId = 1,
            TenantId = 1,
            EdOrgId = 1,
            Document = documentExpandObject
        };

        Transaction(async dbContext =>
        {
            var repository = new CommandRepository<Instance>(dbContext);
            var command = new AddInstanceCommand(repository, Testing.GetEncryptionKeyResolver(), new EncryptionService());

            result = await command.Execute(newInstance);
        });
        Transaction(async dbContext =>
        {
            var repository = new QueriesRepository<Instance>(dbContext);
            var query = new GetInstanceByIdQuery(repository, Testing.GetEncryptionKeyResolver(), new EncryptionService());
            var instance = await query.Execute(result.OdsInstanceId);

            instance.DocId.ShouldBe(result.DocId);
            instance.TenantId.ShouldBe(newInstance.TenantId);
            instance.OdsInstanceId.ShouldBe(newInstance.OdsInstanceId);
            instance.EdOrgId.ShouldBe(newInstance.EdOrgId);
            instance.Document.ShouldBe(JsonSerializer.Serialize(newInstance.Document));
        });
    }

    private class TestInstance : IAddInstanceModel
    {
        public int DocId { get; }
        public int TenantId { get; set; }
        public int OdsInstanceId { get; set; }
        public int? EdOrgId { get; set; }
        public ExpandoObject Document { get; set; }
        public ExpandoObject ApiCredentials { get; set; }
    }
}
