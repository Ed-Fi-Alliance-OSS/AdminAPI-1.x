// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Aws.Tests
{
    [TestFixture]
    public class WhenGettingDatabaseName
    {
        [TestCaseSource("TestCases")]
        public void GivenValidDatabase_ThenReturnExpectedValue(CloudOdsDatabases database, string expected)
        {
            new AwsCloudOdsDatabaseNameProvider().GetDatabaseName(database)
                .ShouldBe(expected);
        }

        static object[] TestCases =
        {
            new object[] {CloudOdsDatabases.Admin, "EdFi_Admin"},
            new object[] {CloudOdsDatabases.Security, "EdFi_Security"},
            new object[] {CloudOdsDatabases.ProductionOds, "EdFi_Ods"},
            new object[] {CloudOdsDatabases.Master, "master"},
            new object[] {CloudOdsDatabases.MinimalTemplate, "EdFi_Ods_Minimal_Template"},
            new object[] {CloudOdsDatabases.PopulatedTemplate, "EdFi_Ods_Populated_Template" }
        };
    }
}
