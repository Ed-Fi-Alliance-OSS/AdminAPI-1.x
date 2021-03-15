// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using Dapper;
using EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears;
using EdFi.Ods.AdminApp.Management.Instances;
using NUnit.Framework;
using Shouldly;
using Action = System.Action;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    public class SchoolYearTests : OdsDataTestBase
    {
        private const string InstanceName = CloudOdsDatabaseNames.ProductionOds;
        private static readonly ApiMode ApiMode = ApiMode.SharedInstance;

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
            invalidYear.ShouldThrow<Exception>().Message.ShouldBe("School year -1234 does not exist.");
        }

        [Test]
        public void ShouldGetCurrentSchoolYearWhenDefined()
        {
            GetCurrentSchoolYear().ShouldBe(null);

            GetNewSchoolYear(2000);
            GetNewSchoolYear(1999);
            GetNewSchoolYear(2001);

            GetCurrentSchoolYear().ShouldBe(null);

            SetSchoolYear(1999);
            GetCurrentSchoolYear().ShouldBeSchoolYear(1999, isCurrent: true);

            SetSchoolYear(2000);
            GetCurrentSchoolYear().ShouldBeSchoolYear(2000, isCurrent: true);

            SetSchoolYear(2001);
            GetCurrentSchoolYear().ShouldBeSchoolYear(2001, isCurrent: true);
        }

        [Test]
        public void ShouldGetNoCurrentSchoolYearWhenAmbiguous()
        {
            // Under normal circumstances, 1 or 0 rows in the edfi.SchoolYearType
            // table will be marked as current. However, we should not use a
            // strict SingleOrDefault behavior when fetching the current year.
            // If we did, users would experience an exception when we would
            // rather give them the opportunity to fix their year setting.
            // So, when the table is ambiguous, we expect null, the same
            // as when there are zero records marked as current.

            GetNewSchoolYear(2000);
            GetNewSchoolYear(1999);
            GetNewSchoolYear(2001);
            GetCurrentSchoolYear().ShouldBe(null);

            // Create an ambiguous, meaningless selection of multiple years.
            using (var connection = TestConnectionProvider.CreateNewConnection(InstanceName, ApiMode))
                connection.Execute(@"UPDATE edfi.SchoolYearType SET CurrentSchoolYear='true'");

            // Rather than throwing, the user should experience this as no valid selection.
            GetCurrentSchoolYear().ShouldBe(null);

            // Users can correct the problem by selecting a year.
            SetSchoolYear(2000);
            GetCurrentSchoolYear().ShouldBeSchoolYear(2000, isCurrent: true);
        }

        private static SchoolYearType GetCurrentSchoolYear()
            => new GetCurrentSchoolYearQuery(TestConnectionProvider)
                .Execute(InstanceName, ApiMode);

        private static IReadOnlyList<SchoolYearType> GetSchoolYears()
            => new GetSchoolYearsQuery(TestConnectionProvider)
                .Execute(InstanceName, ApiMode);

        private static void SetSchoolYear(short schoolYear)
            => new SetCurrentSchoolYearCommand(TestConnectionProvider)
                .Execute(InstanceName, ApiMode, schoolYear);
    }
}
