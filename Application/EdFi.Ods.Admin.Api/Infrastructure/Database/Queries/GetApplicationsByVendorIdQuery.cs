// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.Admin.Api.Infrastructure.ErrorHandling;

namespace EdFi.Ods.Admin.Api.Infrastructure.Database.Queries;

public class GetApplicationsByVendorIdQuery
{
    private readonly IUsersContext _context;

    public GetApplicationsByVendorIdQuery(IUsersContext context)
    {
        _context = context;
    }

    public List<Application> Execute(int vendorid)
    {
        var applications = _context.Applications
            .Where(a => a.Vendor != null && a.Vendor.VendorId == vendorid)
            .ToList();

        if (!applications.Any() && _context.Vendors.Find(vendorid) == null)
        {
            throw new NotFoundException<int>("vendor", vendorid);
        }

        return applications;
    }
}
