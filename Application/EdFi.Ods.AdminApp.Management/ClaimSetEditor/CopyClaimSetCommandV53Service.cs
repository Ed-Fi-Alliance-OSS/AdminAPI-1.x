// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Linq;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.SecurityCompatiblity53.DataAccess.Models;

using SecurityClaimSet = EdFi.SecurityCompatiblity53.DataAccess.Models.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class CopyClaimSetCommandV53Service
    {
        private readonly ISecurityContext _context;

        public CopyClaimSetCommandV53Service(ISecurityContext context)
        {
            _context = context;
        }

        public int Execute(ICopyClaimSetModel claimSet)
        {
            var newClaimSet = new SecurityClaimSet
            {
                ClaimSetName = claimSet.Name,
                Application = _context.Applications.Single()
            };

            var originalResourceClaims =
                _context.ClaimSetResourceClaims
                    .Where(x => x.ClaimSet.ClaimSetId == claimSet.OriginalId)
                    .Include(x => x.ResourceClaim)
                    .Include(x => x.Action)
                    .Include(x => x.AuthorizationStrategyOverride)
                    .ToList();
            _context.ClaimSets.Add(newClaimSet);

            foreach (var resourceClaim in originalResourceClaims)
            {
                var copyResourceClaim = new ClaimSetResourceClaim
                {
                    ClaimSet = newClaimSet,
                    Action = resourceClaim.Action,
                    AuthorizationStrategyOverride = resourceClaim.AuthorizationStrategyOverride,
                    ResourceClaim = resourceClaim.ResourceClaim
                };
                _context.ClaimSetResourceClaims.Add(copyResourceClaim);
            }
            _context.SaveChanges();

            return newClaimSet.ClaimSetId;
        }
    }
}
