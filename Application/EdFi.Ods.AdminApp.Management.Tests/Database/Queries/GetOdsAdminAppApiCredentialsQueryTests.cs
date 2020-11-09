// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries
{
    [TestFixture]
    public class GetOdsAdminAppApiCredentialQueryTester : PlatformUsersContextTestBase
    {
        [Test]
        public async Task ShouldExecute()
        {
            var odsSecretConfiguration = new OdsSecretConfiguration
            {
                ProductionApiKeyAndSecret = new OdsApiCredential("production", "api")
            };

            var instanceContext = new InstanceContext {Id = 1, Name = "Test instance"};

            var mockIOdsSecretConfigurationProvider = new Mock<IOdsSecretConfigurationProvider>();
            mockIOdsSecretConfigurationProvider.Setup(a => a.GetSecretConfiguration(1)).ReturnsAsync(odsSecretConfiguration);

            var query = new GetOdsAdminAppApiCredentialsQuery(mockIOdsSecretConfigurationProvider.Object, instanceContext);
            var credentials = await query.Execute();

            credentials.ProductionApiCredential.Key.ShouldBe(odsSecretConfiguration.ProductionApiKeyAndSecret.Key);
            credentials.ProductionApiCredential.Secret.ShouldBe(odsSecretConfiguration.ProductionApiKeyAndSecret.Secret);
        }
    }
}
