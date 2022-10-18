// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Commands;

namespace EdFi.Ods.Admin.Api.Features.Applications
{
    public class DeleteApplication : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiEndpointBuilder.MapDelete(endpoints, "/applications/{id}", Handle)
                .WithDefaultDescription()
                .WithRouteOptions(b => b.WithResponseCode(200, FeatureConstants.DeletedSuccessResponseDescription))
                .BuildForVersions(AdminApiVersions.V1);
        }

        public Task<IResult> Handle(IDeleteApplicationCommand deleteApplicationCommand, int id)
        {
            deleteApplicationCommand.Execute(id);
            return Task.FromResult(AdminApiResponse.Deleted("Application"));
        }
    }
}
