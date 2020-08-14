// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public class GetClaimSetNamesQuery
    {
        private readonly ISecurityContext _securityContext;

        public GetClaimSetNamesQuery(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public IEnumerable<string> Execute()
        {
            return _securityContext.ClaimSets
                .Select(x => x.ClaimSetName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        } 
    }
}
