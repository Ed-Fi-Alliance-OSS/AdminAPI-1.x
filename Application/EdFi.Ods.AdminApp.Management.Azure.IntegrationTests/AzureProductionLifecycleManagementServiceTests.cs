// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Setup;
using EdFi.Ods.AdminApp.Management.Helpers;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    //These tests requires the following items to be defined in App.config:
    // -AzureSql connection string
    // -ProductionApiUrl, ProductionApiKey, ProductionApiSecret
    //The login provided in the connection string should be in the sysadmin role on the Azure SQL Server
    [Explicit]
    [TestFixture]
    public class AzureProductionLifecycleManagementServiceTests : AzureIntegrationTestBase
    {
        [Test]
        public async Task ShouldResetToMinimal()
        {
            var mockRawSqlConnectionService = new Mock<IRawSqlConnectionService>();
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            var mockCloudOdsDatabaseSqlServerSecurityConfiguration = new Mock<ICloudOdsDatabaseSqlServerSecurityConfiguration>();
            var dbManagementService = new AzureDatabaseManagementService(mockRawSqlConnectionService.Object);
            var dbConfigurator = new SqlServerCloudOdsDatabaseSecurityConfigurator(mockRawSqlConnectionService.Object, mockCloudOdsDatabaseNameProvider.Object);
            mockCloudOdsDatabaseSqlServerSecurityConfiguration
                .Setup(a => a.GetSetupConfiguration(It.IsAny<OdsSqlConfiguration>()))
                .Returns(GetSetupConfiguration(GetSqlConfiguration()));

            var dbLifecycleManagementService = new AzureDatabaseLifecycleManagementService(dbManagementService, dbConfigurator, mockCloudOdsDatabaseSqlServerSecurityConfiguration.Object, mockRawSqlConnectionService.Object, mockCloudOdsDatabaseNameProvider.Object);
            var sqlConfiguration = GetSqlConfiguration();

            var productionLifecycleManagementService = new AzureProductionLifecycleManagementService(dbLifecycleManagementService);
            var result = await productionLifecycleManagementService.ResetToMinimal(sqlConfiguration, CancellationToken.None);
            result.Error.ShouldBe(false);
        }

        private OdsSqlConfiguration GetSqlConfiguration()
        {
            var connectionString = ConfigurationHelper.GetConnectionStrings().AzureSql;
            var csb = new SqlConnectionStringBuilder(connectionString);

            return new OdsSqlConfiguration
            {
                AdminAppCredentials = new OdsSqlCredential { UserName = "EdFiOdsAdminApp", Password = "" },
                AdminCredentials = new OdsSqlAdminCredential { UserName = csb.UserID, Password = csb.Password },
                HostName = csb.DataSource,
                ProductionApiCredentials = new OdsSqlCredential { UserName = "EdFiOdsProductionApi", Password = "" }
            };
        }

        #region Configurations
        private IEnumerable<CloudOdsDatabaseSecurityConfiguration> GetSetupConfiguration(OdsSqlConfiguration sqlConfiguration)
        {
            return new[]
            {
                new CloudOdsDatabaseSecurityConfiguration
                {
                    TargetDatabase = CloudOdsDatabases.Admin,
                    Users = new[]
                    {
                        new CloudOdsDatabaseUser
                        {
                            UserCredential = sqlConfiguration.AdminAppCredentials,
                            Roles = new[] { CloudOdsSqlServerRoles.DatabaseOwner }
                        }
                    }
                },
                new CloudOdsDatabaseSecurityConfiguration
                {
                    TargetDatabase = CloudOdsDatabases.Security,
                    Users = new[]
                    {
                        new CloudOdsDatabaseUser
                        {
                            UserCredential = sqlConfiguration.AdminAppCredentials,
                            Roles = new[] { CloudOdsSqlServerRoles.DatabaseOwner }
                        }
                    }
                }
            };
        }
        
        #endregion
    }
}
