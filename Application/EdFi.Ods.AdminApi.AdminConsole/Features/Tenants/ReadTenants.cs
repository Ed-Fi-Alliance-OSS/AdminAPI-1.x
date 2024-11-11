// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;

public class ReadTenants : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/tenants", GetTenants)
           .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/tenants/{tenantId}", GetTenant)
           .BuildForVersions();
    }

    internal async Task<IResult> GetTenants(IGetTenantQuery getTenantQuery)
    {
        var tenants = await getTenantQuery.GetAll();
        IEnumerable<JsonDocument> tenantsList = tenants.Select(i => JsonDocument.Parse(i.Document));
        return Results.Ok(tenantsList);
    }

    internal async Task<IResult> GetTenant(IGetTenantQuery getTenantQuery, int tenantId)
    {
        var tenant = await getTenantQuery.Get(tenantId);
        if (tenant != null)
            return Results.Ok(JsonDocument.Parse(tenant.Document));
        return Results.NotFound();
    }
}
