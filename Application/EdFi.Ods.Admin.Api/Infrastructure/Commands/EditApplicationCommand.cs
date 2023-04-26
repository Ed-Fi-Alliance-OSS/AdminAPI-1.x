// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.AdminApp.Management.Database.Commands
{
    public interface IEditApplicationCommand
    {
        Application Execute(IEditApplicationModel model);
    }

    public class EditApplicationCommand : IEditApplicationCommand
    {
        private readonly IUsersContext _context;

        public EditApplicationCommand(IUsersContext context)
        {
            _context = context;
        }

        public Application Execute(IEditApplicationModel model)
        {
            var application = _context.Applications
                .Include(a => a.Vendor)
                .Include(a => a.ApplicationEducationOrganizations)
                .Include(a => a.ApiClients)
                .Include(a => a.Profiles)
                .SingleOrDefault(a => a.ApplicationId == model.ApplicationId);

            if (application == null)
            {
                throw new NotFoundException<int>("application", model.ApplicationId);
            }

            if(application.Vendor.IsSystemReservedVendor())
            {
                throw new Exception("This Application is required for proper system function and may not be modified");
            }

            var newVendor = _context.Vendors.Single(v => v.VendorId == model.VendorId);
            var newProfile = model.ProfileId.HasValue
                ? _context.Profiles.Single(p => p.ProfileId == model.ProfileId.Value)
                : null;

            var apiClient = application.ApiClients.Single();
            apiClient.Name = model.ApplicationName;

            application.ApplicationName = model.ApplicationName;
            application.ClaimSetName = model.ClaimSetName;
            application.Vendor = newVendor;

            if (application.ApplicationEducationOrganizations == null)
            {
                application.ApplicationEducationOrganizations = new Collection<ApplicationEducationOrganization>();
            }

            application.ApplicationEducationOrganizations.Clear();
            model.EducationOrganizationIds.ToList().ForEach(id => application.ApplicationEducationOrganizations.Add(application.CreateApplicationEducationOrganization(id)));

            if (application.Profiles == null)
            {
                application.Profiles = new Collection<Profile>();
            }

            application.Profiles.Clear();

            if (newProfile != null)
            {
                application.Profiles.Add(newProfile);
            }

            _context.SaveChanges();
            return application;
        }
    }

    public interface IEditApplicationModel
    {
        int ApplicationId { get; }
        string ApplicationName { get; }
        int VendorId { get; }
        string ClaimSetName { get; }
        int? ProfileId { get; }
        IEnumerable<int> EducationOrganizationIds { get; }
    }
}
