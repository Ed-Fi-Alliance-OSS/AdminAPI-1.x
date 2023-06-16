// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public class GetApplicationsByOdsInstanceIdQuery
{
    private readonly IUsersContext _context;

    public GetApplicationsByOdsInstanceIdQuery(IUsersContext context)
    {
        _context = context;
    }

    public List<Application> Execute(int odsInstanceId)
    {
        var applications = _context.Applications
            .Where(a => a.OdsInstance != null && a.OdsInstance.OdsInstanceId == odsInstanceId)
            .ToList();

        if (!applications.Any() && _context.OdsInstances.Find(odsInstanceId) == null)
        {
            throw new NotFoundException<int>("odsinstance", odsInstanceId);
        }

        return applications;
    }
}
