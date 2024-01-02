// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using System.Data.Entity;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

public interface IAddApplicationCommand
{
    AddApplicationResult Execute(IAddApplicationModel applicationModel);
}

public class AddApplicationCommand : IAddApplicationCommand
{
    private readonly IUsersContext _usersContext;

    public AddApplicationCommand(IUsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public AddApplicationResult Execute(IAddApplicationModel applicationModel)
    {
        var profiles = applicationModel.ProfileIds != null
            ? _usersContext.Profiles.Where(p => applicationModel.ProfileIds!.Contains(p.ProfileId))
            : null;

        var vendor = _usersContext.Vendors.Include(x => x.Users)
            .Single(v => v.VendorId == applicationModel.VendorId);

        var odsInstance = _usersContext.OdsInstances.Single(o => o.OdsInstanceId == applicationModel.OdsInstanceId);

        var user = vendor.Users.FirstOrDefault();

        var apiClient = new ApiClient(true)
        {
            Name = applicationModel.ApplicationName,
            IsApproved = true,
            UseSandbox = false,
            KeyStatus = "Active",
            User = user
        };

        var applicationEdOrgs = applicationModel.EducationOrganizationIds == null
            ? Enumerable.Empty<ApplicationEducationOrganization>()
            : applicationModel.EducationOrganizationIds.Select(id => new ApplicationEducationOrganization
            {
                Clients = new List<ApiClient> { apiClient },
                EducationOrganizationId = id
            });

        var application = new Application
        {
            ApplicationName = applicationModel.ApplicationName,
            ApiClients = new List<ApiClient> { apiClient },
            ApplicationEducationOrganizations = new List<ApplicationEducationOrganization>(applicationEdOrgs),
            ClaimSetName = applicationModel.ClaimSetName,
            Profiles = new List<Profile>(),
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
            OdsInstance = odsInstance
        };

        if (profiles != null)
        {
            foreach (var profile in profiles)
            {
                application.Profiles.Add(profile);
            }
        }

        _usersContext.Applications.Add(application);
        _usersContext.ApiClientOdsInstances.Add(new ApiClientOdsInstance { ApiClient = apiClient, OdsInstance = odsInstance });
        _usersContext.SaveChanges();

        return new AddApplicationResult
        {
            ApplicationId = application.ApplicationId,
            Key = apiClient.Key,
            Secret = apiClient.Secret
        };
    }
}

public interface IAddApplicationModel
{
    string? ApplicationName { get; }
    int VendorId { get; }
    string? ClaimSetName { get; }
    IEnumerable<int>? ProfileIds { get; }
    IEnumerable<int>? EducationOrganizationIds { get; }
    int OdsInstanceId { get; }
}

public class AddApplicationResult
{
    public int ApplicationId { get; set; }
    public string? Key { get; set; }
    public string? Secret { get; set; }
}
