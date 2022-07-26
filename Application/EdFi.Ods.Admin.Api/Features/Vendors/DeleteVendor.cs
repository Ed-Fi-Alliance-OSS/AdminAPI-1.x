// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Commands;

namespace EdFi.Ods.Admin.Api.Features.Vendors
{
    public class DeleteVendor : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiEndpointBuilder.MapDelete(endpoints, "/vendors/{id}", Handle)
                .WithDefaultDescription()
                .BuildForVersions(AdminApiVersions.V1);
        }

        public Task<IResult> Handle(DeleteVendorCommand deleteVendorCommand, int id)
        {
            deleteVendorCommand.Execute(id);
            return Task.FromResult(AdminApiResponse.Deleted("Vendor"));
        }
    }
}
