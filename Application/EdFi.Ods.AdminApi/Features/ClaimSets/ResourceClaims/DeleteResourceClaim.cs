// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

namespace EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims;

public class DeleteResourceClaim : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapDelete(endpoints, "/claimsets/{claimsetid}/resourceclaims/{resourceclaimid}", Handle)
       .WithDefaultDescription()
       .BuildForVersions(AdminApiVersions.V2);
    }


    public async Task<IResult> Handle(IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IAuthStrategyResolver strategyResolver,
        IDeleteResouceClaimOnClaimSetCommand deleteResouceClaimOnClaimSetCommand,
        IMapper mapper, int claimsetid, int resourceclaimid)
    {
        var resourceClaims = getResourcesByClaimSetIdQuery.SingleResource(claimsetid, resourceclaimid);
        if (resourceClaims == null)
        {
            throw new NotFoundException<int>("ResourceClaim", resourceclaimid);

        }
        else
        {
            deleteResouceClaimOnClaimSetCommand.Execute(new FilterResourceClaimOnClaimSet()
            {
                ClaimSetId = claimsetid,
                ResourceClaimId = resourceclaimid
            });
        }

        return await Task.FromResult(Results.Ok());
    }

}
