// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Commands;

namespace EdFi.Ods.Admin.Api.Features.Applications;

public class ResetApplicationCredentials : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPutWithDefaultOptions($"/{FeatureConstants.Applications}" + "/{id}/reset-credential", HandleResetCredentials, FeatureConstants.Applications);
    }

    public async Task<IResult> HandleResetCredentials(RegenerateApiClientSecretCommand resetSecretCommand, IMapper mapper, int id)
    {
        var resetApplicationSecret = await Task.Run(() => resetSecretCommand.Execute(id));
        var model = mapper.Map<ApplicationResult>(resetApplicationSecret);
        return AdminApiResponse<ApplicationResult>.Updated(model, "Application secret");
    }
}
