// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data.SqlClient;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;
using NUnit.Framework;
using Shouldly;
using Action = System.Action;

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

        [Test]
        public void ShouldTrackSingleCurrentSchoolYear()
        {
            GetNewSchoolYear(2000);
            GetNewSchoolYear(1999);
            GetNewSchoolYear(2001);

            SetSchoolYear(1999);
            GetSchoolYears().ShouldSatisfy(
                x => x.ShouldBeSchoolYear(1999, isCurrent: true),
                x => x.ShouldBeSchoolYear(2000),
                x => x.ShouldBeSchoolYear(2001));

            SetSchoolYear(2000);
            GetSchoolYears().ShouldSatisfy(
                x => x.ShouldBeSchoolYear(1999),
                x => x.ShouldBeSchoolYear(2000, isCurrent: true),
                x => x.ShouldBeSchoolYear(2001));

            SetSchoolYear(2001);
            GetSchoolYears().ShouldSatisfy(
                x => x.ShouldBeSchoolYear(1999),
                x => x.ShouldBeSchoolYear(2000),
                x => x.ShouldBeSchoolYear(2001, isCurrent: true));

            Action invalidYear = () => SetSchoolYear(-1234);
            invalidYear.ShouldThrow<SqlException>().Message.ShouldBe("Specified school year does not exist.");
        }

        private static IReadOnlyList<SchoolYearType> GetSchoolYears()
            => new GetSchoolYearsQuery(TestConnectionProvider)
                .Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.SharedInstance);

        private static void SetSchoolYear(short schoolYear)
            => new SetSchoolYearCommand(TestConnectionProvider)
                .Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.SharedInstance, schoolYear);
    }
}
