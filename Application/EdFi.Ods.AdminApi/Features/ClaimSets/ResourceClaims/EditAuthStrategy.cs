// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

namespace EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims;

public class EditAuthStrategy : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/claimsets/{claimsetid}/resourceclaims/{resourceclaimid}/overrideauthstrategy", HandleOverrideAuthStrategies)
       .WithDefaultDescription()
       .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{claimsetid}/resourceclaims/{resourceclaimid}/resetauthstrategies", HandleResetAuthStrategies)
        .WithDefaultDescription()
        .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> HandleOverrideAuthStrategies(OverrideAuthStategyOnClaimSetValidator validator,
      OverrideDefaultAuthorizationStrategyCommand overrideDefaultAuthorizationStrategyCommand, IMapper mapper,
      OverrideAuthStategyOnClaimSetRequest request, int claimsetid, int resourceclaimid)
    {
        await validator.GuardAsync(request);
        var model = mapper.Map<OverrideAuthStategyOnClaimSetModel>(request);
        overrideDefaultAuthorizationStrategyCommand.ExecuteOnSpecificAction(model);

        return Results.Ok();
    }

    public async Task<IResult> HandleResetAuthStrategies(IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IAuthStrategyResolver strategyResolver,
        UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
        IMapper mapper, int claimsetid, int resourceclaimid)
    {
        var resourceClaims = getResourcesByClaimSetIdQuery.AllResources(claimsetid);
        if (!resourceClaims.Any(rc=>rc.Id == resourceclaimid))
        {
            throw new NotFoundException<int>("ResourceClaim", resourceclaimid);

        }
        else
        {
            foreach (var resourceClaim in resourceClaims)
            {
                if (resourceClaim.Id == resourceclaimid)
                {
                    resourceClaim.DefaultAuthStrategiesForCRUD = Array.Empty<AuthorizationStrategy>();
                    resourceClaim.AuthStrategyOverridesForCRUD = Array.Empty<AuthorizationStrategy>();
                }
            }
            
            var resolvedResourceClaims = strategyResolver.ResolveAuthStrategies(resourceClaims).ToList();
            if (resolvedResourceClaims.Count > 0)
            {
                updateResourcesOnClaimSetCommand.Execute(
                new UpdateResourcesOnClaimSetModel { ClaimSetId = claimsetid, ResourceClaims = resolvedResourceClaims });
            }
        }

        return await Task.FromResult(Results.Ok());
    }
}
