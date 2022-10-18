// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Dapper;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public class StudentEconomicSituationReportQuery
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        private const string Query = @" SELECT StudentUniqueId, AtRisk, 
                           EconomicallyDisadvantaged, FreeOrReducedPriceLunchEligible, Migrant, Immigrant, Homeless, LimitedEnglishProficency
                           FROM adminapp.StudentEconomicSituationReport WHERE LocalEducationAgencyId = @LEAid AND EducationOrganizationId = @LEAid";

        public StudentEconomicSituationReportQuery(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public StudentEconomicSituationReport Execute(string instanceName, ApiMode apiMode, int localEducationAgencyId)
        {
            using (var sqlConnection = _connectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                var studentEconomicSituations = sqlConnection.Query<StudentEconomicSituation>(Query,new {LEAId = localEducationAgencyId}).ToList();

                var localEducationAgencyName = new GetLocalEducationAgencyByIdQuery(_connectionProvider).Execute(instanceName, apiMode, localEducationAgencyId);

                return new StudentEconomicSituationReport(studentEconomicSituations, localEducationAgencyName, localEducationAgencyId);
            }
        }
    }

    public class StudentEconomicSituationReport
    {
        public string LocalEducationAgencyName { get; }
        public int LocalEducationAgencyId { get; }
        private readonly List<StudentEconomicSituation> _studentEconomicSituations;
        public int TotalStudentCount => _studentEconomicSituations.Count;

        public StudentEconomicSituationReport(List<StudentEconomicSituation> studentEconomicSituations, string localEducationAgencyName, int localEducationAgencyId)
        {
            _studentEconomicSituations = studentEconomicSituations;
            LocalEducationAgencyName = localEducationAgencyName;
            LocalEducationAgencyId = localEducationAgencyId;
        }

        public List<StudentEconomicSituationRepresentation> GetStudentEconomicSummary()
        {
            var economicSituations = new List<StudentEconomicSituationRepresentation>
            {
                new StudentEconomicSituationRepresentation
                {
                    Description = "At Risk",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.AtRisk) / TotalStudentCount
                },
                new StudentEconomicSituationRepresentation
                {
                    Description = "Economically Disadvantaged",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.EconomicallyDisadvantaged) / TotalStudentCount
                },
                new StudentEconomicSituationRepresentation
                {
                    Description = "Free or Reduced Price Lunch Eligible",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.FreeOrReducedPriceLunchEligible) / TotalStudentCount
                },
                new StudentEconomicSituationRepresentation
                {
                    Description = "Migrant",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.Migrant) / TotalStudentCount
                },
                new StudentEconomicSituationRepresentation
                {
                    Description = "Immigrant",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.Immigrant) / TotalStudentCount
                },
                new StudentEconomicSituationRepresentation
                {
                    Description = "Homeless",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.Homeless) / TotalStudentCount
                },
                new StudentEconomicSituationRepresentation
                {
                    Description = "Limited English Proficiency",
                    PercentOfTotal = (decimal) _studentEconomicSituations.Count(x => x.LimitedEnglishProficency) / TotalStudentCount
                },
            };
            return economicSituations;
        }
    }

    public class StudentEconomicSituation
    {
        public string StudentUniqueId { get; set; }
        public bool AtRisk { get; set; }
        public bool EconomicallyDisadvantaged { get; set; }
        public bool FreeOrReducedPriceLunchEligible { get; set; }
        public bool Migrant { get; set; }
        public bool Immigrant { get; set; }
        public bool Homeless { get; set; }
        public bool LimitedEnglishProficency { get; set; }
    }

    public class StudentEconomicSituationRepresentation
    {
        public string Description { get; set; }
        public decimal PercentOfTotal { get; set; }
    }
}
