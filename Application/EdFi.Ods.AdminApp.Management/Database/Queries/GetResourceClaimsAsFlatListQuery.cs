// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public class GetResourceClaimsAsFlatListQuery
    {
        private readonly ISecurityContext _securityContext;

        public GetResourceClaimsAsFlatListQuery(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public IEnumerable<ResourceClaim> Execute()
        {
            return _securityContext.ResourceClaims
                .Select(x => new ResourceClaim
                {
                    Id = x.ResourceClaimId,
                    Name = x.ResourceName,
                    ParentId = x.ParentResourceClaimId ?? 0,
                    ParentName = x.ParentResourceClaim.ResourceName
                })
                .Distinct()
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
