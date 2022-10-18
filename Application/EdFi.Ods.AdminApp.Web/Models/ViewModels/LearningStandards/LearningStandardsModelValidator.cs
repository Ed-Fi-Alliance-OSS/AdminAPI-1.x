// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Admin.LearningStandards.Core;
using EdFi.Admin.LearningStandards.Core.Configuration;
using EdFi.Admin.LearningStandards.Core.Services.Interfaces;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.LearningStandards
{
    public class LearningStandardsModelValidator : AbstractValidator<LearningStandardsModel>
    {
        private readonly ILearningStandardsCorePluginConnector _learningStandardsPlugin;

        public LearningStandardsModelValidator(ILearningStandardsCorePluginConnector learningStandardsPlugin)

        {
            _learningStandardsPlugin = learningStandardsPlugin;

            RuleFor(m => m.ApiKey).NotEmpty();
            RuleFor(m => m.ApiSecret).NotEmpty();
            When(m => m.ApiKey != null && m.ApiSecret != null, () =>
            {
                RuleFor(x => x)
                    .SafeCustomAsync(
                        async (model, context) =>
                        {
                            var academicBenchmarkApiAuthConfig = GetAcademicBenchmarkApiAuthConfig(model.ApiKey, model.ApiSecret);

                            var validationResult = await ValidateAcademicBenchmarkApiAuthConfiguration(academicBenchmarkApiAuthConfig);

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

        private async Task<IResponse> ValidateAcademicBenchmarkApiAuthConfiguration(IAuthenticationConfiguration abConfig)
        {
            return await _learningStandardsPlugin.LearningStandardsConfigurationValidator
                .ValidateLearningStandardProviderConfigurationAsync(abConfig);
        }
    }
}
