// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Setup;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureSqlSecurityConfigurator : IAzureSqlSecurityConfigurator
    {
        private readonly IRawSqlConnectionService _rawSqlConnectionService;
        private readonly ICloudOdsDatabaseNameProvider _cloudOdsDatabaseNameProvider;

        public AzureSqlSecurityConfigurator(IRawSqlConnectionService rawSqlConnectionService, ICloudOdsDatabaseNameProvider cloudOdsDatabaseNameProvider)
        {
             _rawSqlConnectionService = rawSqlConnectionService;
            _cloudOdsDatabaseNameProvider = cloudOdsDatabaseNameProvider;
        }

        public void ApplyConfiguration(OdsSqlConfiguration sqlConfiguration, IEnumerable<CloudOdsDatabaseSecurityConfiguration> securityConfigurations)
        {
            foreach (var securityConfiguration in securityConfigurations)
            {
                ApplyConfiguration(sqlConfiguration, securityConfiguration);
            }
        }

        public void ApplyConfiguration(OdsSqlConfiguration sqlConfiguration, CloudOdsDatabaseSecurityConfiguration securityConfiguration)
        {
            var databaseName = _cloudOdsDatabaseNameProvider.GetDatabaseName(securityConfiguration.TargetDatabase);

            RunSqlProcessOnDatabase(databaseName, sqlConfiguration, (connection, configuration) =>
            {
                CreateDatabaseRoles(sqlConfiguration, securityConfiguration);

                foreach (var user in securityConfiguration.Users)
                {
                    AddUserToDatabase(connection, user.UserCredential);
                    AddUserToRole(connection, user.UserCredential, user.Roles.ToArray());
                }
            });
        }
        
        public void RemoveConfiguration(OdsSqlConfiguration sqlConfiguration, IEnumerable<CloudOdsDatabaseSecurityConfiguration> securityConfigurations)
        {
            foreach (var securityConfiguration in securityConfigurations)
            {
                RemoveConfiguration(sqlConfiguration, securityConfiguration);
            }
        }

        public void RemoveConfiguration(OdsSqlConfiguration sqlConfiguration, CloudOdsDatabaseSecurityConfiguration securityConfiguration)
        {
            var databaseName = _cloudOdsDatabaseNameProvider.GetDatabaseName(securityConfiguration.TargetDatabase);

            foreach (var user in securityConfiguration.Users)
            {
                RunSqlProcessOnDatabase(databaseName, sqlConfiguration, (connection, configuration) =>
                {
                    RemoveUserFromRole(connection, configuration.AdminAppCredentials, user.Roles.ToArray());
                });
            }
        }

        public void CreateServerLogins(OdsSqlConfiguration odsSqlConfiguration)
        {
            RunSqlProcessOnDatabase(CloudOdsDatabaseNames.Master, odsSqlConfiguration, (connection, configuration) =>
            {
                CreateLogin(connection, configuration.AdminAppCredentials);
                CreateLogin(connection, configuration.ProductionApiCredentials);
            });
        }

        private void CreateDatabaseRole(OdsSqlConfiguration odsSqlConfiguration, CloudOdsDatabaseSecurityConfiguration securityConfiguration, ISqlRole role)
        {
            var databaseName = _cloudOdsDatabaseNameProvider.GetDatabaseName(securityConfiguration.TargetDatabase);

            if (!role.IsBuiltinRole)
            {
                RunSqlProcessOnDatabase(databaseName, odsSqlConfiguration, (connection, configuration) =>
                {
                    _rawSqlConnectionService.ExecuteDdl(connection, role.CreateSql);
                });
            }
        }

        private void CreateDatabaseRoles(OdsSqlConfiguration odsSqlConfiguration, CloudOdsDatabaseSecurityConfiguration securityConfiguration)
        {
            foreach (var role in securityConfiguration.Users.SelectMany(u => u.Roles).Distinct())
            {
                CreateDatabaseRole(odsSqlConfiguration, securityConfiguration, role);
            }
        }

        private void RunSqlProcessOnDatabase(string databaseName, OdsSqlConfiguration configuration, Action<SqlConnection, OdsSqlConfiguration> process)
        {
            var connectionString = _rawSqlConnectionService.GetConnectionStringWithAdminCredentials(configuration, databaseName);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                process(connection, configuration);
            }
        }
        
        private void CreateLogin(SqlConnection connection, OdsSqlCredential credential)
        {
            //Note: DDL statements cannot be parameterized, hence the string building and manual quote replacement
            //This process will get data from a trusted user for one-time setup, so the risk is largely mitigated
            var quotedUserName = EscapeSingleQuotes(credential.UserName);
            var password = EscapeSingleQuotes(credential.Password);
            
            var sql = $@"IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name='{quotedUserName}')
BEGIN
    CREATE LOGIN [{credential.UserName}] WITH PASSWORD = '{password}'
END";
            _rawSqlConnectionService.ExecuteDdl(connection, sql);
        }
        
        private void AddUserToDatabase(SqlConnection connection, OdsSqlCredential credential)
        {
            //Note: DDL statements cannot be parameterized, hence the string building and manual quote replacement
            //This process will get data from a trusted user for one-time setup, so the risk is largely mitigated
            var quotedUserName = EscapeSingleQuotes(credential.UserName);
            
            var sql = $@"IF DATABASE_PRINCIPAL_ID('{quotedUserName}') IS NULL 
BEGIN
    CREATE USER [{credential.UserName}] FOR LOGIN [{credential.UserName}] WITH DEFAULT_SCHEMA=[dbo]
END";

            _rawSqlConnectionService.ExecuteDdl(connection, sql);
        }

        private void AddUserToRole(SqlConnection connection, OdsSqlCredential credential, params ISqlRole[] roles)
        {
            foreach (var role in roles)
            {
                var sql = "sp_addrolemember";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@rolename", role.RoleName);
                    command.Parameters.AddWithValue("@membername", credential.UserName);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void RemoveUserFromRole(SqlConnection connection, OdsSqlCredential credential, params ISqlRole[] roles)
        {
            foreach (var role in roles)
            {
                var sql = "sp_droprolemember";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@rolename", role.RoleName);
                    command.Parameters.AddWithValue("@membername", credential.UserName);

                    command.ExecuteNonQuery();
                }
            }
        }

        public string EscapeSingleQuotes(string str)
        {
            return str.Replace("'", "''");
        }
    }
}
