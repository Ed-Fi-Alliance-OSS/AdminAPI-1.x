// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data;
using Dapper;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public class SetSchoolYearCommand
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public SetSchoolYearCommand(IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public void Execute(string instanceName, ApiMode apiMode, short schoolYear)
        {
            using (var connection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                connection.Execute(
                    "edfi.SetCurrentSchoolYear",
                    new { schoolYear },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
