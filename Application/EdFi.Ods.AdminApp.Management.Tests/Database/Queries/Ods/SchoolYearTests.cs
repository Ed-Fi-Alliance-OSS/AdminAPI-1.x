// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

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

            var result = new GetSchoolYearsQuery(TestConnectionProvider)
                .Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.SharedInstance);

            result.Count.ShouldBe(3);

            result[0].SchoolYear.ShouldBe((short)1999);
            result[0].SchoolYearDescription.ShouldBe("1998-1999");
            result[0].CurrentSchoolYear.ShouldBe(false);

            result[1].SchoolYear.ShouldBe((short)2000);
            result[1].SchoolYearDescription.ShouldBe("1999-2000");
            result[1].CurrentSchoolYear.ShouldBe(false);

            result[2].SchoolYear.ShouldBe((short)2001);
            result[2].SchoolYearDescription.ShouldBe("2000-2001");
            result[2].CurrentSchoolYear.ShouldBe(false);
        }
    }
}
