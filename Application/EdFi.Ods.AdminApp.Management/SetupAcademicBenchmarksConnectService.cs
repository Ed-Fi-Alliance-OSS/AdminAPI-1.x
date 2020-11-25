// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Common.Security;

namespace EdFi.Ods.AdminApp.Management
{
    public class SetupAcademicBenchmarksConnectService : ISetupAcademicBenchmarksConnectService
    {
        private readonly IUsersContext _usersContext;
        private readonly ISecurePackedHashProvider _securePackedHashProvider;
        private readonly IHashConfigurationProvider _hashConfigurationProvider;
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly InstanceContext _instanceContext;

        public SetupAcademicBenchmarksConnectService(IUsersContext usersContext
            , ISecurePackedHashProvider securePackedHashProvider
            , IHashConfigurationProvider hashConfigurationProvider
            , IOdsSecretConfigurationProvider odsSecretConfigurationProvider
            , InstanceContext instanceContext
        )
        {
            _usersContext = usersContext;
            _securePackedHashProvider = securePackedHashProvider;
            _hashConfigurationProvider = hashConfigurationProvider;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _instanceContext = instanceContext;
        }

        public async Task CreateAcademicBenchmarksConnectAppInAdminDatabase(ApiMode apiMode)
        {
            var applicationName = $"{_instanceContext.Name}_{CloudsOdsAcademicBenchmarksConnectApp.ApplicationName}";
            var existingApplication = _usersContext.Applications.SingleOrDefault(x =>
                x.ApplicationName.Equals(applicationName,
                    StringComparison.InvariantCultureIgnoreCase));

            if (existingApplication != null) return;

            var instance = _usersContext.OdsInstances.SingleOrDefault(x =>
                x.Name.Equals(_instanceContext.Name, StringComparison.InvariantCultureIgnoreCase));

            var newApplication = new Application
            {
                ApplicationName = applicationName,
                Vendor = CreateCerticaVendor(),
                OdsInstance = instance,
                ClaimSetName = CloudsOdsAcademicBenchmarksConnectApp.DefaultClaimSet,
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri
            };

            await CreateAndSaveNewApiClients(newApplication);
         
            if (apiMode.Equals(ApiMode.DistrictSpecific))
            {
                var edOrgId = OdsInstanceIdentityHelper.GetIdentityValue(_instanceContext.Name);
                newApplication.ApplicationEducationOrganizations =
                    new List<ApplicationEducationOrganization>
                    {
                        new ApplicationEducationOrganization
                        {
                            Clients = newApplication.ApiClients,
                            EducationOrganizationId = edOrgId
                        }
                    };
            }

            _usersContext.Applications.Add(newApplication);
        }

        private Vendor CreateCerticaVendor()
        {
            var existingVendor = _usersContext.Vendors.SingleOrDefault(x =>
                x.VendorName.Equals(CloudsOdsAcademicBenchmarksConnectApp.VendorName,
                    StringComparison.InvariantCultureIgnoreCase));

            if (existingVendor != null)
                return existingVendor;

            return new Vendor
            {
                VendorNamespacePrefixes = new[]
                {
                    new VendorNamespacePrefix
                    {
                        NamespacePrefix = CloudsOdsAcademicBenchmarksConnectApp.VendorNamespacePrefix
                    }
                },
                VendorName = CloudsOdsAcademicBenchmarksConnectApp.VendorName
            };
        }

        private async Task CreateAndSaveNewApiClients(Application newApplication)
        {
            var apiCreds = CreateNewApiClient(newApplication);
            await StoreApiCredentials(apiCreds);
        }

        private OdsApiCredential CreateNewApiClient(Application newApplication)
        {
            var apiWithCredentials =
                new ApiClientFactory(_securePackedHashProvider, _hashConfigurationProvider)
                    .GetApiClientAndCredentials(CloudsOdsAcademicBenchmarksConnectApp.ApplicationName);

            newApplication.ApiClients.Add(apiWithCredentials.ApiClient);

            return apiWithCredentials.ApiCredentials;
        }

        private async Task StoreApiCredentials(OdsApiCredential apiCreds)
        {
            var odsSecretConfiguration = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);

            odsSecretConfiguration.ProductionAcademicBenchmarkApiClientKeyAndSecret =
                new ABProxyApiCredential
                {
                    ApiKey = apiCreds.Key,
                    ApiSecret = apiCreds.Secret
                };

            await _odsSecretConfigurationProvider.SetSecretConfiguration(odsSecretConfiguration, _instanceContext.Id);
        }
    }
}
