// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Features.Vendors;
using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        IGetVendorsQuery getVendorsAndApplicationsQuery,
        IMapper mapper,
        IOptions<AppSettings> settings,
        [AsParameters] CommonQueryParams commonQueryParams, int? id, string? applicationName, string? claimsetName)
    {
        var vendors = getVendorsAndApplicationsQuery.Execute();
        var applications = new List<ApplicationModel>();
        foreach (var vendor in vendors)
        {
            applications.AddRange(mapper.Map<List<ApplicationModel>>(vendor.Applications));
        }
        var filteredApplications = applications
            .AsEnumerable()
            .Where(a => id == null || a.Id == id)
            .Where(a => applicationName == null || a.ApplicationName == applicationName)
            .Where(a => claimsetName == null || a.ClaimSetName == claimsetName)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, settings);

        var applicationsReturned = mapper.Map<List<ApplicationModel>>(filteredApplications);
        applicationsReturned = applicationsReturned.Sort(commonQueryParams.OrderBy ?? string.Empty, SortingDirectionHelper.GetNonEmptyOrDefault(commonQueryParams.Direction));
        return Task.FromResult(Results.Ok(applicationsReturned));
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
