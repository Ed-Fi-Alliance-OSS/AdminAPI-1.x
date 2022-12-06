﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

extern alias SecurityDataAccess53;

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SecurityDataAccess53::EdFi.Security.DataAccess.Contexts;
using SecurityDataAccess53::EdFi.Security.DataAccess.Models;

using SecurityAuthorizationStrategy = SecurityDataAccess53::EdFi.Security.DataAccess.Models.AuthorizationStrategy;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class OverrideDefaultAuthorizationStrategyCommand
    {
        private readonly ISecurityContext _context;

        public OverrideDefaultAuthorizationStrategyCommand(ISecurityContext context)
        {
            _context = context;
        }

        public void Execute(IOverrideDefaultAuthorizationStrategyModel model)
        {
            var claimSetResourceClaimsToEdit = _context.ClaimSetResourceClaims
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.ClaimSet)
                .Include(x => x.AuthorizationStrategyOverride)
                .Where(x => x.ResourceClaim.ResourceClaimId == model.ResourceClaimId && x.ClaimSet.ClaimSetId == model.ClaimSetId)
                .ToList();

            var parentResourceClaimId = _context.ResourceClaims
                .Single(x => x.ResourceClaimId == model.ResourceClaimId).ParentResourceClaimId;
            var parentResourceClaims = new List<ClaimSetResourceClaim>();

            if (parentResourceClaimId != null)
            {
                parentResourceClaims = _context.ClaimSetResourceClaims
                    .Include(x => x.ResourceClaim)
                    .Include(x => x.Action)
                    .Include(x => x.ClaimSet)
                    .Include(x => x.AuthorizationStrategyOverride)
                    .Where(x => x.ResourceClaim.ResourceClaimId == parentResourceClaimId && x.ClaimSet.ClaimSetId == model.ClaimSetId)
                    .ToList();
            }

            var authorizationStrategiesDictionary = new Dictionary<int, SecurityAuthorizationStrategy>();
            foreach (var authStrategy in _context.AuthorizationStrategies.ToList())
            {
                authorizationStrategiesDictionary[authStrategy.AuthorizationStrategyId] = authStrategy;
            }

            claimSetResourceClaimsToEdit = RemoveOverrides(model, claimSetResourceClaimsToEdit);

            AddOverrides(model, claimSetResourceClaimsToEdit, authorizationStrategiesDictionary, parentResourceClaims);

            _context.SaveChanges();
        }

        private List<ClaimSetResourceClaim> RemoveOverrides(IOverrideDefaultAuthorizationStrategyModel model, IEnumerable<ClaimSetResourceClaim> resourceClaimsToEdit)
        {
            var claimSetResourceClaims = resourceClaimsToEdit.ToList();
            foreach (var claimSetResourceClaim in claimSetResourceClaims)
            {
                if (claimSetResourceClaim.Action.ActionName == Action.Create.Value && model.AuthorizationStrategyForCreate == 0)
                {
                    claimSetResourceClaim.AuthorizationStrategyOverride = null;
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value && model.AuthorizationStrategyForRead == 0)
                {
                    claimSetResourceClaim.AuthorizationStrategyOverride = null;
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value && model.AuthorizationStrategyForUpdate == 0)
                {
                    claimSetResourceClaim.AuthorizationStrategyOverride = null;
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value && model.AuthorizationStrategyForDelete == 0)
                {
                    claimSetResourceClaim.AuthorizationStrategyOverride = null;
                }
            }

            return claimSetResourceClaims;
        }

        private static void AddOverrides(IOverrideDefaultAuthorizationStrategyModel model,
            IEnumerable<ClaimSetResourceClaim> resourceClaimsToEdit,
            Dictionary<int, SecurityAuthorizationStrategy> authorizationStrategiesDictionary,
            List<ClaimSetResourceClaim> parentResourceClaims)
        {
            var claimSetResourceClaims = resourceClaimsToEdit.ToList();
            foreach (var claimSetResourceClaim in claimSetResourceClaims)
            {

                if (claimSetResourceClaim.Action.ActionName == Action.Create.Value && model.AuthorizationStrategyForCreate != 0)
                {
                    if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                            x.Action.ActionName == Action.Create.Value && x.AuthorizationStrategyOverride != null &&
                            x.AuthorizationStrategyOverride.AuthorizationStrategyId == model.AuthorizationStrategyForCreate) == null)
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForCreate];
                    }
                    else if (!parentResourceClaims.Any())
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForCreate];
                    }
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value && model.AuthorizationStrategyForRead != 0)
                {
                    if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                            x.Action.ActionName == Action.Read.Value && x.AuthorizationStrategyOverride != null &&
                            x.AuthorizationStrategyOverride.AuthorizationStrategyId == model.AuthorizationStrategyForRead) == null)
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForRead];
                    }
                    else if (!parentResourceClaims.Any())
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForRead];
                    }
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value && model.AuthorizationStrategyForUpdate != 0)
                {
                    if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                            x.Action.ActionName == Action.Update.Value && x.AuthorizationStrategyOverride != null &&
                            x.AuthorizationStrategyOverride.AuthorizationStrategyId == model.AuthorizationStrategyForUpdate) == null)
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForUpdate];
                    }
                    else if (!parentResourceClaims.Any())
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForUpdate];
                    }
                }
                else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value && model.AuthorizationStrategyForDelete != 0)
                {
                    if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                            x.Action.ActionName == Action.Delete.Value && x.AuthorizationStrategyOverride != null &&
                            x.AuthorizationStrategyOverride.AuthorizationStrategyId == model.AuthorizationStrategyForDelete) == null)
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForDelete];
                    }
                    else if (!parentResourceClaims.Any())
                    {
                        claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[model.AuthorizationStrategyForDelete];
                    }
                }
            }
        }
    }

    public interface IOverrideDefaultAuthorizationStrategyModel
    {
        int ClaimSetId { get; }
        int ResourceClaimId { get; }
        int AuthorizationStrategyForCreate { get; }
        int AuthorizationStrategyForRead { get; }
        int AuthorizationStrategyForUpdate { get; }
        int AuthorizationStrategyForDelete { get; }
    }
}
