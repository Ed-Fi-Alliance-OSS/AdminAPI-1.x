﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public class GetOdsInstanceQuery
    {
        private readonly IUsersContext _usersContext;

        public GetOdsInstanceQuery(IUsersContext userContext)
        {
            _usersContext = userContext;
        }

        public OdsInstance Execute(string instanceName)
        {
            return _usersContext.OdsInstances.SingleOrDefault(i => i.Name == instanceName);
        }
    }
}
