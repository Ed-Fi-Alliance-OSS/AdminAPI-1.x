// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets;

public class ReadClaimSets : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/claimsets", GetClaimSets)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<List<ClaimSetModel>>(200))
            .BuildForVersions(AdminApiVersions.V1);

        AdminApiEndpointBuilder.MapGet(endpoints, "/claimsets/{id}", GetClaimSet)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(200))
            .BuildForVersions(AdminApiVersions.V1);
    }

    internal Task<IResult> GetClaimSets(IGetAllClaimSetsQuery getClaimSetsQuery, IGetApplicationsByClaimSetIdQuery getApplications, IMapper mapper)
    {
        var claimSets = getClaimSetsQuery.Execute().Where(x => !CloudOdsAdminApp.SystemReservedClaimSets.Contains(x.Name)).ToList();
        var model = mapper.Map<List<ClaimSetModel>>(claimSets);
        foreach(var claimSet in model)
        {
            claimSet.ApplicationsCount = getApplications.ExecuteCount(claimSet.Id);
            claimSet.IsSystemReserved = CloudOdsAdminApp.DefaultClaimSets.Contains(claimSet.Name);
        }
        return Task.FromResult(AdminApiResponse<List<ClaimSetModel>>.Ok(model));
    }

    internal Task<IResult> GetClaimSet(IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IGetApplicationsByClaimSetIdQuery getApplications, IMapper mapper, int id)
    {
        ClaimSet claimSet;
        try
        {
            claimSet = getClaimSetByIdQuery.Execute(id);
        }
        catch (AdminAppException)
        {
            throw new NotFoundException<int>("claimset", id);
        }

        var allResources = getResourcesByClaimSetIdQuery.AllResources(id);
        var claimSetData = mapper.Map<ClaimSetDetailsModel>(claimSet);
        claimSetData.ApplicationsCount = getApplications.ExecuteCount(id);
        claimSetData.ResourceClaims = mapper.Map<List<ResourceClaimModel>>(allResources.ToList());

        return Task.FromResult(AdminApiResponse<ClaimSetDetailsModel>.Ok(claimSetData));
    }
}
