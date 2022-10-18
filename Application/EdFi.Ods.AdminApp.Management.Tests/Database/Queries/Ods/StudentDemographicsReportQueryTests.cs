// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Globalization;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    [TestFixture]
    public class StudentDemographicsReportQueryTests : OdsDataTestBase
    {
        [Test]
        public void Should_report_gender_data()
        {
            var gender1Id = GetNewSexDescriptorId("gender 1");
            var gender2Id = GetNewSexDescriptorId("gender 2");
            var studentWithGender1 = GetNewStudentWithSchoolAssociation(null, "Gender 1", gender1Id);
            var schoolId = studentWithGender1.SchoolId;
            var localEducationAgencyId = studentWithGender1.LocalEducationAgencyId;

            var studentWithGender2 = GetNewStudentWithSchoolAssociation(schoolId, "First student with gender 2", gender2Id);

            var anotherStudentWithGender2 = GetNewStudentWithSchoolAssociation(schoolId, "Second Student with gender 2", gender2Id);

            var report = new StudentEnrollmentByGenderQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.GenderRepresentation.Count.ShouldBe(2);
            report.GenderRepresentation.Select(x => x.GetDisplayName()).ShouldContain("gender 1");
            report.GenderRepresentation.Select(x => x.GetDisplayName()).ShouldContain("gender 2");
            report.GenderRepresentation.First(x => x.GetDisplayName() == "gender 1").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("33.33 %");
            report.GenderRepresentation.First(x => x.GetDisplayName() == "gender 2").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("66.67 %");
        }

        [Test]
        public void Should_report_race_data()
        {
            var race1 = GetNewRaceDescriptorId("race 1");
            var race2 = GetNewRaceDescriptorId("race 2");

            var studentWithRace1 = GetNewStudentWithSchoolAssociation(null, "Student with race 1");
            var schoolId = studentWithRace1.SchoolId;
            var localEducationAgencyId = studentWithRace1.LocalEducationAgencyId;
            SetStudentRace(studentWithRace1.StudentUsi, localEducationAgencyId, race1);

            var studentWithRace2 = GetNewStudentWithSchoolAssociation(schoolId, "First student with race 2");
            SetStudentRace(studentWithRace2.StudentUsi, localEducationAgencyId, race2);

            var anotherStudentWithRace2 = GetNewStudentWithSchoolAssociation(schoolId, "Second student with race 2");
            SetStudentRace(anotherStudentWithRace2.StudentUsi, localEducationAgencyId, race2);

            var report = new StudentEnrollmentByRaceQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);

            report.RaceRepresentation.Count.ShouldBe(2);
            report.RaceRepresentation.Select(x => x.GetDisplayName()).ShouldContain("race 1");
            report.RaceRepresentation.Select(x => x.GetDisplayName()).ShouldContain("race 2");
            report.RaceRepresentation.First(x => x.GetDisplayName() == "race 1").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("33.33 %");
            report.RaceRepresentation.First(x => x.GetDisplayName() == "race 2").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("66.67 %");
        }

        [Test]
        public void Should_report_ethnicity_data()
        {
            var student1 = GetNewStudentWithSchoolAssociation(null, "Student not of hispanic/latino origin");
            var schoolId = student1.SchoolId;
            var localEducationAgencyId = student1.LocalEducationAgencyId;

            var student2 = GetNewStudentWithSchoolAssociation(schoolId, "Hispanic/Latino student 1", isHispanicLatino: true);
            var student3 = GetNewStudentWithSchoolAssociation(schoolId, "Hispanic/Latino student 2", isHispanicLatino: true);

            var report = new StudentEnrollmentByEthnicityQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.HispanicLatinoPercent.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("66.67 %");
        }

        [Test]
        public void Should_Report_Data_For_Specified_EdOrg()
        {
            var gender1Id = GetNewSexDescriptorId("gender 1");
            var gender2Id = GetNewSexDescriptorId("gender 2");
            var naGender = GetNewSexDescriptorId("n/a");

            var studentWithGender1 = GetNewStudentWithSchoolAssociation(null, "Gender 1", gender1Id);
            var schoolId = studentWithGender1.SchoolId;
            var localEducationAgencyId = studentWithGender1.LocalEducationAgencyId;

            var studentWithGender2 = GetNewStudentWithSchoolAssociation(schoolId, "First student with gender 2", gender2Id);
            var anotherStudentWithGender2 = GetNewStudentWithSchoolAssociation(schoolId, "Second Student with gender 2", gender2Id);

            var race1 = GetNewRaceDescriptorId("race 1");
            var race2 = GetNewRaceDescriptorId("race 2");

            var studentWithRace1 = GetNewStudentWithSchoolAssociation(schoolId, "Student with race 1", naGender);
            SetStudentRace(studentWithRace1.StudentUsi, localEducationAgencyId, race1);

            var studentWithRace2 = GetNewStudentWithSchoolAssociation(schoolId, "First student with race 2", naGender);
            SetStudentRace(studentWithRace2.StudentUsi, localEducationAgencyId, race2);

            var anotherStudentWithRace2 = GetNewStudentWithSchoolAssociation(schoolId, "Second student with race 2", naGender);
            SetStudentRace(anotherStudentWithRace2.StudentUsi, localEducationAgencyId, race2);

            var latinoStudent = GetNewStudentWithSchoolAssociation(schoolId, "Hispanic/Latino student 1", sexDescriptorId: naGender, isHispanicLatino: true);
            var latinoStudent2 = GetNewStudentWithSchoolAssociation(schoolId, "Hispanic/Latino student 2", sexDescriptorId: naGender, isHispanicLatino: true);

            var studentAtAnotherEducationAgency = GetNewStudentWithSchoolAssociation();
            var otherEducationAgencyId = studentAtAnotherEducationAgency.LocalEducationAgencyId;
            otherEducationAgencyId.ShouldNotBe(localEducationAgencyId);

            var genderReport = new StudentEnrollmentByGenderQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            var raceReport = new StudentEnrollmentByRaceQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            var ethnicityReport = new StudentEnrollmentByEthnicityQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);

            genderReport.GenderRepresentation.Count.ShouldBe(3);
            genderReport.GenderRepresentation.Select(x => x.GetDisplayName()).ShouldContain("gender 1");
            genderReport.GenderRepresentation.Select(x => x.GetDisplayName()).ShouldContain("gender 2");
            genderReport.GenderRepresentation.Select(x => x.GetDisplayName()).ShouldContain("n/a");
            genderReport.GenderRepresentation.First(x => x.GetDisplayName() == "gender 1").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("12.50 %");
            genderReport.GenderRepresentation.First(x => x.GetDisplayName() == "gender 2").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("25.00 %");
            genderReport.GenderRepresentation.First(x => x.GetDisplayName() == "n/a").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("62.50 %");

            raceReport.RaceRepresentation.Count.ShouldBe(3);
            raceReport.RaceRepresentation.Select(x => x.GetDisplayName()).ShouldContain("race 1");
            raceReport.RaceRepresentation.Select(x => x.GetDisplayName()).ShouldContain("race 2");
            raceReport.RaceRepresentation.Select(x => x.GetDisplayName()).ShouldContain("No Data");
            raceReport.RaceRepresentation.First(x => x.GetDisplayName() == "race 1").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("12.50 %");
            raceReport.RaceRepresentation.First(x => x.GetDisplayName() == "race 2").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("25.00 %");
            raceReport.RaceRepresentation.First(x => x.GetDisplayName() == "No Data").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("62.50 %");

            ethnicityReport.HispanicLatinoPercent.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("25.00 %");

            var otherGenderReport = new StudentEnrollmentByGenderQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, otherEducationAgencyId);
            var otherRaceReport = new StudentEnrollmentByRaceQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, otherEducationAgencyId);
            otherGenderReport.GenderRepresentation.Count.ShouldBe(1);
            otherRaceReport.RaceRepresentation.Count.ShouldBe(1);
        }

        [Test]
        public void Should_only_report_active_students()
        {
            var race = GetNewRaceDescriptorId("White");
            var gender = GetNewSexDescriptorId("Male");
            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(null, null, gender, true);
            var schoolId = inactiveStudentThatShouldNotAppearInReport.SchoolId;
            var leaId = inactiveStudentThatShouldNotAppearInReport.LocalEducationAgencyId;
            SetStudentRace(inactiveStudentThatShouldNotAppearInReport.StudentUsi, leaId, race);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);

            var activeStudent = GetNewStudentWithSchoolAssociation(schoolId, null, gender, true);
            SetStudentRace(activeStudent.StudentUsi, leaId, race);

            var report = new StudentEnrollmentByRaceQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, leaId);
            report.TotalStudentCount.ShouldBe(1);
        }
    }
}
