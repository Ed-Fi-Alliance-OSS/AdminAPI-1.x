// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using Dapper;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Database.Ods
{
    public class GetLocalEducationAgencyByIdQuery
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;
        private const string Query = @" SELECT Name FROM adminapp.LocalEducationAgenciesReport WHERE LocalEducationAgencyId = @LEAid";

        public GetLocalEducationAgencyByIdQuery(IDatabaseConnectionProvider connectionProvider)
        {
            _databaseConnectionProvider = connectionProvider;
        }

        public string Execute(string instanceName, ApiMode apiMode, int localEducationAgencyId)
        {
            using (var sqlConnection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                return sqlConnection.Query<string>(Query, new { LEAid = localEducationAgencyId }).Single();
            }
        }
    }
}
