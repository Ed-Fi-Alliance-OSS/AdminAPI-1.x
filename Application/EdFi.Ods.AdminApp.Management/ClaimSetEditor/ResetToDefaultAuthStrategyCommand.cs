// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class ResetToDefaultAuthStrategyCommand
    {
        private readonly ISecurityContext _context;

        public ResetToDefaultAuthStrategyCommand(ISecurityContext context)
        {
            _context = context;
        }

        public void Execute(IResetToDefaultAuthStrategyModel model)
        {         
            var claimSetResourceClaimsToEdit = _context.ClaimSetResourceClaims
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.ClaimSet)
                .Include(x => x.AuthorizationStrategyOverride)
                .Where(x => x.ResourceClaim.ResourceClaimId == model.ResourceClaimId && x.ClaimSet.ClaimSetId == model.ClaimSetId)
                .ToList();

            foreach (var claimSetResourceClaim in claimSetResourceClaimsToEdit)
            {
                claimSetResourceClaim.AuthorizationStrategyOverride = null;
            }

            _context.SaveChanges();
        }
    }

    public interface IResetToDefaultAuthStrategyModel
    {
        int ClaimSetId { get; }
        int ResourceClaimId { get; }
    }
}
