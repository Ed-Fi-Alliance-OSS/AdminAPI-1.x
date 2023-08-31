// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Infrastructure;

namespace EdFi.Ods.AdminApi.Features.OdsInstances;

public class DeleteOdsInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapDelete(endpoints, "/odsInstances/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponseCode(200, FeatureConstants.DeletedSuccessResponseDescription))
            .BuildForVersions(AdminApiVersions.V2);
    }

    public Task<IResult> Handle(IDeleteOdsInstanceCommand deleteOdsInstanceCommand, int id)
    {
        deleteOdsInstanceCommand.Execute(id);
        return Task.FromResult(Results.Ok("OdsInstance".ToJsonObjectResponseDeleted()));
    }
}
