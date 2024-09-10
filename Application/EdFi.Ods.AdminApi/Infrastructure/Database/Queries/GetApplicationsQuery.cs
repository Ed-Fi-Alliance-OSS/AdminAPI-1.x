// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries
{
    public interface IGetApplicationsQuery
    {
        List<Application> Execute();
        List<Application> Execute(CommonQueryParams commonQueryParams);
    }

    public class GetApplicationsQuery : IGetApplicationsQuery
    {
        private readonly IUsersContext _context;

        public GetApplicationsQuery(IUsersContext context)
        {
            _context = context;
        }

        public List<Application> Execute()
        {
            return _context.Applications
                .Include(ap => ap.Vendor).ThenInclude(ap => ap.VendorNamespacePrefixes)
                .Include(ap => ap.Vendor).ThenInclude(ap => ap.Users)
                .Include(ap => ap.Profiles)
                .Include(ap => ap.OdsInstance)
                .Include(ap => ap.ApplicationEducationOrganizations)
                .OrderBy(v => v.Vendor.VendorName)
                .Where(v => !VendorExtensions.ReservedNames.Contains(v.Vendor.VendorName.Trim()))
                .ToList();
        }

        public List<Application> Execute(CommonQueryParams commonQueryParams)
        {
            return _context.Applications
                .Include(ap => ap.Vendor).ThenInclude(ap => ap.VendorNamespacePrefixes)
                .Include(ap => ap.Vendor).ThenInclude(ap => ap.Users)
                .Include(ap => ap.Profiles)
                .Include(ap => ap.OdsInstance)
                .Include(ap => ap.ApplicationEducationOrganizations)
                .OrderBy(v => v.Vendor.VendorName)
                .Where(v => !VendorExtensions.ReservedNames.Contains(v.Vendor.VendorName.Trim()))
                .Paginate(commonQueryParams.Offset, commonQueryParams.Limit)
                .ToList();
        }
    }
}
