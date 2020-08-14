// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Database.Setup;

namespace EdFi.Ods.AdminApp.Management.OnPrem
{
    public class OnPremOdsDatabaseSqlServerSecurityConfiguration : ICloudOdsDatabaseSqlServerSecurityConfiguration
    {
        public IEnumerable<CloudOdsDatabaseSecurityConfiguration> GetSetupConfiguration(OdsSqlConfiguration sqlConfiguration)
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
                                CloudOdsSqlServerRoles.DataWriter,
                                CloudOdsSqlServerRoles.Executor,
                            }
                        },
                    }
                }
            };
        }
    }
}