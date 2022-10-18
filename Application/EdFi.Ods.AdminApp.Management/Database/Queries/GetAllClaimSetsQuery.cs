// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public class GetAllClaimSetsQuery
    {
        private readonly ISecurityContext _securityContext;

        public GetAllClaimSetsQuery(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public IEnumerable<ClaimSet> Execute()
        {
            return _securityContext.ClaimSets
                .Distinct()
                .OrderBy(x => x.ClaimSetName)
                .ToList();
        } 
    }
}
