// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using System.Text.Json;
using AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Features.Permissions;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.UserProfiles.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadUserProfiles : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/userprofile", GetUserProfiles)
            .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/userprofile/{tenantId}", GetUserProfile)
            .WithRouteOptions(b => b.WithResponse<UserProfileModel>(200))
            .BuildForVersions();
    }

    internal async Task<IResult> GetUserProfile(IMapper mapper, IGetUserProfileQuery getUserProfileQuery, int tenantId)
    {
        var userProfile = await getUserProfileQuery.Execute(tenantId);
        if (userProfile != null)
            return Results.Ok(JsonDocument.Parse(userProfile.Document));
        return Results.NotFound();
    }

    internal async Task<IResult> GetUserProfiles(IMapper mapper, IGetUserProfilesQuery getUserProfilesQuery)
    {
        var userProfiles = await getUserProfilesQuery.Execute();
        IEnumerable<JsonDocument> userProfilesList = userProfiles.Select(i => JsonDocument.Parse(i.Document));
        return Results.Ok(userProfilesList);
    }
}
