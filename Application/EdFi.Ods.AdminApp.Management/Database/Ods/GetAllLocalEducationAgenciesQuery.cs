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
    public class GetAllLocalEducationAgenciesQuery
    {
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;
        private const string Query = @" SELECT * FROM adminapp.LocalEducationAgenciesReport";

        public GetAllLocalEducationAgenciesQuery(IDatabaseConnectionProvider connectionProvider)
        {
            _databaseConnectionProvider = connectionProvider;
        }

        public List<LocalEducationAgencyModel> Execute(string instanceName, ApiMode apiMode)
        {
            using (var sqlConnection = _databaseConnectionProvider.CreateNewConnection(instanceName, apiMode))
            {
                return sqlConnection.Query<LocalEducationAgencyModel>(Query).ToList();
            }
        }

        public class LocalEducationAgencyModel
        {
            public int LocalEducationAgencyId { get; set; }
            public string Name { get; set; }
        }
    }
}
