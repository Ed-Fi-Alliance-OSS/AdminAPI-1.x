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
    public class EditResourceOnClaimSetCommand
    {
        private readonly ISecurityContext _context;

        public EditResourceOnClaimSetCommand(ISecurityContext context)
        {
            _context = context;
        }

        public void Execute(IEditResourceOnClaimSetModel model)
        {
            var resourceClaimToEdit = model.ResourceClaim;

            var claimSetToEdit = _context.ClaimSets.Single(x => x.ClaimSetId == model.ClaimSetId);

            var claimSetResourceClaimsToEdit = _context.ClaimSetResourceClaims
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.ClaimSet)
                .Where(x => x.ResourceClaim.ResourceClaimId == resourceClaimToEdit.Id && x.ClaimSet.ClaimSetId == claimSetToEdit.ClaimSetId)
                .ToList();

            AddEnabledActionsToClaimSet(resourceClaimToEdit, claimSetResourceClaimsToEdit, claimSetToEdit);

            RemoveDisabledActionsFromClaimSet(resourceClaimToEdit, claimSetResourceClaimsToEdit);

            _context.SaveChanges();
        }

        private void RemoveDisabledActionsFromClaimSet(ResourceClaim modelResourceClaim, IEnumerable<ClaimSetResourceClaim> resourceClaimsToEdit)
        {
            var recordsToRemove = new List<ClaimSetResourceClaim>();

            foreach (var claimSetResourceClaim in resourceClaimsToEdit)
            {
                if (claimSetResourceClaim.Action.ActionName == Action.Create.Value && !modelResourceClaim.Create)
                {
                    recordsToRemove.Add(claimSetResourceClaim);
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value && !modelResourceClaim.Read)
                {
                    recordsToRemove.Add(claimSetResourceClaim);
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value && !modelResourceClaim.Update)
                {
                    recordsToRemove.Add(claimSetResourceClaim);
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value && !modelResourceClaim.Delete)
                {
                    recordsToRemove.Add(claimSetResourceClaim);
                }
            }

            if (recordsToRemove.Any())
            {
                _context.ClaimSetResourceClaims.RemoveRange(recordsToRemove);
            }
        }

        private void AddEnabledActionsToClaimSet(ResourceClaim modelResourceClaim, IReadOnlyCollection<ClaimSetResourceClaim> claimSetResourceClaimsToEdit, Security.DataAccess.Models.ClaimSet claimSetToEdit)
        {
            var actionsFromDb = _context.Actions.ToList();

            var resourceClaimFromDb = _context.ResourceClaims.Single(x => x.ResourceClaimId == modelResourceClaim.Id);

            var recordsToAdd = new List<ClaimSetResourceClaim>();

            if (modelResourceClaim.Create && claimSetResourceClaimsToEdit.All(x => x.Action.ActionName != Action.Create.Value))
            {
                recordsToAdd.Add(new ClaimSetResourceClaim
                {
                    Action = actionsFromDb.Single(x => x.ActionName == Action.Create.Value),
                    ClaimSet = claimSetToEdit,
                    ResourceClaim = resourceClaimFromDb
                });
            }

            if (modelResourceClaim.Read && claimSetResourceClaimsToEdit.All(x => x.Action.ActionName != Action.Read.Value))
            {
                recordsToAdd.Add(new ClaimSetResourceClaim
                {
                    Action = actionsFromDb.Single(x => x.ActionName == Action.Read.Value),
                    ClaimSet = claimSetToEdit,
                    ResourceClaim = resourceClaimFromDb
                });
            }

            if (modelResourceClaim.Update && claimSetResourceClaimsToEdit.All(x => x.Action.ActionName != Action.Update.Value))
            {
                recordsToAdd.Add(new ClaimSetResourceClaim
                {
                    Action = actionsFromDb.Single(x => x.ActionName == Action.Update.Value),
                    ClaimSet = claimSetToEdit,
                    ResourceClaim = resourceClaimFromDb
                });
            }

            if (modelResourceClaim.Delete && claimSetResourceClaimsToEdit.All(x => x.Action.ActionName != Action.Delete.Value))
            {
                recordsToAdd.Add(new ClaimSetResourceClaim
                {
                    Action = actionsFromDb.Single(x => x.ActionName == Action.Delete.Value),
                    ClaimSet = claimSetToEdit,
                    ResourceClaim = resourceClaimFromDb
                });
            }

            if (recordsToAdd.Any())
            {
                _context.ClaimSetResourceClaims.AddRange(recordsToAdd);
            }
        }
    }

    public interface IEditResourceOnClaimSetModel
    {
        int ClaimSetId { get; }
        ResourceClaim ResourceClaim { get; }
    }
}
