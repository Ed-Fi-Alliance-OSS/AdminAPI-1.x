// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetVendorsQuery
{
    List<Vendor> Execute();
    List<Vendor> Execute(CommonQueryParams commonQueryParams);
}

public class GetVendorsQuery : IGetVendorsQuery
{
    private readonly IUsersContext _context;

    public GetVendorsQuery(IUsersContext context)
    {
        _context = context;
    }

    public List<Vendor> Execute()
    {
        return _context.Vendors
            .Include(vn => vn.VendorNamespacePrefixes)
            .Include(x => x.Users)
            .Include(x => x.Applications).ThenInclude(x => x.ApplicationEducationOrganizations)
            .Include(x => x.Applications).ThenInclude(x => x.Profiles)
            .Include(x => x.Applications).ThenInclude(x => x.OdsInstance)
            .OrderBy(v => v.VendorName).Where(v => !VendorExtensions.ReservedNames.Contains(v.VendorName.Trim()))
            .ToList();
    }

    public List<Vendor> Execute(CommonQueryParams commonQueryParams)
    {
        return _context.Vendors
            .Include(vn => vn.VendorNamespacePrefixes)
            .Include(x => x.Users)
            .Include(x => x.Applications).ThenInclude(x => x.ApplicationEducationOrganizations)
            .Include(x => x.Applications).ThenInclude(x => x.Profiles)
            .Include(x => x.Applications).ThenInclude(x => x.OdsInstance)
            .OrderBy(v => v.VendorName).Where(v => !VendorExtensions.ReservedNames.Contains(v.VendorName.Trim()))
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit)
            .ToList();
    }
}
