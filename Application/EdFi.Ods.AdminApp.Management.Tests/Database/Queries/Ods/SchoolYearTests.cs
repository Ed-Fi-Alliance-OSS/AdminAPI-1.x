// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    public class SchoolYearTests : OdsDataTestBase
    {
        [Test]
        public void ShouldGetOrderedSchoolYearTypes()
        {
            GetNewSchoolYear(2000);
            GetNewSchoolYear(1999);
            GetNewSchoolYear(2001);

            var result = GetSchoolYears();

            result.ShouldSatisfy(
                x => x.ShouldBeSchoolYear(1999),
                x => x.ShouldBeSchoolYear(2000),
                x => x.ShouldBeSchoolYear(2001));
        }

        private static IReadOnlyList<SchoolYearType> GetSchoolYears()
            => new GetSchoolYearsQuery(TestConnectionProvider)
                .Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.SharedInstance);
    }
}
