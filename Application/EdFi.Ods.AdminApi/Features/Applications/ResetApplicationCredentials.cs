// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

namespace EdFi.Ods.AdminApi.Features.Applications;

public class ResetApplicationCredentials : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPut(endpoints, "/applications/{id}/reset-credential", HandleResetCredentials)
            .WithSummary("Reset application credentials. Returns new key and secret.")
            .WithRouteOptions(b => b.WithResponse<ApplicationResult>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> HandleResetCredentials(RegenerateApiClientSecretCommand resetSecretCommand, IMapper mapper, int id)
    {
        var resetApplicationSecret = await Task.Run(() => resetSecretCommand.Execute(id));
        var model = mapper.Map<ApplicationResult>(resetApplicationSecret);
        return Results.Ok(model);
    }
}
