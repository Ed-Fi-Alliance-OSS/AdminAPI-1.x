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
    public class DistrictCharacteristicsReportQueryTests : OdsDataTestBase
    {
        [Test]
        public void ShouldRetrieveSchoolCount()
        {
            var elementarySchoolCategoryDescriptorId = GetNewSchoolCategoryDescriptorId("Elementary School");
            var highSchoolCategoryDescriptorId = GetNewSchoolCategoryDescriptorId("High School");
            var middleSchoolCategoryDescriptorId = GetNewSchoolCategoryDescriptorId("Middle School");

            var localEducationAgencyId = GetNewLocalEducationAgencyId();
            var newHighSchoolId = GetNewSchool(localEducationAgencyId);
            SetSchoolCategoryDescriptor(newHighSchoolId, highSchoolCategoryDescriptorId);

            var newMiddleSchoolId1 = GetNewSchool(localEducationAgencyId);
            var newMiddleSchoolId2 = GetNewSchool(localEducationAgencyId);
            SetSchoolCategoryDescriptor(newMiddleSchoolId1, middleSchoolCategoryDescriptorId);
            SetSchoolCategoryDescriptor(newMiddleSchoolId2, middleSchoolCategoryDescriptorId);

            var newElementarySchoolId1 = GetNewSchool(localEducationAgencyId);
            var newElementarySchoolId2 = GetNewSchool(localEducationAgencyId);
            var newElementarySchoolId3 = GetNewSchool(localEducationAgencyId);
            SetSchoolCategoryDescriptor(newElementarySchoolId1, elementarySchoolCategoryDescriptorId);
            SetSchoolCategoryDescriptor(newElementarySchoolId2, elementarySchoolCategoryDescriptorId);
            SetSchoolCategoryDescriptor(newElementarySchoolId3, elementarySchoolCategoryDescriptorId);

            var otherLocalAgencyId = GetNewLocalEducationAgencyId();
            var otherHighSchoolId = GetNewSchool(otherLocalAgencyId);
            var otherMiddleSchoolId = GetNewSchool(otherLocalAgencyId);
            var otherElementarySchoolId = GetNewSchool(otherLocalAgencyId);

            SetSchoolCategoryDescriptor(otherHighSchoolId, highSchoolCategoryDescriptorId);
            SetSchoolCategoryDescriptor(otherMiddleSchoolId, middleSchoolCategoryDescriptorId);
            SetSchoolCategoryDescriptor(otherElementarySchoolId, elementarySchoolCategoryDescriptorId);

            var report = new GetSchoolsBySchoolTypeQuery(TestConnectionProvider);
            var result = report.Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);

            result.SchoolCounts.Count.ShouldBe(3);
            result.SchoolCounts.Single(c => c.Description == "High School").Count.ShouldBe(1);
            result.SchoolCounts.Single(c => c.Description == "Middle School").Count.ShouldBe(2);
            result.SchoolCounts.Single(c => c.Description == "Elementary School").Count.ShouldBe(3);
        }

        [Test]
        public void ShouldRetrieveTotalEnrollment()
        {
            var localEducationAgencyId = GetNewLocalEducationAgencyId();
            var schoolId = GetNewSchool(localEducationAgencyId);
            var student1 = GetNewStudentWithSchoolAssociation(schoolId);
            var student2 = GetNewStudentWithSchoolAssociation(schoolId);
            var studentInactiveTomorrow = GetNewStudentWithSchoolAssociation(schoolId);  //should still be counted
            SetStudentInactive(studentInactiveTomorrow.StudentUsi, schoolId, DateTime.Today.AddDays(1));

            var inactiveStudentThatShouldNotAppearInReport = GetNewStudentWithSchoolAssociation(schoolId);
            SetStudentInactive(inactiveStudentThatShouldNotAppearInReport.StudentUsi, schoolId);
            
            var otherLocalEducationAgencyId = GetNewLocalEducationAgencyId();
            var otherSchoolId = GetNewSchool(otherLocalEducationAgencyId);
            var otherStudent1 = GetNewStudentWithSchoolAssociation(otherSchoolId);
            var otherStudent2 = GetNewStudentWithSchoolAssociation(otherSchoolId);

            var report = new TotalEnrollmentQuery(TestConnectionProvider);
            var result = report.Execute(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox, localEducationAgencyId);

            result.EnrollmentCount.ShouldBe(3);
        }
    }
}
