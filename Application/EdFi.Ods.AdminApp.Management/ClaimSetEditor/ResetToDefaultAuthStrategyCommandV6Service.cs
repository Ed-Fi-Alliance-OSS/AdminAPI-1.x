// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class ResetToDefaultAuthStrategyCommandV6Service
    {
        private readonly ISecurityContext _context;

        public ResetToDefaultAuthStrategyCommandV6Service(ISecurityContext context)
        {
            _context = context;
        }

        public void Execute(IResetToDefaultAuthStrategyModel model)
        {
            var claimSetResourceClaimsToEdit = _context.ClaimSetResourceClaimActions
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.ClaimSet)
                .Include(x => x.AuthorizationStrategyOverrides)
                .Where(x => x.ResourceClaim.ResourceClaimId == model.ResourceClaimId && x.ClaimSet.ClaimSetId == model.ClaimSetId)
                .ToList();

            foreach (var claimSetResourceClaim in claimSetResourceClaimsToEdit)
            {
                var existingAuthorizationStrategies = _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.First(x => x.ClaimSetResourceClaimActionId == claimSetResourceClaim.ClaimSetResourceClaimActionId);
                if (existingAuthorizationStrategies != null)
                {
                    _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.Remove(existingAuthorizationStrategies);
                }
                claimSetResourceClaim.AuthorizationStrategyOverrides = null;
            }

            _context.SaveChanges();
        }
    }
}
