// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;

namespace EdFi.Ods.AdminApi.Features.Applications;

public class ReadApplication : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/applications", GetApplications)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ApplicationModel[]>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/applications/{id}", GetApplication)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ApplicationModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    internal Task<IResult> GetApplications(
        IGetVendorsQuery getVendorsAndApplicationsQuery, IMapper mapper, int offset, int limit, string? orderBy, string? sortDirection, int? id, string? applicationName, string? claimsetName)
    {
        var vendors = getVendorsAndApplicationsQuery.Execute();
        var applications = new SortableList<ApplicationModel>().Sort(orderBy ?? string.Empty, SortingDirection.GetNonEmptyOrDefault(sortDirection));
        foreach (var vendor in vendors)
        {
            applications.AddRange(mapper.Map<List<ApplicationModel>>(vendor.Applications));
        }

        var filteredApplications = applications.AsEnumerable()
            .Where(a => id == null || a.Id == id)
            .Where(a => applicationName == null || a.ApplicationName == applicationName)
            .Where(a => claimsetName == null || a.ClaimSetName == claimsetName)
            .Skip(offset)
            .Take(limit);

        return Task.FromResult(Results.Ok(filteredApplications));
    }

    internal Task<IResult> GetApplication(GetApplicationByIdQuery getApplicationByIdQuery, IMapper mapper, int id)
    {
        var application = getApplicationByIdQuery.Execute(id);
        if (application == null)
        {
            throw new NotFoundException<int>("application", id);
        }
        var model = mapper.Map<ApplicationModel>(application);
        return Task.FromResult(Results.Ok(model));
    }
}
