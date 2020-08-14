// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using NUnit.Framework;
using Shouldly;
using VendorUser = EdFi.Admin.DataAccess.Models.User;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Commands
{
    [TestFixture]
    public class RegenerateApiClientSecretCommandTests : AdminDataTestBase
    {
        [Test]
        public void ShouldFailIfApplicationDoesNotExist()
        {
            var command = new RegenerateApiClientSecretCommand(TestContext);
            Assert.Throws<InvalidOperationException>(() => command.Execute(0));
        }

        [Test]
        public void ShouldReportFailureIfApiClientDoesNotExist()
        {
            var application = new Application
            {
                ApplicationName = "Api Client Secret Test App",
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri
            };

            Save(application);

            var command = new RegenerateApiClientSecretCommand(TestContext);
            Assert.Throws<InvalidOperationException>(() => command.Execute(application.ApplicationId));
        }

        [Test]
        public void ShouldUpdateApiClientSecret()
        {
            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = "http://tests.com" } },
                VendorName = "Integration Tests"
            };

            var user = new VendorUser
            {
                Email = "nobody@nowhere.com",
                FullName = "Integration Tests",
                Vendor = vendor
            };

            var profile = new Profile
            {
                ProfileName = "Test Profile"
            };

            var apiClient = new ApiClient(true)
            {
                Name = "Integration Test"
            };

            var application = new Application
            {
                ApplicationName = "Test Application",
                ClaimSetName = "FakeClaimSet",
                ApiClients = new List<ApiClient>(),
                Vendor = vendor,
                Profiles = new List<Profile>(),
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri
            };

            application.ApiClients.Add(apiClient);
            application.Profiles.Add(profile);

            Save(vendor, user, profile, application);

            var orignalKey = apiClient.Key;
            var originalSecret = apiClient.Secret;

            var command = new RegenerateApiClientSecretCommand(SetupContext);
            var result = command.Execute(application.ApplicationId);

            var updatedApiClient = TestContext.Clients.Single(c => c.ApiClientId == apiClient.ApiClientId);
            
            result.Key.ShouldBe(orignalKey);
            result.Secret.ShouldNotBe(originalSecret);
            result.Secret.ShouldNotBeEmpty();

            updatedApiClient.Key.ShouldBe(result.Key);
            updatedApiClient.Secret.ShouldBe(result.Secret);
        }
    }
}
