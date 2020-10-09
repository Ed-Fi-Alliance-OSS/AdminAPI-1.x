// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Configuration;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Ods.Reports;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.OdsInstanceServices
{
    public class OdsInstanceFirstTimeSetupService : IOdsInstanceFirstTimeSetupService
    {
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly IFirstTimeSetupService _firstTimeSetupService;
        private readonly IUsersContext _usersContext;
        private readonly IReportViewsSetUp _reportViewsSetUp;
        private readonly AdminAppDbContext _database;

        public OdsInstanceFirstTimeSetupService(IOdsSecretConfigurationProvider odsSecretConfigurationProvider, 
            IFirstTimeSetupService firstTimeSetupService, 
            IUsersContext usersContext,
            IReportViewsSetUp reportViewsSetUp,
            AdminAppDbContext database)
        {
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _firstTimeSetupService = firstTimeSetupService;
            _usersContext = usersContext;
            _reportViewsSetUp = reportViewsSetUp;
            _database = database;
        }

        public async Task CompleteSetup(OdsInstanceRegistration odsInstanceRegistration, CloudOdsClaimSet claimSet,
            ApiMode apiMode)
        {
            await AddOdsInstanceRegistration(odsInstanceRegistration);
            await CreateAndSaveApiKeyAndSecret(odsInstanceRegistration, claimSet, apiMode);
            _reportViewsSetUp.CreateReportViews(odsInstanceRegistration.Name, apiMode);
            _firstTimeSetupService.EnsureAdminDatabaseInitialized();
            await _usersContext.SaveChangesAsync();
        }

        private async Task CreateAndSaveApiKeyAndSecret(OdsInstanceRegistration odsInstanceRegistration, CloudOdsClaimSet claimSet, ApiMode mode)
        {
            var secretConfiguration = new OdsSecretConfiguration();

            var applicationCreateResult = await _firstTimeSetupService.CreateAdminAppInAdminDatabase(claimSet.ClaimSetName, odsInstanceRegistration.Name,
                    ConfigurationHelper.GetAppSettings().AwsCurrentVersion, mode);

            secretConfiguration.ProductionApiKeyAndSecret = applicationCreateResult.ProductionKeyAndSecret;
         
            await _odsSecretConfigurationProvider.SetSecretConfiguration(secretConfiguration, odsInstanceRegistration.Id);
        }

        private async Task AddOdsInstanceRegistration(OdsInstanceRegistration odsInstanceRegistration)
        {
            _database.OdsInstanceRegistrations.Add(odsInstanceRegistration);
            await _database.SaveChangesAsync();
        }
    }
}
