// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

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
        dynamic result = new ExpandoObject();
        result.Documents = tenants.Select(x => x.Document);
        return Results.Ok(result.Documents);
    }

    internal async Task<IResult> GetTenant(IGetTenantQuery getTenantQuery, int tenantId)
    {
        var tenant = await getTenantQuery.Get(tenantId);
        dynamic result = new ExpandoObject();
        result.Document = tenant.Document;
        return Results.Ok(result.Document);
    }
}
