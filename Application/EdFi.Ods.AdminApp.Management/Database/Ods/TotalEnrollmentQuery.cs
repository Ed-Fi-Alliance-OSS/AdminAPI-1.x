// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Dapper;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public class TotalEnrollmentQuery
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        private const string EnrollmentsCountQuery =
            @"SELECT TotalEnrollment FROM adminapp.DistrictTotalEnrollmentsReport WHERE LocalEducationAgencyId = @LEAid";

        public TotalEnrollmentQuery(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public TotalEnrollmentReport Execute(string instanceName, ApiMode apiMode, int localEducationAgencyId)
        {
            using (var sqlConnection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                var enrollmentCount = sqlConnection.QueryFirstOrDefault<int>(EnrollmentsCountQuery, new {LEAId = localEducationAgencyId});

                var localEducationAgencyName = new GetLocalEducationAgencyByIdQuery(_databaseConnectionProvider).Execute(instanceName, apiMode, localEducationAgencyId);

                return new TotalEnrollmentReport
                {
                    EnrollmentCount = enrollmentCount,
                    LocalEducationAgencyName = localEducationAgencyName,
                    LocalEducationAgencyId = localEducationAgencyId
                };
            }
        }
    }

    public class TotalEnrollmentReport
    {
        public int EnrollmentCount { get; set; }
        public string LocalEducationAgencyName { get; set; }
        public int LocalEducationAgencyId { get; set; }
    }
}
