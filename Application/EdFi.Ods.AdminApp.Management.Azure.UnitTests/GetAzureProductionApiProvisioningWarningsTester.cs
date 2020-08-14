// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    [TestFixture]
    public class GetAzureProductionApiProvisioningWarningsTester
    {
        private static AzureCloudOdsInstance GetAzureCloudOdsInstance()
        {
            return new AzureCloudOdsInstance
            {
                Edition = "",
                FriendlyName = "",
                SystemId = "",
                SystemName = "",
                Version = ""
            };
        }

        [Test]
        public async Task ShouldNotProduceWarningWhenRunningAgainstNonAzureSql()
        {
            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            mockCloudOdsDatabaseNameProvider.Setup(x => x.GetDatabaseName(It.IsAny<CloudOdsDatabases>()))
                .Returns(It.IsAny<string>());

            var azureCloudOdsWebsitePerformanceLevelQuery = new Mock<IGetAzureCloudOdsWebsitePerformanceLevelQuery>();
            azureCloudOdsWebsitePerformanceLevelQuery.Setup(x => x.Execute(It.IsAny<ICloudOdsOperationContext>()))
                .ReturnsAsync(AzureWebsitePerformanceLevel.S3);

            var mockRawSqlConnection = new Mock<IRawSqlConnectionService>();
            mockRawSqlConnection.Setup(x => x.GetDatabaseConnectionFromConfigFile(It.IsAny<string>())).Returns((SqlConnection)null);

            var azureDbManagementService = new Mock<AzureDatabaseManagementService>(mockRawSqlConnection.Object);
            azureDbManagementService.Setup(x => x.GetDatabasePerformanceLevel(It.IsAny<SqlConnection>(), It.IsAny<string>()))
                .Returns(new AzureSqlDatabasePerformanceLevel(null, null));

            var sut = new GetAzureProductionApiProvisioningWarningsQuery(
                azureDbManagementService.Object,
                azureCloudOdsWebsitePerformanceLevelQuery.Object,
                mockRawSqlConnection.Object,
                mockCloudOdsDatabaseNameProvider.Object);
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.ShouldBeEmpty();
        }

        [Test]
        public async Task ShouldNotProduceWarningWhenRunningAgainstExpectedAzureSqlTier()
        {
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            mockCloudOdsDatabaseNameProvider.Setup(x => x.GetDatabaseName(It.IsAny<CloudOdsDatabases>()))
                .Returns(It.IsAny<string>());

            var azureCloudOdsWebsitePerformanceLevelQuery = new Mock<IGetAzureCloudOdsWebsitePerformanceLevelQuery>();
            azureCloudOdsWebsitePerformanceLevelQuery.Setup(x => x.Execute(It.IsAny<ICloudOdsOperationContext>()))
                .ReturnsAsync(AzureWebsitePerformanceLevel.S3);

            var mockRawSqlConnection = new Mock<IRawSqlConnectionService>();
            mockRawSqlConnection.Setup(x => x.GetDatabaseConnectionFromConfigFile(It.IsAny<string>())).Returns((SqlConnection)null);

            var azureDbManagementService = new Mock<AzureDatabaseManagementService>(mockRawSqlConnection.Object);
            azureDbManagementService.Setup(x => x.GetDatabasePerformanceLevel(It.IsAny<SqlConnection>(), It.IsAny<string>()))
                .Returns(AzureSqlDatabasePerformanceLevel.P1);

            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery(azureDbManagementService.Object, azureCloudOdsWebsitePerformanceLevelQuery.Object, mockRawSqlConnection.Object, mockCloudOdsDatabaseNameProvider.Object);
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.ShouldBeEmpty();
        }

        [Test]
        public async Task ShouldProduceWarningWhenRunningAgainstLowerThanRecommendedAzureSqlTier()
        {
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            mockCloudOdsDatabaseNameProvider.Setup(x => x.GetDatabaseName(It.IsAny<CloudOdsDatabases>()))
                .Returns(It.IsAny<string>());

            var azureCloudOdsWebsitePerformanceLevelQuery = new Mock<IGetAzureCloudOdsWebsitePerformanceLevelQuery>();
            azureCloudOdsWebsitePerformanceLevelQuery.Setup(x => x.Execute(It.IsAny<ICloudOdsOperationContext>()))
                .ReturnsAsync(AzureWebsitePerformanceLevel.S3);

            var mockRawSqlConnection = new Mock<IRawSqlConnectionService>();
            mockRawSqlConnection.Setup(x => x.GetDatabaseConnectionFromConfigFile(It.IsAny<string>())).Returns((SqlConnection)null);

            var azureDbManagementService = new Mock<AzureDatabaseManagementService>(mockRawSqlConnection.Object);
            azureDbManagementService.Setup(x => x.GetDatabasePerformanceLevel(It.IsAny<SqlConnection>(), It.IsAny<string>()))
                .Returns(AzureSqlDatabasePerformanceLevel.S1);

            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery(azureDbManagementService.Object, azureCloudOdsWebsitePerformanceLevelQuery.Object, mockRawSqlConnection.Object, mockCloudOdsDatabaseNameProvider.Object);
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.Count().ShouldBe(1);
            result.Warnings.First().ShouldBe("Azure Sql performance tier 'P1' or above is recommended");
        }

        [Test]
        public async Task ShouldProduceWarningWhenWebsiteLowerThanRecommendedTier()
        {
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            mockCloudOdsDatabaseNameProvider.Setup(x => x.GetDatabaseName(It.IsAny<CloudOdsDatabases>()))
                .Returns(It.IsAny<string>());

            var azureCloudOdsWebsitePerformanceLevelQuery = new Mock<IGetAzureCloudOdsWebsitePerformanceLevelQuery>();
            azureCloudOdsWebsitePerformanceLevelQuery.Setup(x => x.Execute(It.IsAny<ICloudOdsOperationContext>()))
                .ReturnsAsync(AzureWebsitePerformanceLevel.S2);

            var mockRawSqlConnection = new Mock<IRawSqlConnectionService>();
            mockRawSqlConnection.Setup(x => x.GetDatabaseConnectionFromConfigFile(It.IsAny<string>())).Returns((SqlConnection)null);

            var azureDbManagementService = new Mock<AzureDatabaseManagementService>(mockRawSqlConnection.Object);
            azureDbManagementService.Setup(x => x.GetDatabasePerformanceLevel(It.IsAny<SqlConnection>(), It.IsAny<string>()))
                .Returns(AzureSqlDatabasePerformanceLevel.P1);

            var cloudOdsInstance = GetAzureCloudOdsInstance();

            var sut = new GetAzureProductionApiProvisioningWarningsQuery(azureDbManagementService.Object, azureCloudOdsWebsitePerformanceLevelQuery.Object, mockRawSqlConnection.Object, mockCloudOdsDatabaseNameProvider.Object);
            var result = await sut.Execute(cloudOdsInstance);

            result.Warnings.Count().ShouldBe(1);
            result.Warnings.First().ShouldBe("Azure Website performance tier 'S3' or above is recommended");
        }
    }
}
