// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using VendorUser = EdFi.Admin.DataAccess.Models.User;

namespace EdFi.Ods.AdminApp.Management.Database.Commands
{
    public class AddVendorCommand
    {
        private readonly IUsersContext _context;

        public AddVendorCommand(IUsersContext context)
        {
            _context = context;
        }

        public Vendor Execute(IAddVendorModel newVendor)
        {
            var namespacePrefixes = new List<VendorNamespacePrefix>();
            if (!string.IsNullOrWhiteSpace(newVendor.NamespacePrefixes))
            {
                var namespacePrefixSplits = newVendor.NamespacePrefixes.Split(",");

                namespacePrefixes.AddRange(namespacePrefixSplits.Select(
                        namespacePrefix => new VendorNamespacePrefix
                        {
                            NamespacePrefix = namespacePrefix.Trim()
                        }));
            }

            var vendor = new Vendor
            {
                VendorName = newVendor.Company.Trim(),
                VendorNamespacePrefixes = namespacePrefixes
            };

            var user = new VendorUser
            {
                FullName = newVendor.ContactName.Trim(),
                Email = newVendor.ContactEmailAddress.Trim()
            };

            vendor.Users.Add(user);

            _context.Vendors.Add(vendor);
            _context.SaveChanges();
            return vendor;
        }
    }

    public interface IAddVendorModel
    {
        string Company { get; }
        string NamespacePrefixes { get; }
        string ContactName { get; }
        string ContactEmailAddress { get; }
    }
}
