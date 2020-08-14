// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureSqlDatabasePerformanceLevel : AzurePerformanceLevel
    {
        public AzureSqlDatabasePerformanceLevel(string edition, string serviceObjective) : base(edition, serviceObjective)
        {
        }

        public static readonly AzureSqlDatabasePerformanceLevel Basic = new AzureSqlDatabasePerformanceLevel("Basic", "Basic");

        public static readonly AzureSqlDatabasePerformanceLevel S0 = new AzureSqlDatabasePerformanceLevel("Standard", "S0");
        public static readonly AzureSqlDatabasePerformanceLevel S1 = new AzureSqlDatabasePerformanceLevel("Standard", "S1");
        public static readonly AzureSqlDatabasePerformanceLevel S2 = new AzureSqlDatabasePerformanceLevel("Standard", "S2");
        public static readonly AzureSqlDatabasePerformanceLevel S3 = new AzureSqlDatabasePerformanceLevel("Standard", "S3");

        public static readonly AzureSqlDatabasePerformanceLevel P1 = new AzureSqlDatabasePerformanceLevel("Premium", "P1");
        public static readonly AzureSqlDatabasePerformanceLevel P2 = new AzureSqlDatabasePerformanceLevel("Premium", "P2");
        public static readonly AzureSqlDatabasePerformanceLevel P4 = new AzureSqlDatabasePerformanceLevel("Premium", "P4");
        public static readonly AzureSqlDatabasePerformanceLevel P6 = new AzureSqlDatabasePerformanceLevel("Premium", "P6");
        public static readonly AzureSqlDatabasePerformanceLevel P11 = new AzureSqlDatabasePerformanceLevel("Premium", "P11");
        public static readonly AzureSqlDatabasePerformanceLevel P15 = new AzureSqlDatabasePerformanceLevel("Premium", "P15");
    }
}
