// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management
{
    public sealed class CloudOdsDatabases : Enumeration<CloudOdsDatabases>
    {
        public string DatabaseName { get; }

        public static CloudOdsDatabases Admin = new CloudOdsDatabases(1, "Admin", CloudOdsDatabaseNames.Admin);
        public static CloudOdsDatabases Security = new CloudOdsDatabases(2, "Security", CloudOdsDatabaseNames.Security);
        public static CloudOdsDatabases ProductionOds = new CloudOdsDatabases(3, "Production ODS", CloudOdsDatabaseNames.ProductionOds);
        public static CloudOdsDatabases Master = new CloudOdsDatabases(5, "Master", CloudOdsDatabaseNames.Master);
        public static CloudOdsDatabases MinimalTemplate = new CloudOdsDatabases(6, "Minimal Template", CloudOdsDatabaseNames.MinimalTemplate);
        public static CloudOdsDatabases PopulatedTemplate = new CloudOdsDatabases(7, "Populated Template", CloudOdsDatabaseNames.PopulatedTemplate);
        
        private CloudOdsDatabases(int value, string displayName, string databaseName) : base(value, displayName)
        {
            DatabaseName = databaseName;
        }
    }
}