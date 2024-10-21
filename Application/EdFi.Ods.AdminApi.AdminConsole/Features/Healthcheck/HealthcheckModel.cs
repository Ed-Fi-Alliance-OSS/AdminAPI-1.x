// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck
{
    public class HealthcheckModel
    {
        [JsonProperty("localEducationAgencyId")]
        public long LocalEducationAgencyId { get; set; }

        [JsonProperty("studentSpecialEducationProgramAssociations")]
        public long StudentSpecialEducationProgramAssociations { get; set; }

        [JsonProperty("studentDisciplineIncidentBehaviorAssociations")]
        public long StudentDisciplineIncidentBehaviorAssociations { get; set; }

        [JsonProperty("studentSchoolAssociations")]
        public long StudentSchoolAssociations { get; set; }

        [JsonProperty("studentSchoolAttendanceEvents")]
        public long StudentSchoolAttendanceEvents { get; set; }

        [JsonProperty("studentSectionAssociations")]
        public long StudentSectionAssociations { get; set; }

        [JsonProperty("staffEducationOrganizationAssignmentAssociations")]
        public long StaffEducationOrganizationAssignmentAssociations { get; set; }

        [JsonProperty("staffEducationOrganizationEmploymentAssociations")]
        public long StaffEducationOrganizationEmploymentAssociations { get; set; }

        [JsonProperty("staffSectionAssociations")]
        public long StaffSectionAssociations { get; set; }

        [JsonProperty("courseTranscripts")]
        public long CourseTranscripts { get; set; }

        [JsonProperty("basicReportingPeriodAttendances")]
        public long BasicReportingPeriodAttendances { get; set; }

        [JsonProperty("sections")]
        public long Sections { get; set; }

        [JsonProperty("reportingPeriodExts")]
        public long ReportingPeriodExts { get; set; }

        [JsonProperty("healthy")]
        public bool Healthy { get; set; }
    }
}
