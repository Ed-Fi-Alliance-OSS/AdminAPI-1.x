// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApi.AdminConsole.Features.OdsInstances;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadInstances : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/instances", GetInstances)
            .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/instances/{id}", GetInstance)
            .WithRouteOptions(b => b.WithResponse<InstanceModel>(200))
            .BuildForVersions();
    }

    internal async Task<IResult> GetInstances([FromServices] IGetInstancesQuery getInstancesQuery)
    {
        var instances = await getInstancesQuery.Execute();
        return Results.Ok(instances.Select(i => i.Document).ToList());
    }

    internal async Task<IResult> GetInstance([FromServices] IGetInstanceQuery getInstanceQuery, int tenantId)
    {
        var instance = await getInstanceQuery.Execute(tenantId);

        if (instance != null)
            return Results.Ok(instance.Document);

        return Results.NotFound();
    }
}
