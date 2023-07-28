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
            AuthStrategyOverridesForCRUD = Array.Empty<AuthorizationStrategy>()
        };
        editResourcesOnClaimSetCommand.Execute(editResourceOnClaimSetModel);

        return await Task.FromResult(Results.Ok());
    }
}
