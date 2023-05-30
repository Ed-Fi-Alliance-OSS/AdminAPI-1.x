// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using VendorUser = EdFi.Admin.DataAccess.Models.User;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

public class EditVendorCommand
{
    private readonly IUsersContext _context;

    public EditVendorCommand(IUsersContext context)
    {
        _context = context;
    }

    public Vendor Execute(IEditVendor changedVendorData)
    {
        var vendor = _context.Vendors.SingleOrDefault(v => v.VendorId == changedVendorData.VendorId) ?? throw new NotFoundException<int>("vendor", changedVendorData.VendorId);

        if (vendor.IsSystemReservedVendor())
        {
            throw new Exception("This vendor is required for proper system function and may not be modified.");
        }

        vendor.VendorName = changedVendorData.Company;

        if (vendor.VendorNamespacePrefixes.Any())
        {
            foreach (var vendorNamespacePrefix in vendor.VendorNamespacePrefixes.ToList())
            {
                _context.VendorNamespacePrefixes.Remove(vendorNamespacePrefix);
            }
        }

        var namespacePrefixes = changedVendorData.NamespacePrefixes?.Split(",")
            .Where(namespacePrefix => !string.IsNullOrWhiteSpace(namespacePrefix))
            .Select(namespacePrefix => new VendorNamespacePrefix
            {
                NamespacePrefix = namespacePrefix.Trim(),
                Vendor = vendor
            });

        foreach (var namespacePrefix in namespacePrefixes ?? Enumerable.Empty<VendorNamespacePrefix>())
        {
            _context.VendorNamespacePrefixes.Add(namespacePrefix);
        }

        if (vendor.Users?.FirstOrDefault() != null)
        {
            vendor.Users.First().FullName = changedVendorData.ContactName;
            vendor.Users.First().Email = changedVendorData.ContactEmailAddress;
        }

        else
        {
            var vendorContact = new VendorUser
            {
                Vendor = vendor,
                FullName = changedVendorData.ContactName,
                Email = changedVendorData.ContactEmailAddress
            };
            vendor.Users = new List<VendorUser> { vendorContact };
        }

        _context.SaveChanges();
        return vendor;
    }
}

public interface IEditVendor
{
    int VendorId { get; set; }
    string? Company { get; set; }
    string? NamespacePrefixes { get; set; }
    string? ContactName { get; set; }
    string? ContactEmailAddress { get; set; }
}
