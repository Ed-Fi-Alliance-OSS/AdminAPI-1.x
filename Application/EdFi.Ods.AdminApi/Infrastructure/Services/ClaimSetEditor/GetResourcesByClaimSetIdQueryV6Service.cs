// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.Extensions;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;
using SecurityResourceClaim = EdFi.Security.DataAccess.Models.ResourceClaim;
using SecurityAuthorizationStrategy = EdFi.Security.DataAccess.Models.AuthorizationStrategy;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor.Extensions;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor
{
    public class GetResourcesByClaimSetIdQueryV6Service
    {
        private readonly ISecurityContext _securityContext;
        private readonly IMapper _mapper;

        public GetResourcesByClaimSetIdQueryV6Service(ISecurityContext securityContext, IMapper mapper)
        {
            _securityContext = securityContext;
            _mapper = mapper;
        }

        internal static void AddChildResourcesToParents(IReadOnlyList<ResourceClaim> childResources, IList<ResourceClaim> parentResources)
        {
            foreach (var childResource in childResources)
            {
                var parentResource = parentResources.SingleOrDefault(x => x.Id == childResource.ParentId);
                if (parentResource != null)
                    parentResource.Children.Add(childResource);
                else
                {
                    parentResources.Add(childResource);
                }
            }
        }

        internal IList<ResourceClaim> GetParentResources(int claimSetId)
        {
            var dbParentResources = _securityContext.ClaimSetResourceClaimActions
                .Include(x => x.ResourceClaim)
                .Include(x => x.ResourceClaim.ParentResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy))
                .Where(x => x.ClaimSet.ClaimSetId == claimSetId
                            && x.ResourceClaim.ParentResourceClaimId == null).ToList();

            var defaultAuthStrategies = GetDefaultAuthStrategies(dbParentResources.Select(x => x.ResourceClaim).ToList());
            var authStrategyOverrides = GetAuthStrategyOverrides(dbParentResources.ToList());

            var parentResources = dbParentResources.GroupBy(x => x.ResourceClaim).Select(x => new ResourceClaim
            {
                Id = x.Key.ResourceClaimId,
                Name = x.Key.ResourceName,
                Create = x.Any(a => a.Action.ActionName == Action.Create.Value),
                Read = x.Any(a => a.Action.ActionName == Action.Read.Value),
                Update = x.Any(a => a.Action.ActionName == Action.Update.Value),
                Delete = x.Any(a => a.Action.ActionName == Action.Delete.Value),
                ReadChanges = x.Any(a => a.Action.ActionName == Action.ReadChanges.Value),
                IsParent = true,
                DefaultAuthStrategiesForCRUD = defaultAuthStrategies[x.Key.ResourceClaimId].ToArray(),
                AuthStrategyOverridesForCRUD = authStrategyOverrides[x.Key.ResourceClaimId].ToArray()
            })
                .ToList();

            parentResources.ForEach(x => x.Children = new List<ResourceClaim>());
            return parentResources;
        }

        private Dictionary<int, ClaimSetResourceClaimActionAuthStrategies?[]> GetDefaultAuthStrategies(IReadOnlyCollection<SecurityResourceClaim> resourceClaims)
        {
            var resultDictionary = new Dictionary<int, ClaimSetResourceClaimActionAuthStrategies?[]>();

            var defaultAuthStrategies = _securityContext.ResourceClaimActions
                .Include(x => x.ResourceClaim).Include(x => x.Action).Include(x => x.AuthorizationStrategies.Select(x => x.AuthorizationStrategy)).ToList();

            var defaultAuthStrategiesForParents = defaultAuthStrategies
                .Where(x => x.ResourceClaim.ParentResourceClaimId == null).ToList();

            var defaultAuthStrategiesForChildren = defaultAuthStrategies
                .Where(x => x.ResourceClaim.ParentResourceClaimId != null).ToList();

            foreach (var resourceClaim in resourceClaims)
            {
                var actions = new List<ClaimSetResourceClaimActionAuthStrategies?>();
                if (resourceClaim.ParentResourceClaimId == null)
                {
                    var createDefaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                            x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                            x.Action.ActionName == Action.Create.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions.Add(new ClaimSetResourceClaimActionAuthStrategies() { AuthorizationStrategies = new List<AuthorizationStrategy?> { _mapper.Map<AuthorizationStrategy>(createDefaultStrategy) } });
                    var readDefaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Read.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions.Add(new ClaimSetResourceClaimActionAuthStrategies() { AuthorizationStrategies = new List<AuthorizationStrategy?> { _mapper.Map<AuthorizationStrategy>(readDefaultStrategy) } });
                    var updateDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Update.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions.Add(new ClaimSetResourceClaimActionAuthStrategies() { AuthorizationStrategies = new List<AuthorizationStrategy?> { _mapper.Map<AuthorizationStrategy>(updateDefaultStrategy) } });
                    var deleteDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Delete.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions.Add(new ClaimSetResourceClaimActionAuthStrategies() { AuthorizationStrategies = new List<AuthorizationStrategy?> { _mapper.Map<AuthorizationStrategy>(deleteDefaultStrategy) } });
                    var readChangesDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.ReadChanges.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions.Add(new ClaimSetResourceClaimActionAuthStrategies() { AuthorizationStrategies = new List<AuthorizationStrategy?> { _mapper.Map<AuthorizationStrategy>(readChangesDefaultStrategy) } });
                }
                else
                {
                    var createDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Create.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(createDefaultStrategy, Action.Create);

                    var readDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Read.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(readDefaultStrategy, Action.Read);

                    var updateDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Update.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(updateDefaultStrategy, Action.Update);

                    var deleteDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Delete.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(deleteDefaultStrategy, Action.Delete);

                    var readChangesDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.ReadChanges.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(readChangesDefaultStrategy, Action.ReadChanges);

                    List<ClaimSetResourceClaimActionAuthStrategies?> AddStrategyToChildResource(SecurityAuthorizationStrategy? defaultStrategy, Action action)
                    {
                        if (defaultStrategy == null)
                        {
                            defaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                                       x.ResourceClaim.ResourceClaimId == resourceClaim.ParentResourceClaimId &&
                                       x.Action.ActionName == action.Value)?.AuthorizationStrategies.SingleOrDefault()?.AuthorizationStrategy;
                            var mappedStrategy = _mapper.Map<AuthorizationStrategy>(defaultStrategy);
                            if (mappedStrategy != null) mappedStrategy.IsInheritedFromParent = true;

                            actions.Add(new ClaimSetResourceClaimActionAuthStrategies()
                            {
                                AuthorizationStrategies = new List<AuthorizationStrategy> { _mapper.Map<AuthorizationStrategy>(mappedStrategy) }.ToArray()
                            });
                        }
                        else
                        {
                            actions.Add(new ClaimSetResourceClaimActionAuthStrategies()
                            {
                                AuthorizationStrategies = new List<AuthorizationStrategy> { _mapper.Map<AuthorizationStrategy>(defaultStrategy) }.ToArray()
                            });
                        }

                        return actions;
                    }
                }

                resultDictionary[resourceClaim.ResourceClaimId] = actions.Where(x => x != null).ToArray() as ClaimSetResourceClaimActionAuthStrategies[];
            }

            return resultDictionary;
        }

        private Dictionary<int, ClaimSetResourceClaimActionAuthStrategies?[]> GetAuthStrategyOverrides(List<ClaimSetResourceClaimAction> resourceClaims)
        {
            var resultDictionary = new Dictionary<int, ClaimSetResourceClaimActionAuthStrategies?[]>();
            resourceClaims =
                new List<ClaimSetResourceClaimAction>(resourceClaims.OrderBy(i => new List<string> { Action.Create.Value, Action.Read.Value, Action.Update.Value, Action.Delete.Value, Action.ReadChanges.Value }.IndexOf(i.Action.ActionName)));
            foreach (var resourceClaim in resourceClaims)
            {
                List<AuthorizationStrategy>? authStrategies = null;
                if (resourceClaim.ResourceClaim.ParentResourceClaim == null)
                {
                    authStrategies = _mapper.Map<List<AuthorizationStrategy>>(resourceClaim.AuthorizationStrategyOverrides is not null && resourceClaim.AuthorizationStrategyOverrides.Any() ?
                        resourceClaim.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy) : null);
                }
                else
                {
                    var parentResources = _securityContext.ClaimSetResourceClaimActions
                        .Include(x => x.ResourceClaim)
                        .Include(x => x.ClaimSet)
                        .Include(x => x.Action)
                        .Include(x => x.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy)).ToList();
                    var parentResourceOverride = parentResources.SingleOrDefault(x => x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaim.ParentResourceClaimId
                                                                                               && x.ClaimSet.ClaimSetId == resourceClaim.ClaimSet.ClaimSetId
                                                                                               && x.Action.ActionId == resourceClaim.Action.ActionId);
                    if (parentResourceOverride?.AuthorizationStrategyOverrides != null && parentResourceOverride.AuthorizationStrategyOverrides.Any())
                    {
                        authStrategies =
                            _mapper.Map<List<AuthorizationStrategy>>(parentResourceOverride.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy));
                        if (authStrategies != null)
                        {
                            authStrategies.ForEach(a => a.IsInheritedFromParent = true);
                        }
                    }

                    if (resourceClaim.AuthorizationStrategyOverrides != null && resourceClaim.AuthorizationStrategyOverrides.Any())
                    {
                        authStrategies = _mapper.Map<List<AuthorizationStrategy>>(resourceClaim.AuthorizationStrategyOverrides.Select(x => x.AuthorizationStrategy));
                    }
                }

                if (resultDictionary.ContainsKey(resourceClaim.ResourceClaim.ResourceClaimId))
                {
                    if (authStrategies != null)
                    {
                        foreach (var authStrategy in authStrategies)
                        {
                            resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId].AddAuthorizationStrategyOverrides(resourceClaim.Action.ActionName, authStrategy);
                        }
                    }
                }
                else
                {
                    if (authStrategies != null)
                    {
                        resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId] = new List<ClaimSetResourceClaimActionAuthStrategies?>() {
                        null, null, null, null, null }.ToArray();
                        foreach (var authStrategy in authStrategies)
                        {
                            resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId].AddAuthorizationStrategyOverrides(resourceClaim.Action.ActionName, authStrategy);
                        }
                    }
                }
            }
            return resultDictionary;
        }

        internal IReadOnlyList<ResourceClaim> GetChildResources(int claimSetId)
        {
            var dbChildResources =
                _securityContext.ClaimSetResourceClaimActions
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.AuthorizationStrategyOverrides)
                .Where(x => x.ClaimSet.ClaimSetId == claimSetId
                            && x.ResourceClaim.ParentResourceClaimId != null).ToList();
            var defaultAuthStrategies = GetDefaultAuthStrategies(dbChildResources.Select(x => x.ResourceClaim).ToList());
            var authStrategyOverrides = GetAuthStrategyOverrides(dbChildResources.ToList());

            var childResources = dbChildResources.GroupBy(x => x.ResourceClaim)
                .Select(x => new ResourceClaim
                {
                    Id = x.Key.ResourceClaimId,
                    ParentId = x.Key.ParentResourceClaimId ?? 0,
                    Name = x.Key.ResourceName,
                    Create = x.Any(a => a.Action.ActionName == Action.Create.Value),
                    Read = x.Any(a => a.Action.ActionName == Action.Read.Value),
                    Update = x.Any(a => a.Action.ActionName == Action.Update.Value),
                    Delete = x.Any(a => a.Action.ActionName == Action.Delete.Value),
                    ReadChanges = x.Any(a => a.Action.ActionName == Action.ReadChanges.Value),
                    IsParent = false,
                    DefaultAuthStrategiesForCRUD = defaultAuthStrategies[x.Key.ResourceClaimId],
                    AuthStrategyOverridesForCRUD = authStrategyOverrides.Keys.Any(p => p == x.Key.ResourceClaimId) ? authStrategyOverrides[x.Key.ResourceClaimId] : Array.Empty<ClaimSetResourceClaimActionAuthStrategies>(),
                })
                .ToList();
            return childResources;
        }
    }
}


