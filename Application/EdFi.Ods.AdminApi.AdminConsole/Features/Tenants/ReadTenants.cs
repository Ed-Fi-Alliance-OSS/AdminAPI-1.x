// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;

public class ReadTenants : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/tenants", GetTenants)
           .BuildForVersions(AdminApiVersions.AdminConsole);

        AdminApiEndpointBuilder.MapGet(endpoints, "/tenants/{tenantId}", GetTenantsByTenantId)
           .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    internal async Task<IResult> GetTenants(IGetTenantsQuery getTenantQuery)
    {
        var tenants = await getTenantQuery.Execute();
        return Results.Ok(tenants);
    }

    internal async Task<IResult> GetTenantsById(IGetTenantByIdQuery getTenantQuery, int id)
    {
        var tenant = await getTenantQuery.Execute(id);
        if (tenant != null)
            return Results.Ok(tenant);
        return Results.NotFound();
    }

    internal async Task<IResult> GetTenantsByTenantId(IGetTenantByTenantIdQuery getTenantQuery, int tenantId)
    {
        var tenants = await getTenantQuery.Execute(tenantId);

        if (tenants.Any())
        {
            return Results.Ok(tenants);
        }
        return Results.NotFound();
    }
}
