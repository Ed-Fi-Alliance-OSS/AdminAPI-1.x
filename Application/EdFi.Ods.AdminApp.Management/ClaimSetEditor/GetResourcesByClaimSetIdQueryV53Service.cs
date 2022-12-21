// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor.Extensions;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.SecurityCompatiblity53.DataAccess.Models;

using SecurityResourceClaim = EdFi.SecurityCompatiblity53.DataAccess.Models.ResourceClaim;
using SecurityAuthorizationStrategy = EdFi.SecurityCompatiblity53.DataAccess.Models.AuthorizationStrategy;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
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

        internal void AddChildResourcesToParents(IEnumerable<ResourceClaim> childResources, List<ResourceClaim> parentResources)
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

        internal List<ResourceClaim> GetParentResources(int claimSetId)
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
                    IsParent = true,
                    DefaultAuthStrategiesForCRUD = defaultAuthStrategies[x.Key.ResourceClaimId],
                    AuthStrategyOverridesForCRUD = authStrategyOverrides[x.Key.ResourceClaimId].ToArray()
                })
                .ToList();

            parentResources.ForEach(x => x.Children = new List<ResourceClaim>());
            return parentResources;
        }

        private Dictionary<int, AuthorizationStrategy[]> GetDefaultAuthStrategies(IReadOnlyCollection<SecurityResourceClaim> resourceClaims)
        {
            var resultDictionary = new Dictionary<int, AuthorizationStrategy[]>();

            var defaultAuthStrategies = _securityContext.ResourceClaimAuthorizationMetadatas
                .Include(x => x.ResourceClaim).Include(x => x.Action).Include(x => x.AuthorizationStrategy).ToList();

            var defaultAuthStrategiesForParents = defaultAuthStrategies
                .Where(x => x.ResourceClaim.ParentResourceClaimId == null).ToList();

            var defaultAuthStrategiesForChildren = defaultAuthStrategies
                .Where(x => x.ResourceClaim.ParentResourceClaimId != null).ToList();

            foreach (var resourceClaim  in resourceClaims)
            {
                var actions = new List<AuthorizationStrategy>();
                if (resourceClaim.ParentResourceClaimId == null)
                {
                    var createDefaultStrategy =  defaultAuthStrategiesForParents.SingleOrDefault(x =>
                            x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                            x.Action.ActionName == Action.Create.Value)?.AuthorizationStrategy;
                    actions.Add(_mapper.Map<AuthorizationStrategy>(createDefaultStrategy));
                    var readDefaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Read.Value)?.AuthorizationStrategy;
                    actions.Add(_mapper.Map<AuthorizationStrategy>(readDefaultStrategy));
                    var updateDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Update.Value)?.AuthorizationStrategy;
                    actions.Add(_mapper.Map<AuthorizationStrategy>(updateDefaultStrategy));
                    var deleteDefaultStrategy = defaultAuthStrategiesForParents
                            .SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ResourceClaimId &&
                                x.Action.ActionName == Action.Delete.Value)?.AuthorizationStrategy;
                    actions.Add(_mapper.Map<AuthorizationStrategy>(deleteDefaultStrategy));
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

                    List<AuthorizationStrategy> AddStrategyToChildResource(SecurityAuthorizationStrategy defaultStrategy, Action action)
                    {
                        if (defaultStrategy == null)
                        {
                            defaultStrategy = defaultAuthStrategiesForParents.SingleOrDefault(x =>
                                x.ResourceClaim.ResourceClaimId == resourceClaim.ParentResourceClaimId &&
                                x.Action.ActionName == action.Value)?.AuthorizationStrategy;
                            var mappedStrategy = _mapper.Map<AuthorizationStrategy>(defaultStrategy);
                            if (mappedStrategy != null) mappedStrategy.IsInheritedFromParent = true;
                            actions.Add(mappedStrategy);
                        }
                        else
                        {
                            actions.Add(_mapper.Map<AuthorizationStrategy>(defaultStrategy));
                        }

                        return actions;
                    }
                }

                resultDictionary[resourceClaim.ResourceClaimId] = actions.ToArray();
            }

            return resultDictionary;
        }

        internal Dictionary<int, AuthorizationStrategy[]> GetAuthStrategyOverrides(List<ClaimSetResourceClaim> resourceClaims)
        {
            var resultDictionary = new Dictionary<int, AuthorizationStrategy[]>();
            resourceClaims =
                new List<ClaimSetResourceClaim>(resourceClaims.OrderBy(i => new List<string> {Action.Create.Value, Action.Read.Value, Action.Update.Value, Action.Delete.Value}.IndexOf(i.Action.ActionName)));
            foreach (var resourceClaim in resourceClaims)
            {
                AuthorizationStrategy authStrategy = null;
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

                    if(resourceClaim.AuthorizationStrategyOverride != null)
                    {
                        authStrategy = _mapper.Map<AuthorizationStrategy>(resourceClaim.AuthorizationStrategyOverride);
                    }
                }

                if (resultDictionary.ContainsKey(resourceClaim.ResourceClaim.ResourceClaimId))
                {
                    resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId].AddAuthorizationStrategyOverrides(resourceClaim.Action.ActionName,
                        authStrategy);
                }
                else
                {
                    var actions = new AuthorizationStrategy[]{null, null, null, null};
                    resultDictionary[resourceClaim.ResourceClaim.ResourceClaimId] = actions.AddAuthorizationStrategyOverrides(resourceClaim.Action.ActionName, authStrategy);
                }
            }
            return resultDictionary;
        }

        internal IEnumerable<ResourceClaim> GetChildResources(int claimSetId)
        {
            var dbChildResources =
                _securityContext.ClaimSetResourceClaims
                .Include(x => x.ResourceClaim)
                .Include(x => x.Action)
                .Where(x => x.ClaimSet.ClaimSetId == claimSetId
                            && x.ResourceClaim.ParentResourceClaimId != null).ToList();
            var defaultAuthStrategies = GetDefaultAuthStrategies(dbChildResources.Select(x => x.ResourceClaim).ToList());
            var authStrategyOverrides = GetAuthStrategyOverrides(dbChildResources.ToList());

            var childResources = dbChildResources.GroupBy(x => x.ResourceClaim)
                .Select(x => new ResourceClaim
                {
                    Id = x.Key.ResourceClaimId,
                    ParentId = x.Key.ParentResourceClaimId.Value,
                    Name = x.Key.ResourceName,
                    Create = x.Any(a => a.Action.ActionName == Action.Create.Value),
                    Read = x.Any(a => a.Action.ActionName == Action.Read.Value),
                    Update = x.Any(a => a.Action.ActionName == Action.Update.Value),
                    Delete = x.Any(a => a.Action.ActionName == Action.Delete.Value),
                    IsParent = false,
                    DefaultAuthStrategiesForCRUD = defaultAuthStrategies[x.Key.ResourceClaimId],
                    AuthStrategyOverridesForCRUD = authStrategyOverrides[x.Key.ResourceClaimId].ToArray()
                })
                .ToList();
            return childResources;
        }
    }
}
