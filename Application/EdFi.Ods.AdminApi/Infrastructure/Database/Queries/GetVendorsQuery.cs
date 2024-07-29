// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Features;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetVendorsQuery
{
    List<Vendor> Execute();
    List<Vendor> Execute(int offset, int limit, string? orderBy, string sortDirection, int? id, string? company, string? namespacePrefixes, string? contactName, string? contactEmailAddress);
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
            .Include(v => v.Applications)
                .ThenInclude(a => a.Profiles)
            .Include(v => v.Applications)
                .ThenInclude(a => a.ApplicationEducationOrganizations)
            .Include(v => v.Applications)
                .ThenInclude(a => a.ApiClients)
            .Include(v => v.Users)
            .Include(v => v.VendorNamespacePrefixes)
            .OrderBy(v => v.VendorName).Where(v => !VendorExtensions.ReservedNames.Contains(v.VendorName.Trim())).ToList();
    }

    public List<Vendor> Execute(int offset, int limit, string? orderBy, string? sortDirection, int? id, string? company, string? namespacePrefixes, string? contactName, string? contactEmailAddress)
    {
        return _context.Vendors
            .Where(c => id == null || id < 1 || c.VendorId == id)
            .Where(c => company == null || c.VendorName == company)
            .Where(c => c.VendorNamespacePrefixes.Any(v => namespacePrefixes == null || v.NamespacePrefix == namespacePrefixes))
            .Where(c => c.Users.Any(u => contactName == null || u.FullName == contactName))
            .Where(c => c.Users.Any(u => contactEmailAddress == null || u.Email == contactEmailAddress))
            .Include(v => v.Applications)
                .ThenInclude(a => a.Profiles)
            .Include(v => v.Applications)
                .ThenInclude(a => a.ApplicationEducationOrganizations)
            .Include(v => v.Applications)
                .ThenInclude(a => a.ApiClients)
            .Include(v => v.Users)
            .Include(v => v.VendorNamespacePrefixes)
            .Where(v => !VendorExtensions.ReservedNames.Contains(v.VendorName.Trim()))
            .Skip(offset).Take(limit).ToList();
    }
}
