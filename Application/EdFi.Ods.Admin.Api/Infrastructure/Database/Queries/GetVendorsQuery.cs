// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public interface IGetVendorsQuery
    {
        List<Vendor> Execute();
        List<Vendor> Execute(int offset,int limit);
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
            return _context.Vendors.OrderBy(v => v.VendorName).Where(v => !VendorExtensions.ReservedNames.Contains(v.VendorName.Trim())).ToList();
        }

        public List<Vendor> Execute(int offset, int limit)
        {
            return _context.Vendors.OrderBy(v => v.VendorName).Where(v => !VendorExtensions.ReservedNames.Contains(v.VendorName.Trim())).Skip(offset).Take(limit).ToList();
        }
    }
}
