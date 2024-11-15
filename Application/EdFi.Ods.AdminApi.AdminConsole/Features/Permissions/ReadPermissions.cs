// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Features.Permissions;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Permissions;

public class ReadPermissions : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/permissions", GetPermissions)
            .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/permissions/{tenantId}/{id}", GetPermissionById)
            .WithRouteOptions(b => b.WithResponse<PermissionModel>(200))
            .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/permissions/{tenantId}", GetPermissionsByTenantId)
            .WithRouteOptions(b => b.WithResponse<PermissionModel>(200))
            .BuildForVersions();
    }

    internal async Task<IResult> GetPermissions([FromServices] IGetPermissionsQuery getPermissionQuery)
    {
        var permissions = await getPermissionQuery.Execute();
        return Results.Ok(permissions);
    }

    internal async Task<IResult> GetPermissionById([FromServices] IGetPermissionByIdQuery getPermissionQuery, int tenantId, int id)
    {
        var permission = await getPermissionQuery.Execute(tenantId, id);

        if (permission != null)
            return Results.Ok(permission);

        return Results.NotFound();
    }

    internal async Task<IResult> GetPermissionsByTenantId([FromServices] IGetPermissionsByTenantIdQuery getPermissionQuery, int tenantId)
    {
        var permissions = await getPermissionQuery.Execute(tenantId);
        if (permissions.Any())
        {
            return Results.Ok(permissions);
        }
        return Results.NotFound();
    }
}
