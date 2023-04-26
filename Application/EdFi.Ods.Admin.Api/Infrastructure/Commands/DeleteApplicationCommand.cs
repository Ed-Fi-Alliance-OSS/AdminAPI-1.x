// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.AdminApp.Management.Database.Commands
{
    public interface IDeleteApplicationCommand
    {
        void Execute(int id);
    }

    public class DeleteApplicationCommand : IDeleteApplicationCommand
    {
        private readonly IUsersContext _context;

        public DeleteApplicationCommand(IUsersContext context)
        {
            _context = context;
        }

        public void Execute(int id)
        {
            var application = _context.Applications
                .Include(a => a.ApiClients)
                .Include(a => a.ApiClients.Select(c => c.ClientAccessTokens))
                .Include(a => a.ApplicationEducationOrganizations)
                .SingleOrDefault(a => a.ApplicationId == id);

            if (application == null)
            {
                throw new NotFoundException<int>("application", id);
            }
            if (application != null && application.Vendor.IsSystemReservedVendor())
            {
                throw new Exception("This Application is required for proper system function and may not be modified");
            }

            if (application == null)
            {
                return;
            }
            
            application.ApiClients.ToList().ForEach(a =>
            {
                a.ClientAccessTokens.ToList().ForEach(t => _context.ClientAccessTokens.Remove(t));
                _context.Clients.Remove(a);
            });

            application.ApplicationEducationOrganizations.ToList().ForEach(o => _context.ApplicationEducationOrganizations.Remove(o));

            _context.Applications.Remove(application);
            _context.SaveChanges();
        }
    }
}
