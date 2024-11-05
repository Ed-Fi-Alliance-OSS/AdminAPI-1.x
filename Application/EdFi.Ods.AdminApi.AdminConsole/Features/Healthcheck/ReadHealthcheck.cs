// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck;

public class ReadHealthcheck : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/healthcheck", GetHealthchecks)
      .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/healthcheck/{tenantId}", GetHealthcheck)
      .BuildForVersions();
    }

    internal async Task<IResult> GetHealthcheck(IMapper mapper, IGetHealthCheckQuery getHealthCheckQuery, int tenantId)
    {
        var healthChecks = await getHealthCheckQuery.Execute(tenantId);
        var model = mapper.Map<HealthCheckModel>(healthChecks);
        return Results.Ok(model);
    }

    internal async Task<IResult> GetHealthchecks(IMapper mapper, IGetHealthChecksQuery getHealthChecksQuery)
    {
        var healthChecks = await getHealthChecksQuery.Execute();
        var model = mapper.Map<IEnumerable<HealthCheckModel>>(healthChecks);
        return Results.Ok(model);
    }
}
