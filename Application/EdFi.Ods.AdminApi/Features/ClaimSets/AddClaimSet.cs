// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Features.Applications;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

namespace EdFi.Ods.AdminApi.Features.ClaimSets;

public class AddClaimSet : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/claimsets", Handle)
        .WithDefaultDescription()
        .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(201))
        .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(AddClaimSetValidator validator, AddClaimSetCommand addClaimSetCommand,
        AddOrEditResourcesOnClaimSetCommand addOrEditResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IGetApplicationsByClaimSetIdQuery getApplications,
        IAuthStrategyResolver strategyResolver,
        IMapper mapper,
        AddClaimSetRequest request)
    {
        await validator.GuardAsync(request);
        var addedClaimSetId = addClaimSetCommand.Execute(new AddClaimSetModel
        {
            ClaimSetName = request.Name ?? string.Empty
        });

        var resourceClaims = mapper.Map<List<ResourceClaim>>(request.ResourceClaims);
        var resolvedResourceClaims = strategyResolver.ResolveAuthStrategies(resourceClaims).ToList();

        addOrEditResourcesOnClaimSetCommand.Execute(addedClaimSetId, resolvedResourceClaims);

        var claimSet = getClaimSetByIdQuery.Execute(addedClaimSetId);

        var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
        var applications = getApplications.Execute(addedClaimSetId);
        if (applications != null)
        {
            model.Applications = mapper.Map<List<SimpleApplicationModel>>(applications);
        }
        model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(addedClaimSetId)
            .Select(r => mapper.Map<ResourceClaimModel>(r)).ToList();

        return AdminApiResponse<ClaimSetDetailsModel>.Created(model, "ClaimSet", $"/claimsets/{addedClaimSetId}");
    }
}
