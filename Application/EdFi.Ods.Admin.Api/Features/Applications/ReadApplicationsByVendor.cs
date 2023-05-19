﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.Admin.Api.Infrastructure.Database.Queries;

namespace EdFi.Ods.Admin.Api.Features.Applications;

public class ReadApplicationsByVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var url = "vendors/{id}/applications";

        AdminApiEndpointBuilder.MapGet(endpoints, url, GetVendorApplications)
            .WithDescription("Retrieves applications assigned to a specific vendor based on the resource identifier.")
            .WithRouteOptions(b => b.WithResponse<ApplicationModel[]>(200))
            .BuildForVersions(AdminApiVersions.V1);
    }

    internal Task<IResult> GetVendorApplications(GetApplicationsByVendorIdQuery getApplicationByVendorIdQuery, IMapper mapper, int id)
    {
        var vendorApplications = mapper.Map<List<ApplicationModel>>(getApplicationByVendorIdQuery.Execute(id));
        return Task.FromResult(AdminApiResponse<List<ApplicationModel>>.Ok(vendorApplications));
    }
}
