// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Setup;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Azure.IntegrationTests
{
    [Ignore("These tests are intended to be marked [Explicit], but a " +
            "VS2019 Test Explorer bug prevents that attribute from " +
            "functioning as intended. To run these tests, temporarily" +
            "comment out this [Ignore(...)] attribute.")]
    [TestFixture]
    public class AzureSqlSecurityConfiguratorTester
    {
        private const string DbReader = "db_datareader";
        private const string DbWriter = "db_datawriter";
        private const string DbExecutor = "db_executor";
        private const string DbOwner = "db_owner";

        internal class ExpectedDatabaseSetup
        {
            public string DatabaseName { get; set; }
            public Dictionary<string, string[]> UserRoles { get; set; } 

            public string[] Users => UserRoles.Keys.ToArray();
        }

        internal class ExpectedSqlServerSetup
        {
            public List<ExpectedDatabaseSetup> ExpectedDatabaseSetups { get; set; }

            public string[] Logins
            {
                get { return ExpectedDatabaseSetups.SelectMany(s => s.Users).Distinct().ToArray(); }
            }
        }

        private readonly ExpectedSqlServerSetup _expectedSqlServerSetup = new ExpectedSqlServerSetup
        {
            ExpectedDatabaseSetups = new List<ExpectedDatabaseSetup>
            {
                new ExpectedDatabaseSetup
                {
                    DatabaseName = "EdFi_Admin",
                    UserRoles = new Dictionary<string, string[]>
                    {
                        {"AdminApp_test's", new[] {DbReader, DbWriter}},
                        {"Prod_test's", new[] {DbReader, DbWriter, DbExecutor}}
                        
                    }
                },
                new ExpectedDatabaseSetup
                {
                    DatabaseName = "EdFi_Security",
                    UserRoles = new Dictionary<string, string[]>
                    {
                        {"AdminApp_test's", new[] {DbReader, DbWriter}},
                        {"Prod_test's", new[] {DbReader, DbWriter}}
                    }
                },
                new ExpectedDatabaseSetup
                {
                    DatabaseName = "EdFi_Ods_Production",
                    UserRoles = new Dictionary<string, string[]>
                    {
                        {"AdminApp_test's", new[] {DbReader, DbWriter}},
                        {"Prod_test's", new[] {DbReader, DbWriter}},
                    }
                }
            }
        };

        private string MasterConnectionString
        {
            get
            {
                /*
                    Run these tests only if you understand the full consequences.

                    SQL Server Authentication must be enabled when using a local SQL Server
                    for these tests. Ensure a "IntegrationTests" user account has been created
                    and given the sysadmin role. Set the password in this connection string
                    and return it.

                    DO NOT CHECK THESE VALUES INTO SOURCE CONTROL
                 */

                throw new NotImplementedException("Temporarily uncomment the following line when you need to run this test. Your master database will be affected.");

                //return "Server=.\\;Database=master;User Id=IntegrationTests;Password=IntegrationTests_User_Password_Here";
            }
        }

        private SqlConnectionStringBuilder MasterSqlConnectionStringBuilder => new SqlConnectionStringBuilder(MasterConnectionString);

        private OdsSqlConfiguration GetOdsSqlConfiguration()
        {
            return new OdsSqlConfiguration
            {
                AdminAppCredentials = new OdsSqlCredential
                {
                    Password = "3klja([]')*UDSq3",
                    UserName = "AdminApp_test's"
                },
                AdminCredentials = new OdsSqlAdminCredential
                {
                    Password = MasterSqlConnectionStringBuilder.Password,
                    UserName = MasterSqlConnectionStringBuilder.UserID
                },
                HostName = MasterSqlConnectionStringBuilder.DataSource,
                ProductionApiCredentials = new OdsSqlCredential
                {
                    Password = "3klja([]')*UDSq3",
                    UserName = "Prod_test's"
                }
            };
        }

        private string GetConnectionStringForDatabase(string databaseName)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(MasterConnectionString)
            {
                InitialCatalog = databaseName
            };

            return connectionStringBuilder.ConnectionString;
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

        public IEnumerable<CloudOdsDatabaseSecurityConfiguration> GetRuntimeConfiguration(OdsSqlConfiguration sqlConfiguration)
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
                            Roles = new[]
                            {
                                CloudOdsSqlServerRoles.DataReader,
                                CloudOdsSqlServerRoles.DataWriter
                            }
                        },
                        new CloudOdsDatabaseUser
                        {
                            UserCredential = sqlConfiguration.ProductionApiCredentials,
                            Roles = new[]
                            {
                                CloudOdsSqlServerRoles.DataReader,
                                CloudOdsSqlServerRoles.DataWriter,
                                CloudOdsSqlServerRoles.Executor
                            }
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
                            Roles = new[]
                            {
                                CloudOdsSqlServerRoles.DataReader,
                                CloudOdsSqlServerRoles.DataWriter
                            }
                        },
                        new CloudOdsDatabaseUser
                        {
                            UserCredential = sqlConfiguration.ProductionApiCredentials,
                            Roles = new[]
                            {
                                CloudOdsSqlServerRoles.DataReader,
                                CloudOdsSqlServerRoles.DataWriter
                            }
                        }
                    }
                },
                new CloudOdsDatabaseSecurityConfiguration
                {
                    TargetDatabase = CloudOdsDatabases.ProductionOds,
                    Users = new[]
                    {
                        new CloudOdsDatabaseUser
                        {
                            UserCredential = sqlConfiguration.AdminAppCredentials,
                            Roles = new[]
                            {
                                CloudOdsSqlServerRoles.DataReader,
                                CloudOdsSqlServerRoles.DataWriter
                            }
                        },
                        new CloudOdsDatabaseUser
                        {
                            UserCredential = sqlConfiguration.ProductionApiCredentials,
                            Roles = new[]
                            {
                                CloudOdsSqlServerRoles.DataReader,
                                CloudOdsSqlServerRoles.DataWriter
                            }
                        },
                    }
                }
            };
        }
        #endregion


        [SetUp]
        public void Setup()
        {
            RemoveExistingLogins();

            var mockRawSqlConnectionService = new Mock<IRawSqlConnectionService>();
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            var _sqlConfigurator = new AzureSqlSecurityConfigurator(mockRawSqlConnectionService.Object, mockCloudOdsDatabaseNameProvider.Object);

            var config = GetOdsSqlConfiguration();
            _sqlConfigurator.CreateServerLogins(config);

            var setupConfiguration = GetSetupConfiguration(config);
            var runtimeConfiguration = GetRuntimeConfiguration(config);

            _sqlConfigurator.ApplyConfiguration(config, setupConfiguration);
            _sqlConfigurator.ApplyConfiguration(config, runtimeConfiguration);

            
        }

        [TearDown]
        public void Teardown()
        {
            RemoveExistingLogins();
        }

        private void RemoveExistingLogins()
        {
            foreach (var database in _expectedSqlServerSetup.ExpectedDatabaseSetups)
            {
                using (var connection = new SqlConnection(GetConnectionStringForDatabase(database.DatabaseName)))
                {
                    connection.Open();
                    foreach (var user in database.Users)
                    {
                        var quoteEscapedUsername = user.Replace("'", "''");
                        var sql = $"IF EXISTS (SELECT name FROM sys.database_principals WHERE name = '{quoteEscapedUsername}') BEGIN DROP USER [{user}] END";
                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }


            }

            using (var connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();
                foreach (var login in _expectedSqlServerSetup.Logins)
                {
                    var quoteEscapedLogin = login.Replace("'", "''");
                    var sql = $"IF EXISTS (SELECT name FROM sys.sql_logins WHERE name='{quoteEscapedLogin}') BEGIN DROP LOGIN [{login}] END";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        [Test]
        public void ShouldCreateServerLogins()
        {
            using (var connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();

                var sql = "SELECT name FROM sys.server_principals WHERE type = 'S'";
                using (var command = new SqlCommand(sql, connection))
                {
                    var loginsActuallyOnDatabaseServer = new List<string>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            loginsActuallyOnDatabaseServer.Add(reader[0].ToString());
                        }
                    }

                    _expectedSqlServerSetup.Logins.All(x => loginsActuallyOnDatabaseServer.Contains(x)).ShouldBeTrue();
                }
            }
        }

        [Test]
        public void ShouldCreateUsersInDatabases()
        {
            foreach (var expectedDatabaseSetup in _expectedSqlServerSetup.ExpectedDatabaseSetups)
            {
                using (var connection = new SqlConnection(GetConnectionStringForDatabase(expectedDatabaseSetup.DatabaseName)))
                {
                    connection.Open();

                    var sql = "SELECT name FROM sys.database_principals WHERE type = 'S'";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        var usersActuallyInDatabase = new List<string>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usersActuallyInDatabase.Add(reader[0].ToString());
                            }
                        }

                        expectedDatabaseSetup.Users.All(x => usersActuallyInDatabase.Contains(x)).ShouldBeTrue($"Failed at {expectedDatabaseSetup.DatabaseName}");
                    }
                }
            }
        }

        [Test]
        public void ShouldAddUsersInDatabaseRoles()
        {
            foreach (var expectedDatabaseSetup in _expectedSqlServerSetup.ExpectedDatabaseSetups)
            {
                using (var connection = new SqlConnection(GetConnectionStringForDatabase(expectedDatabaseSetup.DatabaseName)))
                {
                    connection.Open();

                    var sql = @"SELECT us.name AS Username, dp.name AS Role
FROM sys.sysusers us right
JOIN  sys.database_role_members rm ON us.uid = rm.member_principal_id
JOIN sys.database_principals dp ON rm.role_principal_id = dp.principal_id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        var userRolesActuallyInDatabase = new Dictionary<string, List<string>>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userName = reader["Username"].ToString();
                                var role = reader["Role"].ToString();

                                if (!userRolesActuallyInDatabase.ContainsKey(userName))
                                {
                                    userRolesActuallyInDatabase[userName] = new List<string>();
                                }

                                userRolesActuallyInDatabase[userName].Add(role);
                            }
                        }


                        expectedDatabaseSetup.UserRoles.All(expected =>
                                userRolesActuallyInDatabase.ContainsKey(expected.Key) &&
                                expected.Value.All(expectedRole => userRolesActuallyInDatabase[expected.Key].Contains(expectedRole))
                            ).ShouldBeTrue($"Failed at {expectedDatabaseSetup.DatabaseName}");
                    }
                }
            }
        }

        [Test]
        public void ShouldNotLeaveAdminAppUserInDbOwnerRoleAfterRemovalOfSetupConfiguration()
        {
            var mockRawSqlConnectionService = new Mock<IRawSqlConnectionService>();
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            var sqlConfigurator = new AzureSqlSecurityConfigurator(mockRawSqlConnectionService.Object, mockCloudOdsDatabaseNameProvider.Object);

            var sqlConfig = GetOdsSqlConfiguration();
            var setupSecurityConfiguration = GetSetupConfiguration(sqlConfig);

            sqlConfigurator.RemoveConfiguration(sqlConfig, setupSecurityConfiguration);

            foreach (var expectedDatabaseSetup in _expectedSqlServerSetup.ExpectedDatabaseSetups)
            {
                using (var connection = new SqlConnection(GetConnectionStringForDatabase(expectedDatabaseSetup.DatabaseName)))
                {
                    connection.Open();

                    var sql = @"SELECT us.name AS Username, dp.name AS Role
FROM sys.sysusers us right
JOIN  sys.database_role_members rm ON us.uid = rm.member_principal_id
JOIN sys.database_principals dp ON rm.role_principal_id = dp.principal_id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        var userRolesActuallyInDatabase = new Dictionary<string, List<string>>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userName = reader["Username"].ToString();
                                var role = reader["Role"].ToString();

                                if (!userRolesActuallyInDatabase.ContainsKey(userName))
                                {
                                    userRolesActuallyInDatabase[userName] = new List<string>();
                                }

                                userRolesActuallyInDatabase[userName].Add(role);
                            }
                        }

                        userRolesActuallyInDatabase.Any(r => r.Key == sqlConfig.AdminAppCredentials.UserName && r.Value.Contains(DbOwner)).ShouldBeFalse();
                    }
                }
            }
        }

        [Test]
        public void ShouldNotErrorOnSecondRun()
        {
            var mockRawSqlConnectionService = new Mock<IRawSqlConnectionService>();
            var mockCloudOdsDatabaseNameProvider = new Mock<ICloudOdsDatabaseNameProvider>();
            var sqlConfigurator = new AzureSqlSecurityConfigurator(mockRawSqlConnectionService.Object, mockCloudOdsDatabaseNameProvider.Object);


            var config = GetOdsSqlConfiguration();
            sqlConfigurator.CreateServerLogins(config);
            
            var runtimeConfiguration = GetRuntimeConfiguration(config);

            sqlConfigurator.ApplyConfiguration(config, runtimeConfiguration);
        }
    }
}
