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
    public class StudentEnrollmentByRaceQuery
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        private const string Query = @" SELECT studentUniqueId, StudentRace FROM adminapp.StudentDemographicsReport 
                                  WHERE EducationOrganizationId = @LEAid AND LocalEducationAgencyId = @LEAid";

        public StudentEnrollmentByRaceQuery(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public StudentRaceReport Execute(string instanceName, ApiMode apiMode, int localEducationAgencyId)
        {
            using (var sqlConnection = _connectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                var studentDemographicData = sqlConnection.Query<StudentDemographicData>(Query, new { LEAid = localEducationAgencyId}).ToList();

                var localEducationAgencyName = new GetLocalEducationAgencyByIdQuery(_connectionProvider).Execute(instanceName, apiMode, localEducationAgencyId);

                return new StudentRaceReport(studentDemographicData, localEducationAgencyName, localEducationAgencyId);
            }
        }
    }

    public class StudentRaceReport : StudentDemographicsReportBase
    {
        public StudentRaceReport(List<StudentDemographicData> studentDemographicData, string localEducationAgencyName, int localEducationAgencyId)
        {
            StudentDemographicData = studentDemographicData;
            LocalEducationAgencyName = localEducationAgencyName;
            LocalEducationAgencyId = localEducationAgencyId;
        }

        public List<DemographicRepresentation> RaceRepresentation => GetDemographicRepresentation(x => x.StudentRace);
    }
}
