// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

namespace EdFi.Ods.AdminApi.Features.OdsInstances;

public class ReadOdsInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/odsInstances", GetOdsInstances)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<OdsInstanceModel[]>(200))
            .BuildForVersions(AdminApiVersions.V1);

        AdminApiEndpointBuilder.MapGet(endpoints, "/odsInstances/{id}", GetOdsInstances)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<OdsInstanceModel>(200))
            .BuildForVersions(AdminApiVersions.V1);
    }

    internal Task<IResult> GetOdsInstances(IGetOdsInstancesQuery getOdsInstancesQuery, IMapper mapper, [AsParameters] CommonQueryParams commonQueryParams)
    {
        var odsInstances = mapper.Map<List<OdsInstanceModel>>(getOdsInstancesQuery.Execute(
            commonQueryParams));
        return Task.FromResult(Results.Ok(odsInstances));
    }

    internal Task<IResult> GetOdsInstance(IGetOdsInstanceQuery getOdsInstanceQuery, IMapper mapper, int id)
    {
        var odsInstance = getOdsInstanceQuery.Execute(id);
        var model = mapper.Map<OdsInstanceModel>(odsInstance);
        return Task.FromResult(Results.Ok(model));
    }
}
