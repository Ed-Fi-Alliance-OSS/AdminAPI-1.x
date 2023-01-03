// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class DeleteClaimSetCommandV53Service
    {
        private readonly ISecurityContext _context;

        public DeleteClaimSetCommandV53Service(ISecurityContext context)
        {
            _context = context;
        }

        public void Execute(IDeleteClaimSetModel claimSet)
        {
            var claimSetToDelete = _context.ClaimSets.Single(x => x.ClaimSetId == claimSet.Id);

            if (CloudOdsAdminApp.DefaultClaimSets.Contains(claimSetToDelete.ClaimSetName) ||
                        CloudOdsAdminApp.SystemReservedClaimSets.Contains(claimSetToDelete.ClaimSetName))
            {
                throw new AdminAppException($"Claim set({claimSetToDelete.ClaimSetName}) is system reserved.Can not be deleted.");
            }

            var resourceClaimsForClaimSetId =
                _context.ClaimSetResourceClaims.Where(x => x.ClaimSet.ClaimSetId == claimSet.Id).ToList();
            _context.ClaimSetResourceClaims.RemoveRange(resourceClaimsForClaimSetId);
            _context.ClaimSets.Remove(claimSetToDelete);
            _context.SaveChanges();
        }
    }
}
