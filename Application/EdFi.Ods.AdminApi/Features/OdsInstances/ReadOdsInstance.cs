// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using static EdFi.Ods.AdminApi.Features.SortingDirection;

namespace EdFi.Ods.AdminApi.Features.ODSInstances;

public class ReadOdsInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/odsInstances", GetOdsInstances)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<OdsInstanceModel[]>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/odsInstances/{id}", GetOdsInstance)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<OdsInstanceDetailModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    internal Task<IResult> GetOdsInstances(IGetOdsInstancesQuery getOdsInstancesQuery, IMapper mapper, int offset, int limit, string? orderBy, string? sortDirection, int? id, string? name)
    {
        var odsInstances = mapper.Map<SortableList<OdsInstanceModel>>(getOdsInstancesQuery.Execute(offset, limit, id, name));
        return Task.FromResult(Results.Ok(odsInstances.Sort(orderBy ?? string.Empty, SortingDirection.GetNonEmptyOrDefault(sortDirection))));
    }

    internal Task<IResult> GetOdsInstance(IGetOdsInstanceQuery getOdsInstanceQuery, IMapper mapper, int id)
    {
        var odsInstance = getOdsInstanceQuery.Execute(id);
        var model = mapper.Map<OdsInstanceDetailModel>(odsInstance);
        return Task.FromResult(Results.Ok(model));
    }
}

