// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Admin.LearningStandards.Core;
using EdFi.Admin.LearningStandards.Core.Configuration;
using EdFi.Admin.LearningStandards.Core.Services.Interfaces;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.LearningStandards
{
    public class LearningStandardsModelValidator : AbstractValidator<LearningStandardsModel>
    {
        private readonly ILearningStandardsCorePluginConnector _learningStandardsPlugin;
        private readonly InstanceContext _instanceContext;
        private readonly ICloudOdsAdminAppSettingsApiModeProvider _cloudOdsAdminAppSettingsApiModeProvider;

        public LearningStandardsModelValidator(
            IOdsApiConnectionInformationProvider apiConnectionInformationProvider
            , IOdsSecretConfigurationProvider odsSecretConfigurationProvider
            , ILearningStandardsCorePluginConnector learningStandardsPlugin
            , ICloudOdsAdminAppSettingsApiModeProvider cloudOdsAdminAppSettingsApiModeProvider
            , InstanceContext instanceContext)

        {
            _learningStandardsPlugin = learningStandardsPlugin;
            _cloudOdsAdminAppSettingsApiModeProvider = cloudOdsAdminAppSettingsApiModeProvider;
            _instanceContext = instanceContext;

            RuleFor(m => m.ApiKey).NotEmpty();
            RuleFor(m => m.ApiSecret).NotEmpty();
            When(m => m.ApiKey != null && m.ApiSecret != null, () =>
            {
                RuleFor(x => x)
                    .SafeCustomAsync(
                        async (model, context) =>
                        {
                            var connectionInformation = await apiConnectionInformationProvider.GetConnectionInformationForEnvironment();
                            var configuration = await odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);

                            var jobContext = GetJobContext(connectionInformation);
                            var edFiOdsApiConfiguration = GetEdFiOdsApiConfig(jobContext, configuration);

                            var academicBenchmarkApiAuthConfig = GetAcademicBenchmarkApiAuthConfig(model.ApiKey, model.ApiSecret);

                            var validationResult =
                                await ValidateConfiguration(edFiOdsApiConfiguration, academicBenchmarkApiAuthConfig);

                            if (!validationResult.IsSuccess)
                            {
                                context.AddFailure(validationResult.ErrorMessage);
                            }
                        });
            });
        }

        private AuthenticationConfiguration GetAcademicBenchmarkApiAuthConfig(string academicBenchmarkApiKey, string academicBenchmarkApiSecret)
        {
            return new AuthenticationConfiguration(
                academicBenchmarkApiKey,
                academicBenchmarkApiSecret);
        }

        private EdFiOdsApiConfiguration GetEdFiOdsApiConfig(LearningStandardsJobContext context,
            OdsSecretConfiguration configuration)
        {
            var productionApiKey = configuration.ProductionAcademicBenchmarkApiClientKeyAndSecret?.ApiKey ??
                                   string.Empty;
            var productionApiSecret = configuration.ProductionAcademicBenchmarkApiClientKeyAndSecret?.ApiSecret ??
                                      string.Empty;

            var oAuthAuthenticationConfiguration = new AuthenticationConfiguration(
                productionApiKey,
                productionApiSecret);

            return new EdFiOdsApiConfiguration(
                context.ApiUrl, EdFiOdsApiCompatibilityVersion.v3,
                oAuthAuthenticationConfiguration, context.SchoolYear);
        }

        private LearningStandardsJobContext GetJobContext(OdsApiConnectionInformation connectionInformation)
        {
            int? schoolYear = null;

            if (_cloudOdsAdminAppSettingsApiModeProvider.GetApiMode() == ApiMode.YearSpecific)
            {
                schoolYear = _instanceContext.Name.ExtractNumericInstanceSuffix();
            }

            var jobContext = new LearningStandardsJobContext
            {
                ApiUrl = connectionInformation.ApiServerUrl,
                OdsInstanceId = _instanceContext.Id,
                SchoolYear = schoolYear
            };

            return jobContext;
        }

        private async Task<IResponse> ValidateConfiguration(IEdFiOdsApiConfiguration apiConfig,
            IAuthenticationConfiguration abConfig)
        {
            var abresult = await _learningStandardsPlugin.LearningStandardsConfigurationValidator
                .ValidateLearningStandardProviderConfigurationAsync(abConfig);

            if (!abresult.IsSuccess)
            {
                return abresult;
            }

            return await _learningStandardsPlugin.LearningStandardsConfigurationValidator
                .ValidateEdFiOdsApiConfigurationAsync(
                    apiConfig);
        }
    }
}
