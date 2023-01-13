// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor;

public class OverrideDefaultAuthorizationStrategyV6Service
{
    private readonly ISecurityContext _context;

    public OverrideDefaultAuthorizationStrategyV6Service(ISecurityContext context)
    {
        _context = context;
    }

    public void Execute(IOverrideDefaultAuthorizationStrategyModel model)
    {
        var claimSetResourceClaimsToEdit = _context.ClaimSetResourceClaimActions
            .Include(x => x.ResourceClaim)
            .Include(x => x.Action)
            .Include(x => x.ClaimSet)
            .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
            .Where(
                x => x.ResourceClaim.ResourceClaimId == model.ResourceClaimId &&
                     x.ClaimSet.ClaimSetId == model.ClaimSetId)
            .ToList();

        var parentResourceClaimId = _context.ResourceClaims
            .Single(x => x.ResourceClaimId == model.ResourceClaimId).ParentResourceClaimId;

        var parentResourceClaims = new List<ClaimSetResourceClaimAction>();

        if (parentResourceClaimId != null)
        {
            parentResourceClaims = _context.ClaimSetResourceClaimActions
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.ClaimSet)
                .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
                .Where(
                    x => x.ResourceClaim.ResourceClaimId == parentResourceClaimId &&
                         x.ClaimSet.ClaimSetId == model.ClaimSetId)
                .ToList();
        }

        var authorizationStrategiesDictionary =
            new Dictionary<int, EdFi.Security.DataAccess.Models.AuthorizationStrategy>();

        foreach (var authStrategy in _context.AuthorizationStrategies.ToList())
        {
            authorizationStrategiesDictionary[authStrategy.AuthorizationStrategyId] = authStrategy;
        }

        claimSetResourceClaimsToEdit = RemoveOverrides(model, claimSetResourceClaimsToEdit);

        AddOverrides(model, claimSetResourceClaimsToEdit, authorizationStrategiesDictionary, parentResourceClaims);

        _context.SaveChanges();
    }

    private List<ClaimSetResourceClaimAction> RemoveOverrides(
        IOverrideDefaultAuthorizationStrategyModel model,
        IEnumerable<ClaimSetResourceClaimAction> resourceClaimsToEdit)
    {
        var claimSetResourceClaims = resourceClaimsToEdit.ToList();

        foreach (var claimSetResourceClaim in claimSetResourceClaims)
        {
            if (claimSetResourceClaim.Action.ActionName == Action.Create.Value &&
                model.AuthorizationStrategyForCreate == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverrides = null;
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value &&
                     model.AuthorizationStrategyForRead == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverrides = null;
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value &&
                     model.AuthorizationStrategyForUpdate == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverrides = null;
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value &&
                     model.AuthorizationStrategyForDelete == 0)
            {
                claimSetResourceClaim.AuthorizationStrategyOverrides = null;
            }

            RemoveClaimSetResourceClaimActionAuthorizationStrategyOverrides(claimSetResourceClaim);
        }

        void RemoveClaimSetResourceClaimActionAuthorizationStrategyOverrides(ClaimSetResourceClaimAction claimSetResourceClaimAction)
        {
            var existingAuthOverride =
                _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.FirstOrDefault(
                    x => x.ClaimSetResourceClaimActionId ==
                         claimSetResourceClaimAction.ClaimSetResourceClaimActionId);

            if (existingAuthOverride != null)
            {
                _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.Remove(
                    existingAuthOverride);
            }
        }

        return claimSetResourceClaims;
    }

    private static void AddOverrides(IOverrideDefaultAuthorizationStrategyModel model,
        IEnumerable<ClaimSetResourceClaimAction> resourceClaimsToEdit,
        Dictionary<int, EdFi.Security.DataAccess.Models.AuthorizationStrategy> authorizationStrategiesDictionary,
        List<ClaimSetResourceClaimAction> parentResourceClaims)
    {
        var claimSetResourceClaims = resourceClaimsToEdit.ToList();

        foreach (var claimSetResourceClaim in claimSetResourceClaims)
        {

            if (claimSetResourceClaim.Action.ActionName == Action.Create.Value &&
                model.AuthorizationStrategyForCreate != 0)
            {
                SetAuthorizationStrategyOverrides(
                    claimSetResourceClaim, parentResourceClaims, model.AuthorizationStrategyForCreate,
                    authorizationStrategiesDictionary, Action.Create.Value);
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Read.Value &&
                     model.AuthorizationStrategyForRead != 0)
            {
                SetAuthorizationStrategyOverrides(
                    claimSetResourceClaim, parentResourceClaims, model.AuthorizationStrategyForRead,
                    authorizationStrategiesDictionary, Action.Read.Value);
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Update.Value &&
                     model.AuthorizationStrategyForUpdate != 0)
            {
                SetAuthorizationStrategyOverrides(
                    claimSetResourceClaim, parentResourceClaims, model.AuthorizationStrategyForUpdate,
                    authorizationStrategiesDictionary, Action.Update.Value);
            }
            else if (claimSetResourceClaim.Action.ActionName == Action.Delete.Value &&
                     model.AuthorizationStrategyForDelete != 0)
            {
                SetAuthorizationStrategyOverrides(
                    claimSetResourceClaim, parentResourceClaims, model.AuthorizationStrategyForDelete,
                    authorizationStrategiesDictionary, Action.Delete.Value);
            }
        }
    }

    private static void SetAuthorizationStrategyOverrides(
        ClaimSetResourceClaimAction claimSetResourceClaimAction,
        List<ClaimSetResourceClaimAction> parentResourceClaims, int authorizationStrategyValue,
        Dictionary<int, EdFi.Security.DataAccess.Models.AuthorizationStrategy>
            authorizationStrategiesDictionary, string actionName)
    {
        var authStrategyOverrides = new List<ClaimSetResourceClaimActionAuthorizationStrategyOverrides>
        {
            new() { AuthorizationStrategy = authorizationStrategiesDictionary[authorizationStrategyValue] }
        };

        if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(
                x =>
                    x.Action.ActionName == actionName && x.AuthorizationStrategyOverrides != null &&
                    x.AuthorizationStrategyOverrides.Any() &&
                    x.AuthorizationStrategyOverrides.SingleOrDefault()?.AuthorizationStrategyId ==
                    authorizationStrategyValue) == null)
        {
            claimSetResourceClaimAction.AuthorizationStrategyOverrides = authStrategyOverrides;
        }
        else if (!parentResourceClaims.Any())
        {
            claimSetResourceClaimAction.AuthorizationStrategyOverrides = authStrategyOverrides;
        }
    }
}
