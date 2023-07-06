// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using FluentValidation;
using FluentValidation.Results;

namespace EdFi.Ods.AdminApi.Features.ClaimSets;

public class EditClaimSet : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{id}", Handle)
        .WithDefaultDescription()
        .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(200))
        .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(EditClaimSetValidator validator, IEditClaimSetCommand editClaimSetCommand,
        UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IGetApplicationsByClaimSetIdQuery getApplications,
        IAuthStrategyResolver strategyResolver,
        IMapper mapper,
        EditClaimSetRequest request, int id)
    {
        request.Id = id;
        await validator.GuardAsync(request);

        var editClaimSetModel = new EditClaimSetModel
        {
            ClaimSetName = request.Name,
            ClaimSetId = id
        };

        int updatedClaimSetId;
        try
        {
            updatedClaimSetId = editClaimSetCommand.Execute(editClaimSetModel);
        }
        catch (AdminApiException exception)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(id), exception.Message) });
        }

        var resourceClaims = mapper.Map<List<ResourceClaim>>(request.ResourceClaims);
        
        var resolvedResourceClaims = strategyResolver.ResolveAuthStrategies(resourceClaims).ToList();
        if (resolvedResourceClaims.Count > 0)
        {
            updateResourcesOnClaimSetCommand.Execute(
            new UpdateResourcesOnClaimSetModel { ClaimSetId = updatedClaimSetId, ResourceClaims = resolvedResourceClaims });
        }

        var claimSet = getClaimSetByIdQuery.Execute(updatedClaimSetId);

        var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
        model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(updatedClaimSetId)
            .Select(r => mapper.Map<ResourceClaimModel>(r)).ToList();

        return AdminApiResponse<ClaimSetDetailsModel>.Updated(model, "ClaimSet");
    }


  
}
