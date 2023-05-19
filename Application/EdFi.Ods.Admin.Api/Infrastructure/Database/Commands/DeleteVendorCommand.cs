// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.AdminApp.Management.Database.Commands
{
    public class DeleteVendorCommand
    {
        private readonly IUsersContext _context;
        private readonly IDeleteApplicationCommand _deleteApplicationCommand;

        public DeleteVendorCommand(IUsersContext context, IDeleteApplicationCommand deleteApplicationCommand)
        {
            _context = context;
            _deleteApplicationCommand = deleteApplicationCommand;
        }

        public void Execute(int id)
        {
            var vendor = _context.Vendors.SingleOrDefault(v => v.VendorId == id);

            if (vendor == null)
            {
                throw new NotFoundException<int>("vendor", id);
            }
            if (vendor.IsSystemReservedVendor())
            {
                throw new Exception("This Vendor is required for proper system function and may not be deleted");
            }

            foreach (var application in vendor.Applications.ToList())
            {
                _deleteApplicationCommand.Execute(application.ApplicationId);
            }

            foreach (var user in vendor.Users.ToList())
            {
                _context.Users.Remove(user);
            }

            _context.Vendors.Remove(vendor);
            _context.SaveChanges();
        }
    }
}
