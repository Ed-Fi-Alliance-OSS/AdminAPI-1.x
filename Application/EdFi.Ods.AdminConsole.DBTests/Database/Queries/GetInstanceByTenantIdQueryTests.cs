// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Common.Helpers;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class GetInstancesByTenantIdQueryTests : PlatformUsersContextTestBase
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
        Instance result = null;

        var newInstance = new TestInstance
        {
            InstanceId = 1,
            TenantId = 1,
            EdOrgId = 1,
            Document = instanceDocument
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
            var query = new GetInstancesByTenantIdQuery(repository, Testing.GetEncryptionKeyResolver(), new EncryptionService());
            IEnumerable<Instance> instances = await query.Execute(result.TenantId);
            instances.Count().ShouldBe(1);
            instances.FirstOrDefault().DocId.ShouldBe(result.DocId);
            instances.FirstOrDefault().TenantId.ShouldBe(newInstance.TenantId);
            instances.FirstOrDefault().InstanceId.ShouldBe(newInstance.InstanceId);
            instances.FirstOrDefault().EdOrgId.ShouldBe(newInstance.EdOrgId);
            instances.FirstOrDefault().Document.ShouldBe(newInstance.Document);
        });
    }

    private class TestInstance : IAddInstanceModel
    {
        public int DocId { get; }
        public int TenantId { get; set; }
        public int InstanceId { get; set; }
        public int? EdOrgId { get; set; }
        public string Document { get; set; }
    }
}
