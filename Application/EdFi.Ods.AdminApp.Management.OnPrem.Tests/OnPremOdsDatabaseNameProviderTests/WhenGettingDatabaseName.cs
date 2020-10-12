// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.OnPrem.Tests.OnPremOdsDatabaseNameProviderTests
{
    [TestFixture]
    public class WhenGettingDatabaseName
    {
        [Test]
        public void GivenNullArgument_ThenThrowArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => new OnPremOdsDatabaseNameProvider().GetDatabaseName(null));
        }


        [TestCaseSource("TestCases")]
        public void GivenValidOdsDatabase_ThenReturnSpecifiedValue(CloudOdsDatabases database, string expected)
        {
            new OnPremOdsDatabaseNameProvider()
                .GetDatabaseName(database)
                .ShouldBe(expected);
        }

        static object[] TestCases =
        {
            // The value "ADMIN" is provided in the test project's app.config file
            new object[] {CloudOdsDatabases.Admin, "EdFi_Admin_Test"},
            // The value "ODS" is provided in the test project's app.config file
            new object[] { CloudOdsDatabases.ProductionOds, "EdFi_Ods_Empty_Test" },
            // The value "SECURITY" is provided in the test project's app.config file
            new object[] {CloudOdsDatabases.Security, "EdFi_Security_Test"},
            new object[] {CloudOdsDatabases.Master, "master"},
            new object[] {CloudOdsDatabases.MinimalTemplate, "EdFi_Ods_Minimal_Template"},
            new object[] {CloudOdsDatabases.PopulatedTemplate, "EdFi_Ods_Populated_Template" },
        };
    }


}
