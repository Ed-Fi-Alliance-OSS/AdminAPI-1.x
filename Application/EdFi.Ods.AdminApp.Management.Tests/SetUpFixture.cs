// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.DbConfigurations;
using NUnit.Framework;
using System.Data.Entity;
using EdFi.Common.Configuration;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            DbConfiguration.SetConfiguration(new DatabaseEngineDbConfiguration(DatabaseEngine.SqlServer));

            new SecurityTestDatabaseSetup().EnsureSecurityV53DatabaseExists(@"C:\\temp");
        }
    }
}
