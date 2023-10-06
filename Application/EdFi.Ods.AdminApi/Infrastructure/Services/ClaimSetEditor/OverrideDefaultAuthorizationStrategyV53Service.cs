// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.SecurityCompatiblity53.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class OverrideDefaultAuthorizationStrategyV53Service
{
    private readonly ISecurityContext _context;

    public OverrideDefaultAuthorizationStrategyV53Service(ISecurityContext context)
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

        var authorizationStrategiesDictionary = new Dictionary<int, SecurityCompatiblity53.DataAccess.Models.AuthorizationStrategy>();
        foreach (var authStrategy in _context.AuthorizationStrategies.ToList())
        {
            authorizationStrategiesDictionary[authStrategy.AuthorizationStrategyId] = authStrategy;
        }

        claimSetResourceClaimsToEdit = RemoveOverrides(model, claimSetResourceClaimsToEdit);

        AddOverrides(model, claimSetResourceClaimsToEdit, authorizationStrategiesDictionary, parentResourceClaims);

        _context.SaveChanges();
    }

    private static List<ClaimSetResourceClaim> RemoveOverrides(IOverrideDefaultAuthorizationStrategyModel model, IEnumerable<ClaimSetResourceClaim> resourceClaimsToEdit)
    {
        var claimSetResourceClaims = resourceClaimsToEdit.ToList();
        foreach (var claimSetResourceClaim in claimSetResourceClaims)
        {
            if (claimSetResourceClaim.Action.ActionName == Action.Create.Value && model.AuthorizationStrategyForCreate.Length == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverride = null;
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value && model.AuthorizationStrategyForRead.Length == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverride = null;
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value && model.AuthorizationStrategyForUpdate.Length == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverride = null;
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value && model.AuthorizationStrategyForDelete.Length == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverride = null;
            }
        }

        return claimSetResourceClaims;
    }

    private static void AddOverrides(IOverrideDefaultAuthorizationStrategyModel model,
        IEnumerable<ClaimSetResourceClaim> resourceClaimsToEdit,
        Dictionary<int, SecurityCompatiblity53.DataAccess.Models.AuthorizationStrategy> authorizationStrategiesDictionary,
        List<ClaimSetResourceClaim> parentResourceClaims)
    {
        var claimSetResourceClaims = resourceClaimsToEdit.ToList();
        foreach (var claimSetResourceClaim in claimSetResourceClaims)
        {

            if (claimSetResourceClaim.Action.ActionName == Action.Create.Value && model.AuthorizationStrategyForCreate.Length != 0)
            {
                foreach (var authStrategyId in model.AuthorizationStrategyForCreate)
                {
                    if (authStrategyId != 0)
                    {
                        if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                            x.Action.ActionName == Action.Create.Value && x.AuthorizationStrategyOverride != null &&
                            x.AuthorizationStrategyOverride.AuthorizationStrategyId == authStrategyId) == null)
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                        else if (!parentResourceClaims.Any())
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                    }
                }
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value && model.AuthorizationStrategyForRead.Length != 0)
            {
                foreach (var authStrategyId in model.AuthorizationStrategyForRead)
                {
                    if (authStrategyId != 0)
                    {
                        if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                        x.Action.ActionName == Action.Read.Value && x.AuthorizationStrategyOverride != null &&
                        x.AuthorizationStrategyOverride.AuthorizationStrategyId == authStrategyId) == null)
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                        else if (!parentResourceClaims.Any())
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                    }
                }
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value && model.AuthorizationStrategyForUpdate.Length != 0)
            {
                foreach (var authStrategyId in model.AuthorizationStrategyForUpdate)
                {
                    if (authStrategyId != 0)
                    {
                        if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                        x.Action.ActionName == Action.Update.Value && x.AuthorizationStrategyOverride != null &&
                        x.AuthorizationStrategyOverride.AuthorizationStrategyId == authStrategyId) == null)
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                        else if (!parentResourceClaims.Any())
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                    }
                }
                    
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value && model.AuthorizationStrategyForDelete.Length != 0)
            {
                foreach (var authStrategyId in model.AuthorizationStrategyForDelete)
                {
                    if (authStrategyId != 0)
                    {
                        if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(x =>
                        x.Action.ActionName == Action.Delete.Value && x.AuthorizationStrategyOverride != null &&
                        x.AuthorizationStrategyOverride.AuthorizationStrategyId == authStrategyId) == null)
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                        else if (!parentResourceClaims.Any())
                        {
                            claimSetResourceClaim.AuthorizationStrategyOverride = authorizationStrategiesDictionary[authStrategyId];
                        }
                    }
                }
                
            }
        }
    }
}
