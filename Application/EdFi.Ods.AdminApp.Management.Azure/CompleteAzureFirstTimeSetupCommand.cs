// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Security.DataAccess.Contexts;
using Action = System.Action;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class CompleteAzureFirstTimeSetupCommand : ICompleteOdsFirstTimeSetupCommand
    {
        private readonly IUsersContext _usersContext;
        private readonly IAzureSqlSecurityConfigurator _cloudOdsSqlConfigurator;
        private readonly ISecurityContext _securityContext;
        private readonly ICloudOdsClaimSetConfigurator _cloudOdsClaimSetConfigurator;
        private readonly IGetCloudOdsInstanceQuery _getCloudOdsInstanceQuery;
        private readonly IGetAzureCloudOdsHostedComponentsQuery _getCloudOdsHostedComponentsQuery;
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly ICloudOdsDatabaseSqlServerSecurityConfiguration _cloudOdsDatabaseSqlServerSecurityConfiguration;
        private readonly IOdsInstanceFirstTimeSetupService _odsInstanceFirstTimeSetupService;
        private readonly IRestartAppServicesCommand _restartAppServicesCommand;
        private readonly IAssessmentVendorAdjustment _assessmentVendorAdjustment;
        private readonly ILearningStandardsSetup _learningStandardsSetup;
        private readonly IClaimSetCheckService _claimSetCheckService;

        public Action ExtraDatabaseInitializationAction { get; set; }

        public CompleteAzureFirstTimeSetupCommand(
            IUsersContext usersContext,
            IAzureSqlSecurityConfigurator cloudOdsSqlConfigurator,
            ISecurityContext securityContext,
            ICloudOdsClaimSetConfigurator cloudOdsClaimSetConfigurator,
            IGetCloudOdsInstanceQuery getCloudOdsInstanceQuery,
            IGetAzureCloudOdsHostedComponentsQuery getCloudOdsHostedComponentQuery,
            IOdsSecretConfigurationProvider odsSecretConfigurationProvider,
            ICloudOdsDatabaseSqlServerSecurityConfiguration cloudOdsDatabaseSqlServerSecurityConfiguration,
            IOdsInstanceFirstTimeSetupService odsInstanceFirstTimeSetupService,
            IRestartAppServicesCommand restartAppServicesCommand,
            IAssessmentVendorAdjustment assessmentVendorAdjustment,
            ILearningStandardsSetup learningStandardsSetup,
            IClaimSetCheckService claimSetCheckService)
        {
            _restartAppServicesCommand = restartAppServicesCommand;
            _assessmentVendorAdjustment = assessmentVendorAdjustment;
            _learningStandardsSetup = learningStandardsSetup;
            _claimSetCheckService = claimSetCheckService;
            _usersContext = usersContext;
            _cloudOdsSqlConfigurator = cloudOdsSqlConfigurator;
            _securityContext = securityContext;
            _cloudOdsClaimSetConfigurator = cloudOdsClaimSetConfigurator;
            _getCloudOdsInstanceQuery = getCloudOdsInstanceQuery;
            _getCloudOdsHostedComponentsQuery = getCloudOdsHostedComponentQuery;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _cloudOdsDatabaseSqlServerSecurityConfiguration = cloudOdsDatabaseSqlServerSecurityConfiguration;
            _odsInstanceFirstTimeSetupService = odsInstanceFirstTimeSetupService;
        }
        
        public async Task<bool> Execute(string odsInstanceName, CloudOdsClaimSet claimSet, ApiMode apiMode)
        {
            var odsSqlConfiguration = await _odsSecretConfigurationProvider.GetSqlConfiguration();
            var cloudOdsInstance = await _getCloudOdsInstanceQuery.Execute(odsInstanceName);
            var firstTimeSetupConfiguration = await GetFirstTimeSetupConfiguration(cloudOdsInstance, claimSet, odsSqlConfiguration);
            var restartRequired = false;

            SetupAndRuntimeConfigurations(firstTimeSetupConfiguration);

            if (apiMode.SupportsSingleInstance)
            {
                var defaultOdsInstance = new OdsInstanceRegistration
                {
                    Name = odsInstanceName,
                    Description = "Default single ods instance"
                };
                await _odsInstanceFirstTimeSetupService.CompleteSetup(defaultOdsInstance, claimSet, apiMode);
            }


            if (!_claimSetCheckService.RequiredClaimSetsExist())
            {
                CreateClaimSetForAdminApp(firstTimeSetupConfiguration.ClaimSet);

                ApplyAdditionalClaimSetModifications();

                restartRequired = true;
            }

            await _usersContext.SaveChangesAsync();
            await _securityContext.SaveChangesAsync();
            await _restartAppServicesCommand.Execute(new CloudOdsApiOperationContext(cloudOdsInstance));

            return restartRequired;
        }

        private void SetupAndRuntimeConfigurations(OdsFirstTimeSetupConfiguration firstTimeSetupConfiguration)
        {
            _cloudOdsSqlConfigurator.CreateServerLogins(firstTimeSetupConfiguration.SqlConfiguration);

            var setupConfiguration =
                _cloudOdsDatabaseSqlServerSecurityConfiguration.GetSetupConfiguration(firstTimeSetupConfiguration
                    .SqlConfiguration).ToList();
         
            var runtimeConfiguration =
                _cloudOdsDatabaseSqlServerSecurityConfiguration.GetRuntimeConfiguration(firstTimeSetupConfiguration
                    .SqlConfiguration);

            _cloudOdsSqlConfigurator.ApplyConfiguration(firstTimeSetupConfiguration.SqlConfiguration, setupConfiguration);
            _cloudOdsSqlConfigurator.ApplyConfiguration(firstTimeSetupConfiguration.SqlConfiguration, runtimeConfiguration);

            ExtraDatabaseInitializationAction?.Invoke();

            _cloudOdsSqlConfigurator.RemoveConfiguration(firstTimeSetupConfiguration.SqlConfiguration, setupConfiguration);
        }

        private async Task<OdsFirstTimeSetupConfiguration> GetFirstTimeSetupConfiguration(CloudOdsInstance cloudOdsInstance, CloudOdsClaimSet claimSet, OdsSqlConfiguration odsSqlConfiguration)
        {
            return new OdsFirstTimeSetupConfiguration
            {
                Name = cloudOdsInstance.FriendlyName,
                Version = cloudOdsInstance.Version,
                Components = await _getCloudOdsHostedComponentsQuery.Execute(cloudOdsInstance),
                SqlConfiguration = odsSqlConfiguration,
                ClaimSet = claimSet
            };
        }

        private void CreateClaimSetForAdminApp(CloudOdsClaimSet cloudOdsClaimSet)
        {
            _cloudOdsClaimSetConfigurator.ApplyConfiguration(cloudOdsClaimSet);
        }

        private void ApplyAdditionalClaimSetModifications()
        {
            _learningStandardsSetup.SetupLearningStandardsClaims();

            _assessmentVendorAdjustment.ReadAndCreatePerformanceLevelDescriptors();
        }

        private OdsInstance RegisterOdsInstance(OdsFirstTimeSetupConfiguration configuration)
        {
            var existingInstance = _usersContext.OdsInstances.SingleOrDefault(x => x.Name == configuration.Name);
            if (existingInstance != null)
                return existingInstance;

            var instance = new OdsInstance
            {
                InstanceType = "Cloud",
                IsExtended = false,
                Name = configuration.Name,
                Status = CloudOdsStatus.SetupRequired.DisplayName,
                Version = configuration.Version,
                OdsInstanceComponents = configuration.Components.Select(c => new OdsInstanceComponent
                {
                    Name = $"{c.Name} ({c.Environment})",
                    Url = c.Url,
                    Version = c.Version
                }).ToList()
            };

            _usersContext.OdsInstances.Add(instance);
            return instance;
        }
    }
}
