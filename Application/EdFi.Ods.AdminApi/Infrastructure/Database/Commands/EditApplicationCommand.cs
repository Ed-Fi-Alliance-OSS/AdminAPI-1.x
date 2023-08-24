// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

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
            .SingleOrDefault(a => a.ApplicationId == model.Id) ?? throw new NotFoundException<int>("application", model.Id);

        if (application.Vendor.IsSystemReservedVendor())
        {
            throw new Exception("This Application is required for proper system function and may not be modified");
        }

        var currentOdsInstanceId = application.OdsInstanceId();

        var newVendor = _context.Vendors.Single(v => v.VendorId == model.VendorId);
        var newProfiles = model.ProfileIds != null
            ? _context.Profiles.Where(p => model.ProfileIds.Contains(p.ProfileId))
            : null;
        var newOdsInstance = _context.OdsInstances.Single(o => o.OdsInstanceId == model.OdsInstanceId);

        var apiClient = application.ApiClients.Single();
        var currentApiClientId = apiClient.ApiClientId;
        apiClient.Name = model.ApplicationName;

        var apiClientOdsInstance = _context.ApiClientOdsInstances.FirstOrDefault(o => o.OdsInstance.OdsInstanceId == currentOdsInstanceId && o.ApiClient.ApiClientId == currentApiClientId);

        if (apiClientOdsInstance != null)
        {
            _context.ApiClientOdsInstances.Remove(apiClientOdsInstance);
        }

        application.ApplicationName = model.ApplicationName;
        application.ClaimSetName = model.ClaimSetName;
        application.Vendor = newVendor;
        application.OdsInstance = newOdsInstance;

        application.ApplicationEducationOrganizations ??= new Collection<ApplicationEducationOrganization>();

        application.ApplicationEducationOrganizations.Clear();
        model.EducationOrganizationIds?.ToList().ForEach(id => application.ApplicationEducationOrganizations.Add(application.CreateApplicationEducationOrganization(id)));

        application.Profiles ??= new Collection<Profile>();

        application.Profiles.Clear();

        if (newProfiles != null)
        {
            foreach (var profile in newProfiles)
            {
                application.Profiles.Add(profile);
            }
        }

        _context.ApiClientOdsInstances.Add(new ApiClientOdsInstance { ApiClient = apiClient, OdsInstance = newOdsInstance });

        _context.SaveChanges();
        return application;
    }
}

public interface IEditApplicationModel
{
    int Id { get; }
    string? ApplicationName { get; }
    int VendorId { get; }
    string? ClaimSetName { get; }
    IEnumerable<int>? ProfileIds { get; }
    IEnumerable<int>? EducationOrganizationIds { get; }
    int OdsInstanceId { get; }
}
