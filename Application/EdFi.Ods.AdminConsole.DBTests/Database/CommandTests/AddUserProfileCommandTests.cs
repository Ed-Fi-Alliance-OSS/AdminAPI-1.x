// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.UserProfiles.Commands;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class AddUserProfileCommandTests : PlatformUsersContextTestBase
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
        var userProfileDocument = "{\"tenantId\": \"abc123\",\"firstName\": \"John\",\"lastName\": \"Doe\",\"userName\": \"jdoe\",\"email\": \"john.doe@example.com\",\"preferences\": [{\"code\": \"language\",\"value\": \"en\"},{\"code\": \"timezone\",\"value\": \"America\\/New_York\"}],\"extensions\": [{\"code\": \"extraInfo\",\"data\": \"some value\"}],\"tenants\": [{\"createdBy\": \"admin\",\"createdDateTime\": \"2022-01-15T12:00:00Z\",\"domains\": [\"companyA.com\"],\"isDemo\": false,\"isIdentityProviders\": [\"Google\", \"Azure AD\"],\"lastModifiedBy\": \"admin\",\"lastModifiedDateTime\": \"2022-05-20T08:30:00Z\",\"organizationIdentifier\": \"ORG001\",\"organizationName\": \"Company A\",\"state\": \"active\",\"subscriptions\": [],\"subscriptionsMigrated\": true,\"tenantId\": \"tenant1\",\"tenantStatus\": \"active\",\"tenantType\": \"standard\"},{\"createdBy\": \"admin\",\"createdDateTime\": \"2021-03-10T09:00:00Z\",\"domains\": [\"companyB.com\"],\"isDemo\": true,\"isIdentityProviders\": [\"Okta\"],\"lastModifiedBy\": \"admin\",\"lastModifiedDateTime\": \"2023-07-11T13:45:00Z\",\"organizationIdentifier\": \"ORG002\",\"organizationName\": \"Company B\",\"state\": \"inactive\",\"subscriptions\": [],\"subscriptionsMigrated\": false,\"tenantId\": \"tenant2\",\"tenantStatus\": \"inactive\",\"tenantType\": \"demo\"}],\"selectedTenant\": {\"createdBy\": \"admin\",\"createdDateTime\": \"2022-01-15T12:00:00Z\",\"domains\": [\"companyA.com\"],\"isDemo\": false,\"isIdentityProviders\": [\"Google\", \"Azure AD\"],\"lastModifiedBy\": \"admin\",\"lastModifiedDateTime\": \"2022-05-20T08:30:00Z\",\"organizationIdentifier\": \"ORG001\",\"organizationName\": \"Company A\",\"state\": \"active\",\"subscriptions\": [],\"subscriptionsMigrated\": true,\"tenantId\": \"tenant1\",\"tenantStatus\": \"active\",\"tenantType\": \"standard\"},\"tenantsTotalCount\": 2}";

        var encryptionService = new EncryptionService();
        var encryptionKey = Testing.GetEncryptionKeyResolver().GetEncryptionKey();

        Transaction(async dbContext =>
        {
            var repository = new CommandRepository<UserProfile>(dbContext);
            var newUserProfile = new TestUserProfile
            {
                InstanceId = 1,
                TenantId = 1,
                EdOrgId = 1,
                Document = userProfileDocument
            };

            var command = new AddUserProfileCommand(repository);

            var result = await command.Execute(newUserProfile);
        });

        Transaction(dbContext =>
        {
            var persistedUserProfile = dbContext.UserProfiles;
            persistedUserProfile.Count().ShouldBe(1);
            persistedUserProfile.First().DocId.ShouldBe(1);
            persistedUserProfile.First().TenantId.ShouldBe(1);
            persistedUserProfile.First().InstanceId.ShouldBe(1);
            persistedUserProfile.First().EdOrgId.ShouldBe(1);
            persistedUserProfile.First().Document.ShouldBe(userProfileDocument);
        });
    }

    private class TestUserProfile : IAddUserProfileModel
    {
        public int InstanceId { get; set; }
        public int? EdOrgId { get; set; }
        public int TenantId { get; set; }
        public string Document { get; set; }
    }
}
