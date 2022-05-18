// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class DeleteClaimSetCommand
    {
        private readonly ISecurityContext _context;

        public DeleteClaimSetCommand(ISecurityContext context)
        {
            _context = context;
        }

        public void Execute(IDeleteClaimSetModel claimSet)
        {
            var claimSetToDelete = _context.ClaimSets.Single(x => x.ClaimSetId == claimSet.Id);
            var resourceClaimsForClaimSetId =
                _context.ClaimSetResourceClaims.Where(x => x.ClaimSet.ClaimSetId == claimSet.Id).ToList();
            _context.ClaimSetResourceClaims.RemoveRange(resourceClaimsForClaimSetId);
            _context.ClaimSets.Remove(claimSetToDelete);
            _context.SaveChanges();
        }
    }

    public interface IDeleteClaimSetModel
    {
        string Name { get; }
        int Id { get; }
    }
}
