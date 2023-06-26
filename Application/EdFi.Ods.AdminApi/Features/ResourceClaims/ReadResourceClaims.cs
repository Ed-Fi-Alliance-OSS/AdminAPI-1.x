// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Features.ClaimSets;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

namespace EdFi.Ods.AdminApi.Features.ResourceClaims;

public class ReadResourceClaims : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/resourceclaims", GetResourceClaims)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<List<SimpleResourceClaimModel>>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/resourceclaims/{id}", GetResourceClaim)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<SimpleResourceClaimModel>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/resourceclaims/{id}/children", GetResourceClaimChildren)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<SimpleResourceClaimModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    internal Task<IResult> GetResourceClaims(IGetResourceClaimsQuery getResourceClaimsQuery, IMapper mapper)
    {
        var resourceClaims = getResourceClaimsQuery.Execute().ToList();
        var model = mapper.Map<List<SimpleResourceClaimModel>>(resourceClaims);
        
        return Task.FromResult(AdminApiResponse<List<SimpleResourceClaimModel>>.Ok(model));
    }

    internal Task<IResult> GetResourceClaim(IGetResourceClaimByResourceClaimIdQuery getResourceClaimByResourceClaimIdQuery, IMapper mapper, int id)
    {
        var resourceClaim = getResourceClaimByResourceClaimIdQuery.Execute(id);
        var model = mapper.Map<SimpleResourceClaimModel>(resourceClaim);

        return Task.FromResult(AdminApiResponse<SimpleResourceClaimModel>.Ok(model));
    }

    internal Task<IResult> GetResourceClaimChildren(IGetResourceClaimByResourceClaimIdQuery getResourceClaimByResourceClaimIdQuery, IMapper mapper, int id)
    {
        var resourceClaim = getResourceClaimByResourceClaimIdQuery.Execute(id);
        var model = mapper.Map<SimpleResourceClaimModel>(resourceClaim);

        return Task.FromResult(AdminApiResponse<List<SimpleResourceClaimModel>>.Ok(model.Children));
    }
}
