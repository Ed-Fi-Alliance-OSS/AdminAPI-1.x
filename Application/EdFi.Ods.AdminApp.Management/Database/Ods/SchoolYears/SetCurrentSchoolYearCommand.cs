// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Dapper;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears
{
    public interface ISetCurrentSchoolYearCommand
    {
        void Execute(string instanceName, ApiMode apiMode, short schoolYear);
    }

    public class SetCurrentSchoolYearCommand : ISetCurrentSchoolYearCommand
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public SetCurrentSchoolYearCommand(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public void Execute(string instanceName, ApiMode apiMode, short schoolYear)
        {
            using (var connection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                // Take special care that any modifications to the SQL here fall
                // within the common subset of SQL Server and Postgres syntax.

                var rowsAffected = connection.Execute(
                    @"UPDATE edfi.SchoolYearType
                      SET CurrentSchoolYear = 'true'
                      WHERE SchoolYear = @schoolYear", new { schoolYear });

                if (rowsAffected == 0)
                    throw new Exception($"School year {schoolYear} does not exist.");

                connection.Execute(
                    @"UPDATE edfi.SchoolYearType
                      SET CurrentSchoolYear = 'false'
                      WHERE SchoolYear <> @schoolYear", new { schoolYear });
            }
        }
    }
}
