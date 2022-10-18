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
    public class StudentsByProgramQuery
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        private const string Query = @"SELECT ProgramName FROM adminapp.StudentsByProgramReport 
                                      WHERE EducationOrganizationId = @LEAid AND LocalEducationAgencyId =  @LEAid";

        public StudentsByProgramQuery(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public StudentsByProgramReport Execute(string instanceName, ApiMode apiMode, int localEducationAgencyId)
        {
            using (var sqlConnection = _connectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                var studentPrograms = sqlConnection.Query<string>(Query, new {LEAId = localEducationAgencyId }).ToList();

                var localEducationAgencyName = new GetLocalEducationAgencyByIdQuery(_connectionProvider).Execute(instanceName, apiMode, localEducationAgencyId);

                return new StudentsByProgramReport(studentPrograms, localEducationAgencyName, localEducationAgencyId);
            }
        }
    }

    public class StudentsByProgramReport
    {
        public string LocalEducationAgencyName { get; }
        public int LocalEducationAgencyId { get; }
        private readonly List<string> _studentPrograms;
        public int TotalStudentCount => _studentPrograms.Count;

        public List<StudentProgramRepresentation> GetAllPrograms()
        {
            return
                _studentPrograms
                    .Where(programName => !string.IsNullOrEmpty(programName))
                    .GroupBy(programName => programName)
                    .Select(group => new StudentProgramRepresentation
                    {
                        ProgramName = group.Key,
                        PercentOfTotalStudents = (decimal)group.Count() / TotalStudentCount
                    })
                    .ToList();
        }

        public StudentsByProgramReport(List<string> studentPrograms, string localEducationAgencyName, int localEducationAgencyId)
        {
            _studentPrograms = studentPrograms;
            LocalEducationAgencyName = localEducationAgencyName;
            LocalEducationAgencyId = localEducationAgencyId;
        }
    }

    public class StudentProgramRepresentation
    {
        public string ProgramName { get; set; }
        public decimal PercentOfTotalStudents { get; set; }
    }
}
