// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using VendorUser = EdFi.Admin.DataAccess.Models.User;

namespace EdFi.Ods.AdminApp.Management.Database.Commands
{
    public interface IAddApplicationCommand
    {
        AddApplicationResult Execute(IAddApplicationModel applicationModel);
    }

    public class AddApplicationCommand : IAddApplicationCommand
    {
        private readonly IUsersContext _usersContext;
        private readonly InstanceContext _instanceContext;

        public AddApplicationCommand(IUsersContext usersContext, InstanceContext instanceContext)
        {
            _usersContext = usersContext;
            _instanceContext = instanceContext;
        }

        public AddApplicationResult Execute(IAddApplicationModel applicationModel)
        {
            var profile = applicationModel.ProfileId.HasValue
                ? _usersContext.Profiles.SingleOrDefault(p => p.ProfileId == applicationModel.ProfileId.Value)
                : null;

            var vendor = _usersContext.Vendors.Single(v => v.VendorId == applicationModel.VendorId);            

            var odsInstance = _usersContext.OdsInstances.FirstOrDefault(x =>
                x.Name.Equals(_instanceContext.Name, StringComparison.InvariantCultureIgnoreCase));

            var user = new VendorUser
            {
                Email = "",                
                FullName = applicationModel.ApplicationName,
                Vendor = vendor
            };

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

            if (profile != null)
            {
                application.Profiles.Add(profile);
            }

            _usersContext.Applications.Add(application);
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
        string ApplicationName { get; }
        int VendorId { get; }
        string ClaimSetName { get; }
        int? ProfileId { get; }
        IEnumerable<int> EducationOrganizationIds { get; } 
    }

    public class AddApplicationResult
    {
        public int ApplicationId { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
    }
}
