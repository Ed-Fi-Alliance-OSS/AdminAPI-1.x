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

        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{claimsetid}/resourceclaims/{resourceclaimid}", HandleResourceClaims)
        .WithDefaultDescription()
        .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(200))
        .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{claimsetid}/resourceclaims/{resourceclaimid}/overrideauthstrategies", HandleOverrideAuthStrategies)
        .WithDefaultDescription()
        .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{claimsetid}/resourceclaims/{resourceclaimid}/resetauthstrategies", HandleResetAuthStrategies)
        .WithDefaultDescription()
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

        return Results.Ok(model);
    }

    public async Task<IResult> HandleResourceClaims(EditResourceClaimClaimSetValidator validator,
        EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IMapper mapper,
        EditResourceClaimOnClaimSetRequest request, int claimsetid, int resourceclaimid)
    {
        await validator.GuardAsync(request);
        var editResourceOnClaimSetModel = mapper.Map<EditResourceOnClaimSetModel>(request);
        editResourceOnClaimSetModel.ResourceClaim!.Id = resourceclaimid;
        editResourcesOnClaimSetCommand.Execute(editResourceOnClaimSetModel);

        var claimSet = getClaimSetByIdQuery.Execute(claimsetid);

        var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
        model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(claimsetid)
            .Select(r => mapper.Map<ResourceClaimModel>(r)).ToList();

        return Results.Ok(model);
    }

    public async Task<IResult> HandleOverrideAuthStrategies(OverrideAuthStategiesOnClaimSetValidator validator,
       OverrideDefaultAuthorizationStrategyCommand overrideDefaultAuthorizationStrategyCommand,
       OverrideAuthorizationStrategyModel request, int claimsetid, int resourceclaimid)
    {
        await validator.GuardAsync(request);
        overrideDefaultAuthorizationStrategyCommand.Execute(request);

        return Results.Ok();
    }

    public async Task<IResult> HandleResetAuthStrategies(IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand,
        IMapper mapper, int claimsetid, int resourceclaimid)
    {
        var resourceClaim = getResourcesByClaimSetIdQuery.SingleResource(claimsetid, resourceclaimid);
        if (resourceClaim == null)
        {
            throw new NotFoundException<int>("ResourceClaim", resourceclaimid);
        }
        EditResourceOnClaimSetModel editResourceOnClaimSetModel = new EditResourceOnClaimSetModel();
        editResourceOnClaimSetModel.ClaimSetId = claimsetid;
        editResourceOnClaimSetModel.ResourceClaim = new ResourceClaim()
        {
            Id = resourceclaimid,
            Name = resourceClaim!.Name,
            Create = resourceClaim!.Create,
            Read = resourceClaim!.Read,
            Update = resourceClaim!.Update,
            Delete = resourceClaim!.Delete,
        };
        editResourcesOnClaimSetCommand.Execute(editResourceOnClaimSetModel);

        return await Task.FromResult(Results.Ok());
    }
}
