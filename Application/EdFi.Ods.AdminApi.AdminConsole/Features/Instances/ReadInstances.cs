// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Features.OdsInstances;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

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
        IEnumerable<JsonDocument> instancesList = instances.Select(i => JsonDocument.Parse(i.Document));
        return Results.Ok(instancesList);
    }

    internal async Task<IResult> GetInstance([FromServices] IGetInstanceQuery getInstanceQuery, int tenantId)
    {
        var instance = await getInstanceQuery.Execute(tenantId);

        if (instance != null)
            return Results.Ok(JsonDocument.Parse(instance.Document));

        return Results.NotFound();
    }
}
