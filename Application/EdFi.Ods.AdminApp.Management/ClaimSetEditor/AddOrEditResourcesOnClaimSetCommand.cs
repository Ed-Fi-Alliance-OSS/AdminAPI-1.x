// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor.Extensions;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class AddOrEditResourcesOnClaimSetCommand
    {
        private readonly EditResourceOnClaimSetCommand _editResourceOnClaimSetCommand;
        private readonly IGetResourceClaimsQuery _getResourceClaimsQuery;
        private readonly OverrideDefaultAuthorizationStrategyCommand _overrideDefaultAuthorizationStrategyCommand;

        public AddOrEditResourcesOnClaimSetCommand(EditResourceOnClaimSetCommand editResourceOnClaimSetCommand,
            IGetResourceClaimsQuery getResourceClaimsQuery,
            OverrideDefaultAuthorizationStrategyCommand overrideDefaultAuthorizationStrategyCommand)
        {
            _editResourceOnClaimSetCommand = editResourceOnClaimSetCommand;
            _getResourceClaimsQuery = getResourceClaimsQuery;
            _overrideDefaultAuthorizationStrategyCommand = overrideDefaultAuthorizationStrategyCommand;
        }

        public void Execute(int claimSetId, List<ResourceClaim> resources)
        {

            var allResources = GetDbResources();

            var childResources = new List<ResourceClaim>();
            foreach (var resourceClaims in resources.Select(x => x.Children))
                childResources.AddRange(resourceClaims);
            resources.AddRange(childResources);
            var currentResources = resources.Select(r =>
                {
                    var resource = allResources.FirstOrDefault(dr => dr.Name.Equals(r.Name));
                    if (resource != null)
                    {
                        resource.Create = r.Create;
                        resource.Read = r.Read;
                        resource.Update = r.Update;
                        resource.Delete = r.Delete;
                        resource.AuthStrategyOverridesForCRUD = r.AuthStrategyOverridesForCRUD;
                    }
                    return resource;
                }).ToList();
            currentResources.RemoveAll(x => x == null);
            foreach (var resource in currentResources)
            {
                var editResourceModel = new EditResourceOnClaimSetModel
                {
                    ClaimSetId = claimSetId,
                    ResourceClaim = resource
                };

                _editResourceOnClaimSetCommand.Execute(editResourceModel);

                if (resource.AuthStrategyOverridesForCRUD != null && resource.AuthStrategyOverridesForCRUD.Any())
                {
                    var overrideAuthStrategyModel = new OverrideAuthorizationStrategyModel
                    {
                        ClaimSetId = claimSetId,
                        ResourceClaimId = resource.Id,
                        AuthorizationStrategyForCreate = AuthStrategyOverrideForAction(resource.AuthStrategyOverridesForCRUD.Create()),
                        AuthorizationStrategyForRead = AuthStrategyOverrideForAction(resource.AuthStrategyOverridesForCRUD.Read()),
                        AuthorizationStrategyForUpdate = AuthStrategyOverrideForAction(resource.AuthStrategyOverridesForCRUD.Update()),
                        AuthorizationStrategyForDelete = AuthStrategyOverrideForAction(resource.AuthStrategyOverridesForCRUD.Delete())
                    };
                    _overrideDefaultAuthorizationStrategyCommand.Execute(overrideAuthStrategyModel);
                }
            }

            static int AuthStrategyOverrideForAction(AuthorizationStrategy authorizationStrategy)
            {
                return authorizationStrategy != null ? authorizationStrategy.AuthStrategyId : 0;
            }
        }

        private List<ResourceClaim> GetDbResources()
        {
            var allResources = new List<ResourceClaim>();
            var parentResources = _getResourceClaimsQuery.Execute().ToList();
            allResources.AddRange(parentResources);
            foreach (var children in parentResources.Select(x => x.Children))
            {
                allResources.AddRange(children);
            }

            return allResources;
        }
    }

    public class AddClaimSetModel : IAddClaimSetModel
    {
        public string ClaimSetName { get; set; }
    }

    public class EditResourceOnClaimSetModel : IEditResourceOnClaimSetModel
    {
        public int ClaimSetId { get; set; }
        public ResourceClaim ResourceClaim { get; set; }
    }

    public class OverrideAuthorizationStrategyModel : IOverrideDefaultAuthorizationStrategyModel
    {
        public int ClaimSetId { get; set; }
        public int ResourceClaimId { get; set; }
        public int AuthorizationStrategyForCreate { get; set; }
        public int AuthorizationStrategyForRead { get; set; }
        public int AuthorizationStrategyForUpdate { get; set; }
        public int AuthorizationStrategyForDelete { get; set; }
    }
}
