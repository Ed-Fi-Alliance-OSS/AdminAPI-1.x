// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Commands;

namespace EdFi.Ods.Admin.Api.Features.Vendors
{
    public class DeleteApplication : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDeleteWithDefaultOptions($"/{FeatureConstants.Applications}" + "/{id}", Handle, FeatureConstants.Applications);
        }

        public Task<IResult> Handle(IDeleteApplicationCommand deleteApplicationCommand, int id)
        {
            deleteApplicationCommand.Execute(id);
            return Task.FromResult(AdminApiResponse.Deleted("Application"));
        }
    }
}
