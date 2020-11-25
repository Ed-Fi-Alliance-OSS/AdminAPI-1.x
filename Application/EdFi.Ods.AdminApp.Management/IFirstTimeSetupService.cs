// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Common.Security;

namespace EdFi.Ods.AdminApp.Management
{
    public interface IFirstTimeSetupService
    {
        Task<ApplicationCreateResult> CreateAdminAppInAdminDatabase(string claimSetClaimSetName, string odsInstanceName, string odsInstanceVersion, ApiMode apiMode);
        void EnsureAdminDatabaseInitialized();
    }

    public class ApplicationCreateResult
    {
        public Application Application { get; set; }
        public OdsApiCredential ProductionKeyAndSecret { get; set; }
    }

    public abstract class FirstTimeSetupServiceBase : IFirstTimeSetupService
    {
        protected readonly IUsersContext UsersContext;
        private readonly ISecurePackedHashProvider _securePackedHashProvider;
        private readonly IHashConfigurationProvider _hashConfigurationProvider;

        protected FirstTimeSetupServiceBase(IUsersContext usersContext, ISecurePackedHashProvider securePackedHashProvider, IHashConfigurationProvider hashConfigurationProvider)
        {
            UsersContext = usersContext;
            _securePackedHashProvider = securePackedHashProvider;
            _hashConfigurationProvider = hashConfigurationProvider;
        }

        public abstract void EnsureAdminDatabaseInitialized();

        public async Task<ApplicationCreateResult> CreateAdminAppInAdminDatabase(string claimSetClaimSetName,
            string odsInstanceName, string odsInstanceVersion, ApiMode apiMode)
        {
            var applicationName = odsInstanceName.GetAdminApplicationName();

            var existingApplication = await UsersContext.Applications.SingleOrDefaultAsync(x =>
                x.ApplicationName.Equals(applicationName,
                    StringComparison.InvariantCultureIgnoreCase));

            if (existingApplication != null)
            {
                return new ApplicationCreateResult
                {
                    Application = existingApplication
                };
            }

            var result = new ApplicationCreateResult
            {
                Application = new Application
                {
                    ApplicationName = applicationName,
                    Vendor = CreateEdFiVendor(),
                    ClaimSetName = claimSetClaimSetName,
                    OdsInstance = CreateOdsInstance(odsInstanceName, odsInstanceVersion),
                    OperationalContextUri = OperationalContext.DefaultOperationalContextUri
                }
            };

            var apiClientFactory = new ApiClientFactory(_securePackedHashProvider, _hashConfigurationProvider);

            var apiWithCredentials = apiClientFactory.GetApiClientAndCredentials(applicationName);

            result.Application.ApiClients.Add(apiWithCredentials.ApiClient);
            result.ProductionKeyAndSecret = apiWithCredentials.ApiCredentials;

            if (apiMode.Equals(ApiMode.DistrictSpecific))
            {
                var edOrgId = OdsInstanceIdentityHelper.GetIdentityValue(odsInstanceName);

                var applicationEdOrgs = new List<ApplicationEducationOrganization>
                {
                    new ApplicationEducationOrganization
                    {
                        Clients = new List<ApiClient> {apiWithCredentials.ApiClient},
                        EducationOrganizationId = edOrgId
                    }
                };

                result.Application.ApplicationEducationOrganizations =
                    new List<ApplicationEducationOrganization>(applicationEdOrgs);
            }

            UsersContext.Applications.Add(result.Application);

            return result;
        }

        private OdsInstance CreateOdsInstance(string odsInstanceName, string odsInstanceVersion)
        {
            var existingInstance = UsersContext.OdsInstances.SingleOrDefault(x => x.Name == odsInstanceName);
            if (existingInstance != null)
                return existingInstance;

            var instance = new OdsInstance
            {
                InstanceType = "CloudOds",
                IsExtended = false,
                Name = odsInstanceName,
                Status = CloudOdsStatus.Ok.DisplayName,
                Version = odsInstanceVersion
            };

            UsersContext.OdsInstances.Add(instance);

            return instance;
        }

        private Vendor CreateEdFiVendor()
        {
            var existingVendor = UsersContext.Vendors.SingleOrDefault(x => x.VendorName.Equals(CloudOdsAdminApp.VendorName, StringComparison.InvariantCultureIgnoreCase));
            if (existingVendor != null)
                return existingVendor;

            var vendor = new Vendor
            {
                VendorNamespacePrefixes = new List<VendorNamespacePrefix> { new VendorNamespacePrefix { NamespacePrefix = CloudOdsAdminApp.VendorNamespacePrefix } },
                VendorName = CloudOdsAdminApp.VendorName
            };

            UsersContext.Vendors.Add(vendor);

            return vendor;
        }
    }
}
