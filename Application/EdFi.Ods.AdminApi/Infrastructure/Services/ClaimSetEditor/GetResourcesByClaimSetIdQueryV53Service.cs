// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.Extensions;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor.Extensions;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.SecurityCompatiblity53.DataAccess.Models;
using System.Data.Entity;
using SecurityAuthorizationStrategy = EdFi.SecurityCompatiblity53.DataAccess.Models.AuthorizationStrategy;
using SecurityResourceClaim = EdFi.SecurityCompatiblity53.DataAccess.Models.ResourceClaim;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor
{
    public class GetResourcesByClaimSetIdQueryV53Service
    {
        private readonly ISecurityContext _securityContext;
        private readonly IMapper _mapper;

        public GetResourcesByClaimSetIdQueryV53Service(ISecurityContext securityContext, IMapper mapper)
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
            var dbParentResources = _securityContext.ClaimSetResourceClaims
                .Include(x => x.ResourceClaim)
                .Include(x => x.ResourceClaim.ParentResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.AuthorizationStrategyOverride)
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
                DefaultAuthStrategiesForCRUD = defaultAuthStrategies[x.Key.ResourceClaimId],
                AuthStrategyOverridesForCRUD = authStrategyOverrides[x.Key.ResourceClaimId].ToArray()
            }).ToList();

            parentResources.ForEach(x => x.Children = new List<ResourceClaim>());
            return parentResources;
        }

        private Dictionary<int, ClaimSetResourceClaimActionAuthStrategies[]> GetDefaultAuthStrategies(IReadOnlyCollection<SecurityResourceClaim> resourceClaims)
        {
            var resultDictionary = new Dictionary<int, ClaimSetResourceClaimActionAuthStrategies[]>();

            var defaultAuthStrategies = _securityContext.ResourceClaimAuthorizationMetadatas
                .Include(x => x.ResourceClaim).Include(x => x.Action).Include(x => x.AuthorizationStrategy).ToList();

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
                            x.Action.ActionName == Action.Create.Value)?.AuthorizationStrategy;
                    AddStrategyToParentResource(createDefaultStrategy);
                    var readDefaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Read.Value)?.AuthorizationStrategy;
                    AddStrategyToParentResource(readDefaultStrategy);
                    var updateDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Update.Value)?.AuthorizationStrategy;
                    AddStrategyToParentResource(updateDefaultStrategy);
                    var deleteDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Delete.Value)?.AuthorizationStrategy;
                    AddStrategyToParentResource(deleteDefaultStrategy);
                                        var readChangesDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.ReadChanges.Value)?.AuthorizationStrategy;
                    AddStrategyToParentResource(readChangesDefaultStrategy);

                    void AddStrategyToParentResource(SecurityAuthorizationStrategy ? defaultStrategy)
                    {
                        actions.Add(defaultStrategy != null ?new ClaimSetResourceClaimActionAuthStrategies()
                        {
                            AuthorizationStrategies = new List<AuthorizationStrategy?>
                        { _mapper.Map<AuthorizationStrategy>(defaultStrategy) }}: null);
                    }
                }
                else
                {
                    var createDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Create.Value)?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(createDefaultStrategy, Action.Create);

                    var readDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Read.Value)?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(readDefaultStrategy, Action.Read);

                    var updateDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Update.Value)?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(updateDefaultStrategy, Action.Update);

                    var deleteDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.Delete.Value)?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(deleteDefaultStrategy, Action.Delete);

                                        var readChangesDefaultStrategy = defaultAuthStrategiesForChildren.SingleOrDefault(x =>
                        x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                        x.Action.ActionName == Action.ReadChanges.Value)?.AuthorizationStrategy;
                    actions = AddStrategyToChildResource(readChangesDefaultStrategy, Action.ReadChanges);

                    List<ClaimSetResourceClaimActionAuthStrategies?> AddStrategyToChildResource(SecurityAuthorizationStrategy? defaultStrategy, Action action)
                    {
                        if (defaultStrategy == null)
                        {
                            defaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ParentResourceClaimId &&
                                x.Action.ActionName == action.Value)?.AuthorizationStrategy;
                            var mappedStrategy = _mapper.Map<AuthorizationStrategy>(defaultStrategy);
                            if (mappedStrategy != null) mappedStrategy.IsInheritedFromParent = true;

                            var strategy = mappedStrategy != null ? new ClaimSetResourceClaimActionAuthStrategies()
                            {
                                AuthorizationStrategies = new List<AuthorizationStrategy> { _mapper.Map<AuthorizationStrategy>(mappedStrategy) }.ToArray()
                            } : null;

                            actions.Add(strategy);
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

                resultDictionary[resourceClaim.ResourceClaimId] = actions.ToArray() as ClaimSetResourceClaimActionAuthStrategies[];
            }

            return resultDictionary;
        }


        internal Dictionary<int, ClaimSetResourceClaimActionAuthStrategies?[]> GetAuthStrategyOverrides(List<ClaimSetResourceClaim> resourceClaims)
        {
            var resultDictionary = new Dictionary<int, ClaimSetResourceClaimActionAuthStrategies?[]>();
            resourceClaims =
                new List<ClaimSetResourceClaim>(resourceClaims.OrderBy(i => new List<string> { Action.Create.Value, Action.Read.Value, Action.Update.Value, Action.Delete.Value, Action.ReadChanges.Value /* SFUQUA */ }.IndexOf(i.Action.ActionName)));
            foreach (var resourceClaim in resourceClaims)
            {
                AuthorizationStrategy? authStrategy = null;
                if (resourceClaim.ResourceClaim.ParentResourceClaim == null)
                {
                    authStrategy = _mapper.Map<AuthorizationStrategy>(resourceClaim.AuthorizationStrategyOverride);
                }
                else
                {
                    var parentResources = _securityContext.ClaimSetResourceClaims
                        .Include(x => x.ResourceClaim)
                        .Include(x => x.ClaimSet)
                        .Include(x => x.Action)
                        .Include(x => x.AuthorizationStrategyOverride).ToList();
                    var parentResourceOverride = parentResources.SingleOrDefault(x => x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaim.ParentResourceClaimId
                                                                                               && x.ClaimSet.ClaimSetId == resourceClaim.ClaimSet.ClaimSetId
                                                                                               && x.Action.ActionId == resourceClaim.Action.ActionId);
                    if (parentResourceOverride?.AuthorizationStrategyOverride != null)
                    {
                        authStrategy =
                            _mapper.Map<AuthorizationStrategy>(parentResourceOverride.AuthorizationStrategyOverride);
                        if (authStrategy != null)
                        {
                            authStrategy.IsInheritedFromParent = true;
                        }
                    }

                    if (resourceClaim.AuthorizationStrategyOverride != null)
                    {
                        authStrategy = _mapper.Map<AuthorizationStrategy>(resourceClaim.AuthorizationStrategyOverride);
                    }
                }

                if (resultDictionary.ContainsKey(resourceClaim.ResourceClaim.ResourceClaimId))
                {
                    resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId].AddAuthorizationStrategyOverrides(resourceClaim.Action.ActionName, authStrategy);
                }
                else
                {
                    resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId] = new ClaimSetResourceClaimActionAuthStrategies[4];
                    resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId].AddAuthorizationStrategyOverrides(resourceClaim.Action.ActionName, authStrategy);
                }
            }
            return resultDictionary;
        }

        internal IReadOnlyList<ResourceClaim> GetChildResources(int claimSetId)
        {
            var dbChildResources =
                _securityContext.ClaimSetResourceClaims
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Include(x => x.AuthorizationStrategyOverride)
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
                 }).ToList();
            return childResources;
        }
    }
}
