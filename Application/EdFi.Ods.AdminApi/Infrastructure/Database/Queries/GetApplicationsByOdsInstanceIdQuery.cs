// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetApplicationsByOdsInstanceIdQuery
{
    List<Application> Execute(int odsInstanceId);
}

public class GetApplicationsByOdsInstanceIdQuery : IGetApplicationsByOdsInstanceIdQuery
{
    private readonly IUsersContext _context;

    public GetApplicationsByOdsInstanceIdQuery(IUsersContext context)
    {
        _context = context;
    }

    public List<Application> Execute(int odsInstanceId)
    {
        var applications = _context.ApiClientOdsInstances
            .Include(aco => aco.OdsInstance)
            .Include(aco => aco.ApiClient)
                .ThenInclude(api => api.Application)
                .ThenInclude(app => app.ApplicationEducationOrganizations)
            .Include(api => api.ApiClient)
                .ThenInclude(api => api.Application)
                .ThenInclude(api => api.Profiles)
            .Include(api => api.ApiClient)
                .ThenInclude(api => api.Application)
                .ThenInclude(api => api.Vendor)
            .Where(a => a.OdsInstance.OdsInstanceId == odsInstanceId)
            .Select(app => app.ApiClient.Application)
            .ToList();

        if (!applications.Any() && _context.OdsInstances.Find(odsInstanceId) == null)
        {
            throw new NotFoundException<int>("odsinstance", odsInstanceId);
        }

        return applications;
    }
}
