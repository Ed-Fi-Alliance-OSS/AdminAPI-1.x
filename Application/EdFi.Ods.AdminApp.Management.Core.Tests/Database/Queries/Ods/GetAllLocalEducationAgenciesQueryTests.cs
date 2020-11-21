// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Ods;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    [TestFixture]
    public class GetAllLocalEducationAgenciesQueryTests : OdsDataTestBase
    {
        [Test]
        public void ShouldRetrieveLocalEducationAgencies()
        {
            var id = GetNewLocalEducationAgencyId("Sample LEA");
            var districts = new GetAllLocalEducationAgenciesQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox);
            districts.Single().Name.ShouldBe("Sample LEA");
            districts.Single().LocalEducationAgencyId.ShouldBe(id);
        }
    }
}
