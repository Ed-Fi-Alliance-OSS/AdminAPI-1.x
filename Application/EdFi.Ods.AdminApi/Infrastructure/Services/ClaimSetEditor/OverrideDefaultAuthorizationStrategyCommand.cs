// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class OverrideDefaultAuthorizationStrategyCommand
{
    private readonly ISecurityContext _context;

    public OverrideDefaultAuthorizationStrategyCommand(ISecurityContext context)
    {
        _context = context;
    }

    public void Execute(IOverrideDefaultAuthorizationStrategyModel model)
    {
        var claimSetResourceClaimsToEdit = GetClaimSetResourceClaimsToEdit(model.ClaimSetId, model.ResourceClaimId);
        var parentResourceClaims = GetParentResourceClaims(model.ClaimSetId, model.ResourceClaimId);

        var authorizationStrategiesDictionary = GetAuthorizationStrategiesAsDictionary();

        claimSetResourceClaimsToEdit = RemoveOverrides(model, claimSetResourceClaimsToEdit);

        AddOverrides(model, claimSetResourceClaimsToEdit, authorizationStrategiesDictionary, parentResourceClaims);

        _context.SaveChanges();
    }


    public void ExecuteOnSpecificAction(OverrideAuthStrategyOnClaimSetModel model)
    {
        var claimSetResourceClaimsToEdit = GetClaimSetResourceClaimsToEdit(model.ClaimSetId, model.ResourceClaimId);

        var claimSetResourceClaimAction = claimSetResourceClaimsToEdit.FirstOrDefault(rc => rc.ResourceClaimId == model.ResourceClaimId && rc.Action.ActionName.ToLower() == model.ActionName!.ToLower());

        if (claimSetResourceClaimAction != null)
        {
            if (!claimSetResourceClaimAction!.AuthorizationStrategyOverrides.Any(rc => rc.ClaimSetResourceClaimAction.Action.ActionName.ToLower() == model.ActionName!.ToLower()))
            {
                claimSetResourceClaimAction.AuthorizationStrategyOverrides.Add(new ClaimSetResourceClaimActionAuthorizationStrategyOverrides()
                {
                    AuthorizationStrategyId = model.AuthStrategyId,
                    AuthorizationStrategy = _context.AuthorizationStrategies.First(p => p.AuthorizationStrategyId == model.AuthStrategyId),
                    ClaimSetResourceClaimActionId = claimSetResourceClaimAction.ClaimSetResourceClaimActionId
                });
            }
            else
            {
                var authStrategyOverride = claimSetResourceClaimAction.AuthorizationStrategyOverrides.FirstOrDefault(rc => rc.ClaimSetResourceClaimAction.Action.ActionName.ToLower() == model.ActionName!.ToLower());
                if (authStrategyOverride != null)
                {
                    authStrategyOverride.AuthorizationStrategyId = model.AuthStrategyId;
                }
            }
            _context.SaveChanges();
        }
    }

    public void ResetAuthorizationStrategyOverrides(OverrideAuthStrategyOnClaimSetModel model)
    {
        var claimSetResourceClaimsToEdit = GetClaimSetResourceClaimsToEdit(model.ClaimSetId, model.ResourceClaimId);

        var claimSetResourceClaimAction = claimSetResourceClaimsToEdit.FirstOrDefault(rc => rc.ResourceClaimId == model.ResourceClaimId);
        if (claimSetResourceClaimAction!.AuthorizationStrategyOverrides.Any())
        {
            _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.RemoveRange(claimSetResourceClaimAction.AuthorizationStrategyOverrides.ToList());
        }
        _context.SaveChanges();
    }

    private Dictionary<int, EdFi.Security.DataAccess.Models.AuthorizationStrategy> GetAuthorizationStrategiesAsDictionary()
    {
        var authorizationStrategiesDictionary =
            new Dictionary<int, EdFi.Security.DataAccess.Models.AuthorizationStrategy>();

        foreach (var authStrategy in _context.AuthorizationStrategies.ToList())
        {
            authorizationStrategiesDictionary[authStrategy.AuthorizationStrategyId] = authStrategy;
        }

        return authorizationStrategiesDictionary;
    }

    private List<ClaimSetResourceClaimAction> GetParentResourceClaims(int claimSetId, int resourceClaimId)
    {
        var parentResourceClaims = new List<ClaimSetResourceClaimAction>();
        var parentResourceClaimId = _context.ResourceClaims
                    .Single(x => x.ResourceClaimId == resourceClaimId).ParentResourceClaimId;
        if (parentResourceClaimId != null)
        {
            parentResourceClaims = _context.ClaimSetResourceClaimActions
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.ClaimSet)
                .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
                .Where(
                    x => x.ResourceClaim.ResourceClaimId == parentResourceClaimId &&
                         x.ClaimSet.ClaimSetId == claimSetId)
                .ToList();
        }

        return parentResourceClaims;
    }

    private List<ClaimSetResourceClaimAction> GetClaimSetResourceClaimsToEdit(int claimSetId, int resourceClaimId)
    {
        return _context.ClaimSetResourceClaimActions
                     .Include(x => x.ResourceClaim)
                     .Include(x => x.Action)
                     .Include(x => x.ClaimSet)
                     .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
                     .Where(
                         x => x.ResourceClaim.ResourceClaimId == resourceClaimId &&
                              x.ClaimSet.ClaimSetId == claimSetId)
                     .ToList();
    }

    private List<ClaimSetResourceClaimAction> RemoveOverrides(
       IOverrideDefaultAuthorizationStrategyModel model,
       IEnumerable<ClaimSetResourceClaimAction> resourceClaimsToEdit)
    {
        var claimSetResourceClaims = resourceClaimsToEdit.ToList();

        foreach (var claimSetResourceClaim in claimSetResourceClaims)
        {
            var actionAuthStrategiesToOverride = model.ClaimSetResourceClaimActionAuthStrategyOverrides?.Where(x =>
            x != null && x.ActionName.Equals(claimSetResourceClaim.Action.ActionName, StringComparison.InvariantCultureIgnoreCase));

            if (actionAuthStrategiesToOverride != null && actionAuthStrategiesToOverride.Any())
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
            var actionAuthStrategiesToOverride = model.ClaimSetResourceClaimActionAuthStrategyOverrides?.FirstOrDefault(x =>
               x != null && x.ActionId == claimSetResourceClaim.ActionId);

            List<ClaimSetResourceClaimActionAuthorizationStrategyOverrides>? authStrategyOverrides = null;

            if (actionAuthStrategiesToOverride != null)
            {
                authStrategyOverrides = new List<ClaimSetResourceClaimActionAuthorizationStrategyOverrides>();
                if (actionAuthStrategiesToOverride.AuthorizationStrategies != null)
                    foreach (var actionAuthOverride in actionAuthStrategiesToOverride.AuthorizationStrategies)
                    {
                        authStrategyOverrides.Add(new()
                        {
                            AuthorizationStrategy = authorizationStrategiesDictionary[actionAuthOverride.AuthStrategyId]
                        });
                    }

                if (parentResourceClaims.Any() && parentResourceClaims.SingleOrDefault(
                    x => x.Action.ActionId == claimSetResourceClaim.ActionId
                    && x.AuthorizationStrategyOverrides != null &&
                        x.AuthorizationStrategyOverrides.Any(a =>
                        authStrategyOverrides.Select(c => c.AuthorizationStrategyId).
                        Contains(a.AuthorizationStrategyId))) == null)
                {
                    claimSetResourceClaim.AuthorizationStrategyOverrides = authStrategyOverrides;
                }
                else if (!parentResourceClaims.Any())
                {
                    claimSetResourceClaim.AuthorizationStrategyOverrides = authStrategyOverrides;
                }
            }
        }
    }
}
  

public interface IOverrideDefaultAuthorizationStrategyModel
{
    int ClaimSetId { get; }
    int ResourceClaimId { get; }
    List<ClaimSetResourceClaimActionAuthStrategies?>? ClaimSetResourceClaimActionAuthStrategyOverrides { get; }
}

public class OverrideAuthStrategyOnClaimSetModel 
{
    [SwaggerExclude]
    public int ClaimSetId { get; set; }
    [SwaggerExclude]
    public int ResourceClaimId { get; set; }
    public string? ActionName { get; set; }
    [SwaggerExclude]
    public int AuthStrategyId { get; set; }
}

