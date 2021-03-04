// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Dapper;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public class GetCurrentSchoolYearQuery
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public GetCurrentSchoolYearQuery(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public SchoolYearType Execute(string instanceName, ApiMode apiMode)
        {
            using (var connection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                return connection.QuerySingleOrDefault<SchoolYearType>(
                    @"SELECT [SchoolYear], [SchoolYearDescription], [CurrentSchoolYear]
                      FROM [edfi].[SchoolYearType]
                      WHERE [CurrentSchoolYear]=1");
            }
        }
    }
}
