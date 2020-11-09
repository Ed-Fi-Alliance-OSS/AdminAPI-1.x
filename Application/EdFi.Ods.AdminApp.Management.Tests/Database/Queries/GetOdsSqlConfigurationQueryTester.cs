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
    public class GetOdsSqlConfigurationQueryTester : PlatformUsersContextTestBase
    {
        [Test]
        public async Task ShouldExecute()
        {
            var odsSqlConfiguration = new OdsSqlConfiguration
            {
                HostName = "test",
                AdminCredentials = new OdsSqlAdminCredential { UserName = "test", Password = "test"},
                AdminAppCredentials = new OdsSqlCredential { UserName = "test", Password = "test" },
                ProductionApiCredentials = new OdsSqlCredential { UserName = "test", Password = "test" }
            };

            var mockIOdsSecretConfigurationProvider = new Mock<IOdsSecretConfigurationProvider>();
            mockIOdsSecretConfigurationProvider.Setup(a => a.GetSqlConfiguration()).ReturnsAsync(odsSqlConfiguration);

            var query = new GetOdsSqlConfigurationQuery(mockIOdsSecretConfigurationProvider.Object);
            var configuration = await query.Execute();

            ValidateCredentials(configuration.AdminCredentials, odsSqlConfiguration.AdminCredentials);
            ValidateCredentials(configuration.AdminAppCredentials, odsSqlConfiguration.AdminAppCredentials);
            ValidateCredentials(configuration.ProductionApiCredentials, odsSqlConfiguration.ProductionApiCredentials);
        }

        private void ValidateCredentials(OdsSqlCredential actual, OdsSqlCredential expected)
        {
            actual.UserName.ShouldBe(expected.UserName);
            actual.Password.ShouldBe(expected.Password);
        }

        private void ValidateCredentials(OdsSqlAdminCredential actual, OdsSqlAdminCredential expected)
        {
            actual.UserName.ShouldBe(expected.UserName);
            actual.Password.ShouldBe(expected.Password);
        }
    }
}
