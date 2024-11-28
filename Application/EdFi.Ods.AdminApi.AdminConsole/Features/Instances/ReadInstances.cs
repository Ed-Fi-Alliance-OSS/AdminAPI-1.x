// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

public class ReadInstances : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/instances", GetInstances)
            .BuildForVersions(AdminApiVersions.AdminConsole);

        AdminApiEndpointBuilder.MapGet(endpoints, "/instances/{tenantId}/{id}", GetInstanceById)
            .WithRouteOptions(b => b.WithResponse<InstanceModel>(200))
            .BuildForVersions(AdminApiVersions.AdminConsole);

        AdminApiEndpointBuilder.MapGet(endpoints, "/instances/{tenantId}", GetInstancesByTenantId)
            .WithRouteOptions(b => b.WithResponse<InstanceModel>(200))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    internal async Task<IResult> GetInstances([FromServices] IGetInstancesQuery getInstancesQuery)
    {
        var instances = await getInstancesQuery.Execute();
        return Results.Ok(instances);
    }

    internal async Task<IResult> GetInstanceById([FromServices] IGetInstanceByIdQuery getInstanceQuery, int tenantId, int id)
    {
        var instance = await getInstanceQuery.Execute(tenantId, id);

        if (instance != null)
            return Results.Ok(instance);

        return Results.NotFound();
    }

    internal async Task<IResult> GetInstancesByTenantId([FromServices] IGetInstancesByTenantIdQuery getInstancesQuery, int tenantId)
    {
        var instances = await getInstancesQuery.Execute(tenantId);
        if (instances.Any())
        {
            return Results.Ok(instances);
        }
        return Results.NotFound();
    }
}
