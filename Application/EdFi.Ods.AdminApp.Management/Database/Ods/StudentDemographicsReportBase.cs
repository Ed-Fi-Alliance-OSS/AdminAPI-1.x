// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public abstract class StudentDemographicsReportBase
    {
        public string LocalEducationAgencyName { get; set; }
        public int LocalEducationAgencyId { get; set; }
        protected List<StudentDemographicData> StudentDemographicData;
        public int TotalStudentCount => StudentDemographicData?.Count ?? 0;

        protected List<DemographicRepresentation> GetDemographicRepresentation(Func<StudentDemographicData, string> demographic)
        {
            return StudentDemographicData.GroupBy(demographic)
                .Select(g => new DemographicRepresentation
                {
                    Value = g.Key,
                    PercentOfTotal = (decimal)g.Count() / TotalStudentCount
                }).ToList();
        }

        public class DemographicRepresentation
        {
            public string Value { get; set; }
            public decimal PercentOfTotal { get; set; }

            public string GetDisplayName()
            {
                return string.IsNullOrEmpty(Value) ? "No Data" : Value;
            }
        }
    }

    public class StudentDemographicData
    {
        public string StudentUniqueId { get; set; }
        public string Gender { get; set; }
        public bool HispanicLatinoEthnicity { get; set; }
        public string StudentRace { get; set; }
    }
}