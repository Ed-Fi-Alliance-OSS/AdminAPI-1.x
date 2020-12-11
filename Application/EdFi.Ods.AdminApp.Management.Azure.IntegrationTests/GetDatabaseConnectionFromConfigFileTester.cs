// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Azure.IntegrationTests.Testing;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    [TestFixture]
    public class GetDatabaseConnectionFromConfigFileTester : AzureIntegrationTestBase
    {
        [Test]
        public void ShouldMatchExpectedConnectionStringFromAppSettings()
        {
            var connectionStringsAccessor = new Mock<IOptions<ConnectionStrings>>();
            Scoped<IOptions<ConnectionStrings>>(connStrings =>
            {
                connectionStringsAccessor.Setup(x => x.Value).Returns(connStrings.Value);
            });

            var connectionStrings = connectionStringsAccessor.Object.Value;

            var rawSqlConnectionService = new RawSqlConnectionService(connectionStringsAccessor.Object);

            rawSqlConnectionService.GetDatabaseConnectionFromConfigFile(CloudOdsDatabaseNames.Admin).ConnectionString.ShouldBe(connectionStrings.Admin);
            rawSqlConnectionService.GetDatabaseConnectionFromConfigFile(CloudOdsDatabaseNames.Security).ConnectionString.ShouldBe(connectionStrings.Security);
        }
    }
}
