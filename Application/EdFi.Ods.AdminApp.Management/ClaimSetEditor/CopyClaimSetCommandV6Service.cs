// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class CopyClaimSetCommandV6Service
    {
        private readonly ISecurityContext _context;

        public CopyClaimSetCommandV6Service(ISecurityContext context)
        {
            _context = context;
        }

        public int Execute(ICopyClaimSetModel claimSet)
        {
            var newClaimSet = new Security.DataAccess.Models.ClaimSet
            {
                ClaimSetName = claimSet.Name,
                Application = _context.Applications.Single(),
                IsEdfiPreset = false,
                ForApplicationUseOnly = false
            };

            var originalResourceClaims =
                _context.ClaimSetResourceClaimActions
                    .Where(x => x.ClaimSet.ClaimSetId == claimSet.OriginalId)
                    .Include(x => x.ResourceClaim)
                    .Include(x => x.Action)
                    .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
                    .ToList();
            _context.ClaimSets.Add(newClaimSet);

            foreach (var resourceClaim in originalResourceClaims.ToList())
            {
                List<ClaimSetResourceClaimActionAuthorizationStrategyOverrides> authStrategyOverrides = null;
                if (resourceClaim.AuthorizationStrategyOverrides != null && resourceClaim.AuthorizationStrategyOverrides.Any())
                {
                    authStrategyOverrides = new List<ClaimSetResourceClaimActionAuthorizationStrategyOverrides>();
                    foreach (var authStrategyOverride in resourceClaim.AuthorizationStrategyOverrides)
                    {
                        authStrategyOverrides.Add(new ClaimSetResourceClaimActionAuthorizationStrategyOverrides
                        { AuthorizationStrategy = authStrategyOverride.AuthorizationStrategy });
                    }
                }
                var copyResourceClaim = new ClaimSetResourceClaimAction
                {
                    ClaimSet = newClaimSet,
                    Action = resourceClaim.Action,
                    AuthorizationStrategyOverrides = authStrategyOverrides,
                    ResourceClaim = resourceClaim.ResourceClaim
                };
                _context.ClaimSetResourceClaimActions.Add(copyResourceClaim);
            }
            _context.SaveChanges();

            return newClaimSet.ClaimSetId;
        }
    }
}
