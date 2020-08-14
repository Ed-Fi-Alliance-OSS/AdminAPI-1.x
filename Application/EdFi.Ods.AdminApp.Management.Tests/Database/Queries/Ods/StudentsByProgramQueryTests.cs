// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Ods;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    [TestFixture]
    public class StudentsByProgramQueryTests : OdsDataTestBase
    {
        [Test]
        public void ShouldProduceCorrectResultsForStudentsCurrentlyEnrolledInAProgram()
        {
            var localEducationAgencyId = GetNewLocalEducationAgencyId();
            var schoolId = GetNewSchool(localEducationAgencyId);
            var student1 = GetNewStudentWithSchoolAssociation(schoolId);
            var student2 = GetNewStudentWithSchoolAssociation(schoolId);
            var student3 = GetNewStudentWithSchoolAssociation(schoolId);
            var student4 = GetNewStudentWithSchoolAssociation(schoolId);

            var studentWithNoProgram = GetNewStudentWithSchoolAssociation(schoolId);
            var inactiveStudentThatShouldNotBeCounted = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotBeCounted.StudentUsi, schoolId);

            var otherLocalEducationAgencyId = GetNewLocalEducationAgencyId();
            var otherSchoolId = GetNewSchool(otherLocalEducationAgencyId);
            var otherStudent = GetNewStudentWithSchoolAssociation(otherSchoolId);

            var programName = "Fake Program";
            var programDescriptorId = GetNewProgramDescriptorId(programName);
            CreateProgram(localEducationAgencyId, programDescriptorId, programName);
            CreateProgram(otherLocalEducationAgencyId, programDescriptorId, programName);

            var otherProgramName = "Other Fake Program";
            var otherProgramDescriptorId = GetNewProgramDescriptorId(otherProgramName);
            CreateProgram(localEducationAgencyId, otherProgramDescriptorId, otherProgramName);
            
            var yesterday = DateTime.Today.AddDays(-1);
            var dayBeforeYesterday = DateTime.Today.AddDays(-2);

            EnrollStudentInProgram(student1.StudentUsi, localEducationAgencyId, programDescriptorId, programName, yesterday, DateTime.MaxValue);
            EnrollStudentInProgram(student2.StudentUsi, localEducationAgencyId, programDescriptorId, programName, yesterday, null);
            EnrollStudentInProgram(student3.StudentUsi, localEducationAgencyId, programDescriptorId, programName, dayBeforeYesterday, yesterday);
            EnrollStudentInProgram(student4.StudentUsi, localEducationAgencyId, otherProgramDescriptorId, otherProgramName, dayBeforeYesterday, null);
            EnrollStudentInProgram(inactiveStudentThatShouldNotBeCounted.StudentUsi, localEducationAgencyId, programDescriptorId, programName, yesterday, DateTime.MaxValue);
            EnrollStudentInProgram(otherStudent.StudentUsi, otherLocalEducationAgencyId, programDescriptorId, programName, yesterday, null);

            var query = new StudentsByProgramQuery(TestConnectionProvider);
            var result = query.Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);

            result.TotalStudentCount.ShouldBe(3);
            result.GetAllPrograms().Single(p => p.ProgramName == programName).PercentOfTotalStudents.ShouldBe(0.66m, 0.01m);
            result.GetAllPrograms().Single(p => p.ProgramName == otherProgramName).PercentOfTotalStudents.ShouldBe(0.33m, 0.01m);
        }
    }
}
