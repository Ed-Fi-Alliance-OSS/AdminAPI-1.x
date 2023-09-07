// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FluentValidation;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

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
                var resource = allResources.FirstOrDefault(dr => (dr.Name ?? string.Empty).Equals(r.Name, StringComparison.Ordinal));
                if (resource != null)
                {
                    resource.Actions = r.Actions;
                    resource.AuthStrategyOverridesForCRUD = r.AuthStrategyOverridesForCRUD;
                }
                return resource;
            }).ToList();

        currentResources.RemoveAll(x => x is null);

        foreach (var resource in currentResources.Where(x => x is not null))
        {
            var editResourceModel = new EditResourceOnClaimSetModel
            {
                ClaimSetId = claimSetId,
                ResourceClaim = resource
            };

            _editResourceOnClaimSetCommand.Execute(editResourceModel);

            if (resource!.AuthStrategyOverridesForCRUD != null && resource.AuthStrategyOverridesForCRUD.Any())
            {
                var overrideAuthStrategyModel = new OverrideAuthorizationStrategyModel
                {
                    ClaimSetId = claimSetId,
                    ResourceClaimId = resource.Id,
                    ClaimSetResourceClaimActionAuthStrategyOverrides = resource.AuthStrategyOverridesForCRUD
                };
                _overrideDefaultAuthorizationStrategyCommand.Execute(overrideAuthStrategyModel);
            }
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
    public string? ClaimSetName { get; set; }
}

public class EditResourceOnClaimSetModel : IEditResourceOnClaimSetModel
{
    public int ClaimSetId { get; set; }
    public ResourceClaim? ResourceClaim { get; set; }
}

public class OverrideAuthorizationStrategyModel : IOverrideDefaultAuthorizationStrategyModel
{
    public int ClaimSetId { get; set; }
    public int ResourceClaimId { get; set; }
    public List<ClaimSetResourceClaimActionAuthStrategies?>? ClaimSetResourceClaimActionAuthStrategyOverrides { get; set; }
}
