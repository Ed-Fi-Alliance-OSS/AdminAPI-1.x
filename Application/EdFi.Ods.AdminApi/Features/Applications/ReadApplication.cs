// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

namespace EdFi.Ods.AdminApi.Features.Applications;

public class ReadApplication : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/applications", GetApplications)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ApplicationModel[]>(200))
            .BuildForVersions(AdminApiVersions.V1);

        AdminApiEndpointBuilder.MapGet(endpoints, "/applications/{id}", GetApplication)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ApplicationModel>(200))
            .BuildForVersions(AdminApiVersions.V1);
    }

<<<<<<< HEAD
    internal Task<IResult> GetApplications(
        IGetApplicationsQuery getApplicationsAndApplicationsQuery, IMapper mapper, [AsParameters] CommonQueryParams commonQueryParams)
=======
    internal Task<IResult> GetApplications(IGetApplicationsQuery getApplicationsAndApplicationsQuery, IMapper mapper, [AsParameters] CommonQueryParams commonQueryParams)
>>>>>>> 31924e07 (Adds tests and other fixes)
    {
        var applications = getApplicationsAndApplicationsQuery.Execute(commonQueryParams);
        return Task.FromResult(AdminApiResponse<List<ApplicationModel>>.Ok(mapper.Map<List<ApplicationModel>>(applications)));
    }

    internal Task<IResult> GetApplication(GetApplicationByIdQuery getApplicationByIdQuery, IMapper mapper, int id)
    {
        var application = getApplicationByIdQuery.Execute(id);
        if (application == null)
        {
            throw new NotFoundException<int>("application", id);
        }
        var model = mapper.Map<ApplicationModel>(application);
        return Task.FromResult(AdminApiResponse<ApplicationModel>.Ok(model));
    }
}
