// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Globalization;
using Dapper;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    public class StudentEconomicSituationReportQueryTests : OdsDataTestBase
    {

        [Test]
        public void Should_report_at_risk_count()
        {
            var atRiskStudent = GetNewStudentWithSchoolAssociation();
            var localEducationAgencyId = atRiskStudent.LocalEducationAgencyId;
            var schoolId = atRiskStudent.SchoolId;
            SetStudentRiskStatus(atRiskStudent.StudentUsi, "At Risk", localEducationAgencyId);

            var studentNotAtRisk = GetNewStudentWithSchoolAssociation(schoolId);
            var studentNotAtRisk2 = GetNewStudentWithSchoolAssociation(schoolId);

            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);
            SetStudentRiskStatus(inactiveStudentThatShouldNotAppearInReport.StudentUsi, "At Risk", localEducationAgencyId);

            var report = new StudentEconomicSituationReportQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.TotalStudentCount.ShouldBe(3);
            report.GetStudentEconomicSummary().First(x => x.Description == "At Risk").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("33.33 %");
        }

        [Test]
        public void Should_report_economically_disadvantaged_count()
        {
            var economicDisadvantagedDescriptorId = GetNewDescriptorId("Economic Disadvantaged", "StudentCharacteristic");

            var disadvantagedStudent = GetNewStudentWithSchoolAssociation();
            var localEducationAgencyId = disadvantagedStudent.LocalEducationAgencyId;
            var schoolId = disadvantagedStudent.SchoolId;

            SetStudentCharacteristicDescriptor(disadvantagedStudent.StudentUsi, economicDisadvantagedDescriptorId, localEducationAgencyId);

            var studentNotDisadvantaged = GetNewStudentWithSchoolAssociation(schoolId);
            var studentNotDisadvantaged2 = GetNewStudentWithSchoolAssociation(schoolId);

            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);
            SetStudentCharacteristicDescriptor(inactiveStudentThatShouldNotAppearInReport.StudentUsi, economicDisadvantagedDescriptorId, localEducationAgencyId);

            var report = new StudentEconomicSituationReportQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.TotalStudentCount.ShouldBe(3);
            report.GetStudentEconomicSummary().First(x => x.Description == "Economically Disadvantaged").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("33.33 %");
        }

        [Test]
        public void Should_report_discounted_lunch_count()
        {
            var freeLunchDescriptorId = GetNewDescriptorId("Free", "SchoolFoodServiceProgramService");
            var reducedLunchDescriptorId = GetNewDescriptorId("Reduced Price", "SchoolFoodServiceProgramService");
            var fullPriceSchoolLunchDescriptorId = GetNewDescriptorId("Full Price", "SchoolFoodServiceProgramService");
            var programTypeDescriptorId = GetNewProgramDescriptorId("Food Service");

            var freeSchoolLunchStudent = GetNewStudentWithSchoolAssociation();
            var schoolId = freeSchoolLunchStudent.SchoolId;
            var localEducationAgencyId = freeSchoolLunchStudent.LocalEducationAgencyId;

            CreateProgram(schoolId, programTypeDescriptorId, "Food Service");

            EnrollInFoodServiceProgram(freeSchoolLunchStudent.StudentUsi, freeLunchDescriptorId, schoolId, programTypeDescriptorId, "Food Service");

            var reducedSchoolLunchStudent = GetNewStudentWithSchoolAssociation(schoolId);
            EnrollInFoodServiceProgram(reducedSchoolLunchStudent.StudentUsi, reducedLunchDescriptorId, schoolId, programTypeDescriptorId, "Food Service");

            var fullPriceSchoolLunchStudent = GetNewStudentWithSchoolAssociation(schoolId);
            EnrollInFoodServiceProgram(fullPriceSchoolLunchStudent.StudentUsi, fullPriceSchoolLunchDescriptorId, schoolId, programTypeDescriptorId, "Food Service");

            var noLunchEligibilityDataStudent = GetNewStudentWithSchoolAssociation(schoolId);

            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);
            EnrollInFoodServiceProgram(inactiveStudentThatShouldNotAppearInReport.StudentUsi, freeLunchDescriptorId, schoolId, programTypeDescriptorId, "Food Service");

            var report = new StudentEconomicSituationReportQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.GetStudentEconomicSummary().First(x => x.Description == "Free or Reduced Price Lunch Eligible").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("50.00 %");
        }

        [Test]
        public void Should_report_student_characteristics()
        {
            var studentWithNoCharacteristicData = GetNewStudentWithSchoolAssociation();
            var schoolId = studentWithNoCharacteristicData.SchoolId;
            var localEducationAgencyId = studentWithNoCharacteristicData.LocalEducationAgencyId;

            var migrantCharacteristicDescriptorId = GetNewDescriptorId("Migrant", "StudentCharacteristic");
            var migrantStudent1 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(migrantStudent1.StudentUsi, migrantCharacteristicDescriptorId, localEducationAgencyId);
            var migrantStudent2 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(migrantStudent2.StudentUsi, migrantCharacteristicDescriptorId, localEducationAgencyId);

            var immigrantCharacteristicDescriptorId = GetNewDescriptorId("Immigrant", "StudentCharacteristic");
            var immigrantStudent1 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(immigrantStudent1.StudentUsi, immigrantCharacteristicDescriptorId, localEducationAgencyId);
            var immigrantStudent2 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(immigrantStudent2.StudentUsi, immigrantCharacteristicDescriptorId, localEducationAgencyId);
            var immigrantStudent3 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(immigrantStudent3.StudentUsi, immigrantCharacteristicDescriptorId, localEducationAgencyId);

            var homelessCharacteristicDescriptorId = GetNewDescriptorId("Homeless", "StudentCharacteristic");
            var homelessStudent1 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(homelessStudent1.StudentUsi, homelessCharacteristicDescriptorId, localEducationAgencyId);
            var homelessStudent2 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(homelessStudent2.StudentUsi, homelessCharacteristicDescriptorId, localEducationAgencyId);
            var homelessStudent3 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(homelessStudent3.StudentUsi, homelessCharacteristicDescriptorId, localEducationAgencyId);
            var homelessStudent4 = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentCharacteristicDescriptor(homelessStudent4.StudentUsi, homelessCharacteristicDescriptorId, localEducationAgencyId);

            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);
            SetStudentCharacteristicDescriptor(inactiveStudentThatShouldNotAppearInReport.StudentUsi, migrantCharacteristicDescriptorId, localEducationAgencyId);

            var report = new StudentEconomicSituationReportQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.TotalStudentCount.ShouldBe(10);
            report.GetStudentEconomicSummary().First(x => x.Description == "Migrant").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("20.00 %");
            report.GetStudentEconomicSummary().First(x => x.Description == "Immigrant").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("30.00 %");
            report.GetStudentEconomicSummary().First(x => x.Description == "Homeless").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("40.00 %");
        }

        [Test]
        public void Should_report_limited_english_proficency_count()
        {
            var studentWithNoLanguageDescriptor = GetNewStudentWithSchoolAssociation();
            var schoolId = studentWithNoLanguageDescriptor.SchoolId;
            var localEducationAgencyId = studentWithNoLanguageDescriptor.LocalEducationAgencyId;

            var limitedEnglishProfiencyDescriptorId = GetNewDescriptorId("Limited", "LimitedEnglishProficiency");
            var studentWithLimitedEnglishProficency = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentLimitedEnglishProficiencyDescriptor(studentWithLimitedEnglishProficency.StudentUsi, limitedEnglishProfiencyDescriptorId);

            var otherLimitedEnglishProfiencyTypeId = GetNewDescriptorId("N/A", "LimitedEnglishProficiency");
            var studentWithOtherProfiencyType = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentLimitedEnglishProficiencyDescriptor(studentWithOtherProfiencyType.StudentUsi, otherLimitedEnglishProfiencyTypeId);

            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);
            SetStudentLimitedEnglishProficiencyDescriptor(inactiveStudentThatShouldNotAppearInReport.StudentUsi, limitedEnglishProfiencyDescriptorId);

            var report = new StudentEconomicSituationReportQuery(TestConnectionProvider).Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);
            report.TotalStudentCount.ShouldBe(3);
            report.GetStudentEconomicSummary().First(x => x.Description == "Limited English Proficiency").PercentOfTotal.ToString("P2", CultureInfo.InvariantCulture).ShouldBe("33.33 %");
        }

        private static void SetStudentRiskStatus(int studentUsi, string riskStatus, int edOrgId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                    INSERT INTO [edfi].[StudentEducationOrganizationAssociationStudentIndicator]
                   (
                        [EducationOrganizationId],
                        [StudentUSI],
                        [IndicatorName],
                        [Indicator]
                    )
                    VALUES
                    (
                        @EducationOrganizationId,
                        @StudentUsi,
                        @RiskStatus,
                        @RiskStatus
                    )"
                    , new { EducationOrganizationId = edOrgId, StudentUsi = studentUsi, RiskStatus = riskStatus });
            }
        }

        private void EnrollInFoodServiceProgram(int studentUsi, int schoolFoodServicesEligibilityDescriptorId, int edOrgId, int programTypeDescriptorId = 1, string programName = "Fake Program", DateTime? beginDate = null, DateTime? endDate = null)
        {
            var nonNullBeginDate = beginDate ?? DateTime.Now;

            EnrollStudentInProgram(studentUsi, edOrgId, programTypeDescriptorId, programName, nonNullBeginDate, endDate);

            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                    INSERT INTO [edfi].[StudentSchoolFoodServiceProgramAssociation]
                    (
                        [BeginDate],
                        [EducationOrganizationId],
                        [ProgramEducationOrganizationId],
                        [ProgramName],
                        [ProgramTypeDescriptorId],
                        [StudentUSI]
                    )
                    VALUES
                    (
                        @BeginDate,
                        @EducationOrganizationId,
                        @EducationOrganizationId,
                        @ProgramName,
                        @ProgramTypeDescriptorId,
                        @StudentUsi
                    )
                    ",
                    new
                    {
                        BeginDate = nonNullBeginDate,
                        EducationOrganizationId = edOrgId,
                        ProgramEducationOrganizationId = edOrgId,
                        ProgramName = programName,
                        ProgramTypeDescriptorId = programTypeDescriptorId,
                        StudentUsi = studentUsi
                    });

                sqlConnection.Execute(@"
                    INSERT INTO [edfi].[StudentSchoolFoodServiceProgramAssociationSchoolFoodServiceProgramService]
                    (
                        [BeginDate],
                        [EducationOrganizationId],
                        [ProgramEducationOrganizationId],
                        [ProgramName],
                        [ProgramTypeDescriptorId],
                        [SchoolFoodServiceProgramServiceDescriptorId],
                        [StudentUSI]
                    )
                     VALUES
                    (
                        @BeginDate,
                        @EducationOrganizationId,
                        @EducationOrganizationId,
                        @ProgramName,
                        @ProgramTypeDescriptorId,
                        @SchoolFoodServiceProgramServiceDescriptorId,
                        @StudentUsi
                    )",
                    new
                    {
                        BeginDate = nonNullBeginDate,
                        EducationOrganizationId = edOrgId,
                        ProgramEducationOrganizationId = edOrgId,
                        ProgramName = programName,
                        ProgramTypeDescriptorId = programTypeDescriptorId,
                        SchoolFoodServiceProgramServiceDescriptorId = schoolFoodServicesEligibilityDescriptorId,
                        StudentUsi = studentUsi
                    });
            }
        }

        private void SetStudentCharacteristicDescriptor(int studentUsi, int studentCharacteristicDescriptorId, int edOrgId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                INSERT INTO [edfi].[StudentEducationOrganizationAssociationStudentCharacteristic]
                (
                    [EducationOrganizationId],
                    [StudentUSI],
                    [StudentCharacteristicDescriptorId]
                )
                 VALUES
                (
                    @EducationOrganizationId,
                    @StudentUsi,
                    @StudentCharacteristicDescriptorId
                )"
                    , new { EducationOrganizationId = edOrgId, StudentUsi = studentUsi, StudentCharacteristicDescriptorId = studentCharacteristicDescriptorId});
            }
        }

        private void SetStudentLimitedEnglishProficiencyDescriptor(int studentUsi, int? limitedEnglishProficiencyDescriptorId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                    UPDATE edfi.StudentEducationOrganizationAssociation
                    SET LimitedEnglishProficiencyDescriptorId = @LimitedEnglishProficiencyDescriptorId
                    WHERE StudentUSI = @StudentUsi"
                    , new { StudentUsi = studentUsi, LimitedEnglishProficiencyDescriptorId = limitedEnglishProficiencyDescriptorId });
            }
        }
    }
}
