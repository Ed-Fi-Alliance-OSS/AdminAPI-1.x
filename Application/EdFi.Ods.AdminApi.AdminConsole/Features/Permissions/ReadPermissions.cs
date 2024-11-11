// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Features.OdsInstances;
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

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/permissions/{id}", GetPermission)
            .WithRouteOptions(b => b.WithResponse<PermissionModel>(200))
            .BuildForVersions();
    }

    internal async Task<IResult> GetPermissions([FromServices] IGetPermissionsQuery getPermissionQuery)
    {
        var permissions = await getPermissionQuery.Execute();
        IEnumerable<JsonDocument> permissionsList = permissions.Select(i => JsonDocument.Parse(i.Document));
        return Results.Ok(permissionsList);
    }

    internal async Task<IResult> GetPermission([FromServices] IGetPermissionQuery getPermissionQuery, int tenantId)
    {
        var permission = await getPermissionQuery.Execute(tenantId);

        if (permission != null)
            return Results.Ok(JsonDocument.Parse(permission.Document));

        return Results.NotFound();
    }
}
