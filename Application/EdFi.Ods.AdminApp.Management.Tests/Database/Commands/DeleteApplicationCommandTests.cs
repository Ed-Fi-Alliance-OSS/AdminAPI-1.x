// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Commands;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class DeleteApplicationCommandTests : AdminDataTestBase
    {
        [Test]
        public void ShouldDeleteApplication()
        {
            var application = new Application {ApplicationName = "test application", OperationalContextUri = OperationalContext.DefaultOperationalContextUri };
            Save(application);
            var applicationId = application.ApplicationId;

            var deleteApplicationCommand = new DeleteApplicationCommand(TestContext);
            deleteApplicationCommand.Execute(applicationId);
            TestContext.Applications.Where(a => a.ApplicationId == applicationId).ShouldBeEmpty();
        }

        [Test]
        public void ShouldDeleteApplicationWithClient()
        {
            var application = new Application { ApplicationName = "test application", OperationalContextUri = OperationalContext.DefaultOperationalContextUri };

            var client = new ApiClient
            {
                Name = "test client",
                Key = "n/a",
                Secret = "n/a",
                ActivationCode = "fake activation code"
            };

            var clientAccessToken = new ClientAccessToken
            {
                ApiClient = client,
                Expiration = DateTime.Now.AddDays(1)
            };

            client.ClientAccessTokens.Add(clientAccessToken);
            
            application.ApiClients.Add(client);
            Save(application);

            var applicationId = application.ApplicationId;
            applicationId.ShouldBeGreaterThan(0);

            var clientId = client.ApiClientId;
            clientId.ShouldBeGreaterThan(0);

            var tokenId = clientAccessToken.Id;
            tokenId.ShouldNotBe(Guid.Empty);

            var deleteApplicationCommand = new DeleteApplicationCommand(TestContext);
            deleteApplicationCommand.Execute(applicationId);

            TestContext.Applications.Where(a => a.ApplicationId == applicationId).ShouldBeEmpty();
            TestContext.Clients.Where(c => c.ApiClientId == clientId).ShouldBeEmpty();
        }

        [Test]
        public void ShouldDeleteApplicationWithOrganization()
        {
            var application = new Application { ApplicationName = "test application", OperationalContextUri = OperationalContext.DefaultOperationalContextUri };

            var client = new ApiClient
            {
                Name = "test client",
                Key = "n/a",
                Secret = "n/a",
            };

            var organization = new ApplicationEducationOrganization
            {
                Application = application,
                Clients = new List<ApiClient> {client}
            };

            application.ApiClients.Add(client);
            application.ApplicationEducationOrganizations.Add(organization);
            Save(application);

            var applicationId = application.ApplicationId;
            applicationId.ShouldBeGreaterThan(0);

            var organizationId = organization.ApplicationEducationOrganizationId;
            organizationId.ShouldBeGreaterThan(0);

            var deleteApplicationCommand = new DeleteApplicationCommand(TestContext);
            deleteApplicationCommand.Execute(applicationId);

            TestContext.Applications.Where(a => a.ApplicationId == applicationId).ShouldBeEmpty();
            TestContext.ApplicationEducationOrganizations.Where(o => o.ApplicationEducationOrganizationId == organizationId).ShouldBeEmpty();
        }

        [Test]
        public void ShouldDeleteApplicationWithProfile()
        {
            var application = new Application { ApplicationName = "test application", OperationalContextUri = OperationalContext.DefaultOperationalContextUri };
            var profile = new Profile {ProfileName = "test profile"};
            application.Profiles.Add(profile);

            Save(application);

            var applicationId = application.ApplicationId;
            applicationId.ShouldBeGreaterThan(0);

            var profileId = profile.ProfileId;
            profileId.ShouldBeGreaterThan(0);

            var deleteApplicationCommand = new DeleteApplicationCommand(TestContext);
            deleteApplicationCommand.Execute(applicationId);

            TestContext.Applications.Where(a => a.ApplicationId == applicationId).ShouldBeEmpty();
            TestContext.Profiles.Where(p => p.ProfileId == profileId).ShouldNotBeEmpty();
        }
    }
}
