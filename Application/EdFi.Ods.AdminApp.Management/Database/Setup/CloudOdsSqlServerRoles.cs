// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Database.Setup
{
    public sealed class CloudOdsSqlServerRoles : Enumeration<CloudOdsSqlServerRoles>, ISqlRole
    {
        private const string DbReader = "db_datareader";
        private const string DbWriter = "db_datawriter";
        private const string DbExecutor = "db_executor";
        private const string DbOwner = "db_owner";
        private const string DbDdlAdmin = "db_ddladmin";

        public static CloudOdsSqlServerRoles DataReader = new CloudOdsSqlServerRoles(1, DbReader);
        public static CloudOdsSqlServerRoles DataWriter = new CloudOdsSqlServerRoles(2, DbWriter);
        public static CloudOdsSqlServerRoles Executor = new CloudOdsSqlServerRoles(3, DbExecutor, "EXECUTE");
        public static CloudOdsSqlServerRoles DatabaseOwner = new CloudOdsSqlServerRoles(4, DbOwner);
        public static CloudOdsSqlServerRoles DatabaseDdlAdmin = new CloudOdsSqlServerRoles(5, DbDdlAdmin, "create table, create view");

        public string CreateSql => IsBuiltinRole
            ? null
            : $@"IF DATABASE_PRINCIPAL_ID('{DisplayName}') IS NULL 
BEGIN 
    CREATE ROLE [{DisplayName}] 
    GRANT {RolePrivileges} TO [{DisplayName}]
END";

        public bool IsBuiltinRole { get; }
        public string RolePrivileges { get; set; }
        public string RoleName => DisplayName;

        private CloudOdsSqlServerRoles(int value, string displayName) : base(value, displayName)
        {
            IsBuiltinRole = true;
            RolePrivileges = null;
        }

        private CloudOdsSqlServerRoles(int value, string displayName, string rolePrivileges) : base(value, displayName)
        {
            IsBuiltinRole = false;
            RolePrivileges = rolePrivileges;
        }
    }
}