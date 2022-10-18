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
    public class GetSchoolsBySchoolTypeQuery
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        private const string SchoolsCountQuery = @" SELECT Description, Count FROM adminapp.DistrictSchoolsByTypeReport
                WHERE LocalEducationAgencyId = @LEAid";

        public GetSchoolsBySchoolTypeQuery(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public SchoolTypeReport Execute(string instanceName, ApiMode apiMode, int localEducationAgencyId)
        {
            using (var sqlConnection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                var schoolCounts = sqlConnection.Query<SchoolCount>(SchoolsCountQuery, new { LEAId = localEducationAgencyId }).ToList();

                var localEducationAgencyName = new GetLocalEducationAgencyByIdQuery(_databaseConnectionProvider).Execute(instanceName, apiMode, localEducationAgencyId);

                return new SchoolTypeReport
                {
                    SchoolCounts = schoolCounts,
                    LocalEducationAgencyName = localEducationAgencyName,
                    LocalEducationAgencyId = localEducationAgencyId
                };
            }
        }
    }

    public class SchoolTypeReport
    {
        public List<SchoolCount> SchoolCounts { get; set; }
        public string LocalEducationAgencyName { get; set; }
        public int LocalEducationAgencyId { get; set; }
    }

    public class SchoolCount
    {
        public string Description { get; set; }
        public int Count { get; set; }
    }
}
