// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using System.Text.Json;
using AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Queries;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck;

public class ReadHealthcheck : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/healthcheck", GetHealthchecks)
      .BuildForVersions(AdminApiVersions.AdminConsole);

        AdminApiEndpointBuilder.MapGet(endpoints, "/healthcheck/{tenantId}", GetHealthcheck)
      .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    internal async Task<IResult> GetHealthcheck(IMapper mapper, IGetHealthCheckQuery getHealthCheckQuery, int tenantId)
    {
        var healthChecks = await getHealthCheckQuery.Execute(tenantId);
        if(healthChecks != null)
        {
            return Results.Ok(healthChecks);
        }
        return Results.NotFound();
    }

    internal async Task<IResult> GetHealthchecks(IMapper mapper, IGetHealthChecksQuery getHealthChecksQuery)
    {
        var healthChecks = await getHealthChecksQuery.Execute();
        return Results.Ok(healthChecks);
    }
}
