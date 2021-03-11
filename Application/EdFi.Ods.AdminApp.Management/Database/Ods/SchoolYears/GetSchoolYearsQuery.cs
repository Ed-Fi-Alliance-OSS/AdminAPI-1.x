// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Dapper;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears
{
    public class GetSchoolYearsQuery
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public GetSchoolYearsQuery(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public IReadOnlyList<SchoolYearType> Execute(string instanceName, ApiMode apiMode)
        {
            using (var connection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                return connection.Query<SchoolYearType>(
                    @"SELECT SchoolYear, SchoolYearDescription, CurrentSchoolYear
                      FROM edfi.SchoolYearType
                      ORDER BY SchoolYear").ToList();
            }
        }
    }
}
