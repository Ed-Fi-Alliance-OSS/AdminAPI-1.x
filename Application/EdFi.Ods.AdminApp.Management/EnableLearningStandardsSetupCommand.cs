// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management
{
    public class AcademicBenchmarkConfig
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public ApiMode OdsApiMode { get; set; }
    }

    public class EnableLearningStandardsSetupCommand : IEnableLearningStandardsSetupCommand
    {
        private readonly ISetupAcademicBenchmarksConnectService _academicBenchmarksConnectService;
        private readonly IUsersContext _usersContext;
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly InstanceContext _instanceContext;

        public EnableLearningStandardsSetupCommand(
            ISetupAcademicBenchmarksConnectService academicBenchmarksConnectService,
            IUsersContext usersContext,
            IOdsSecretConfigurationProvider odsSecretConfigurationProvider,
            InstanceContext instanceContext)
        {
            _academicBenchmarksConnectService = academicBenchmarksConnectService;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _instanceContext = instanceContext;
            _usersContext = usersContext;
        }

        public async Task Execute(AcademicBenchmarkConfig options)
        {
            await CreateAcademicBenchmarksAppAndApiClient(options.OdsApiMode);

            await SaveAcademicBenchmarkKeyAndSecret(options.ApiKey, options.ApiSecret);

            _usersContext.SaveChanges();
        }

        private async Task CreateAcademicBenchmarksAppAndApiClient(ApiMode apiMode)
        {
            await _academicBenchmarksConnectService.CreateAcademicBenchmarksConnectAppInAdminDatabase(apiMode);
        }

        private async Task SaveAcademicBenchmarkKeyAndSecret(string apiKey, string apiSecret)
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);

            config.LearningStandardsCredential = new ABProxyApiCredential
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret
            };

            await _odsSecretConfigurationProvider.SetSecretConfiguration(config, _instanceContext.Id);
        }
    }
}