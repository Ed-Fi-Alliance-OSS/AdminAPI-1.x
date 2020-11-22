// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstanceSettings;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Reports
{
    public class ReportsIndexModel : BaseOdsInstanceSettingsModel
    {
        public SelectDistrictModel SelectDistrictModel { get; set; }
        public TotalEnrollmentReport TotalEnrollmentReport { get; set; }
        public SchoolTypeReport SchoolTypeReport { get; set; }
        public StudentGenderReport StudentGenderReport { get; set; }
        public StudentRaceReport StudentRaceReport { get; set; }
        public StudentEthnicityReport StudentEthnicityReport { get; set; }
        public StudentsByProgramReport StudentsByProgramReport { get; set; }
        public StudentEconomicSituationReport StudentEconomicSituationReport { get; set; }
    }
}
