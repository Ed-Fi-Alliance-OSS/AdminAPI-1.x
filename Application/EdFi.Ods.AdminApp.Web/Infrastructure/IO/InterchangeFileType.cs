// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public sealed class InterchangeFileType : Enumeration<InterchangeFileType>
    {
        public static InterchangeFileType AssessmentMetadata = new InterchangeFileType(1, "Assessment Metadata", "AssessmentMetadata");

        //Descriptor uploads disallowed 
        //public static UploadFileType Descriptors = new UploadFileType(2, "Descriptor", "Descriptors");

        public static InterchangeFileType EducationOrganization = new InterchangeFileType(3, "Education Organization", "EducationOrganization");
        public static InterchangeFileType EducationOrgCalendar = new InterchangeFileType(4, "Education Organization Calendar", "EducationOrgCalendar");
        public static InterchangeFileType Finance = new InterchangeFileType(5, "Finance", "Finance");
        public static InterchangeFileType MasterSchedule = new InterchangeFileType(6, "Master Schedule", "MasterSchedule");
        public static InterchangeFileType Parent = new InterchangeFileType(7, "Parent", "Parent");
        public static InterchangeFileType PostSecondaryEvent = new InterchangeFileType(8, "Post Secondary Event", "PostSecondaryEvent");
        public static InterchangeFileType StaffAssociation = new InterchangeFileType(9, "Staff Association", "StaffAssociation");
        public static InterchangeFileType Standards = new InterchangeFileType(10, "Standards", "Standards");
        public static InterchangeFileType Student = new InterchangeFileType(11, "Student", "Student");
        public static InterchangeFileType StudentAssessment = new InterchangeFileType(12, "Student Assessment", "StudentAssessment");
        public static InterchangeFileType StudentAttendance = new InterchangeFileType(13, "Student Attendance", "StudentAttendance");
        public static InterchangeFileType StudentCohort = new InterchangeFileType(14, "Student Cohort", "StudentCohort");
        public static InterchangeFileType StudentDiscipline = new InterchangeFileType(15, "Student Discipline", "StudentDiscipline");
        public static InterchangeFileType StudentEnrollment = new InterchangeFileType(16, "Student Enrollment", "StudentEnrollment");
        public static InterchangeFileType StudentGrade = new InterchangeFileType(17, "Student Grade", "StudentGrade");
        public static InterchangeFileType StudentGradebook = new InterchangeFileType(18, "Student Gradebook", "StudentGradebook");
        public static InterchangeFileType StudentIntervention = new InterchangeFileType(19, "Student Intervention", "StudentIntervention");
        public static InterchangeFileType StudentProgram = new InterchangeFileType(20, "Student Program", "StudentProgram");
        public static InterchangeFileType StudentTranscript = new InterchangeFileType(21, "Student Transcript", "StudentTranscript");

        public string InterchangeFileNamePrefix { get; }

        private InterchangeFileType(int value, string displayName, string interchangeFileNamePrefix) : base(value, displayName)
        {
            InterchangeFileNamePrefix = interchangeFileNamePrefix;
        }
    }
}