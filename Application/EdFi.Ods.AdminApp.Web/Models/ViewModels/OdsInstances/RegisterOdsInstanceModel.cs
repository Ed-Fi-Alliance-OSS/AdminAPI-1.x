// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances
{
    public class RegisterOdsInstanceModel: IRegisterOdsInstanceModel
    {
        public int? NumericSuffix { get; set; }

        [Display(Name="ODS Instance Description")]
        public string Description { get; set; }
    }

    public class RegisterOdsInstanceModelValidator : AbstractValidator<RegisterOdsInstanceModel>
    {
        private static AdminAppDbContext _database;
        private static IDatabaseValidationService _databaseValidationService;
        private static ApiMode _mode;

        public RegisterOdsInstanceModelValidator(AdminAppDbContext database
            , ICloudOdsAdminAppSettingsApiModeProvider apiModeProvider
            , IDatabaseValidationService databaseValidationService
            , bool validationMessageWithDetails = false)
        {
            _database = database;
            _databaseValidationService = databaseValidationService;
            var requiredNumericSuffixMessage = "'ODS Instance District / EdOrg Id' must not be empty.";
            var inRangeMessage = "'ODS Instance District / EdOrg Id' must be a valid positive integer.";
            var maxValue = int.MaxValue;
            var minValue = 1;
            var beUniqueValidationMsg = "Education Organization / District Id";
            _mode = apiModeProvider.GetApiMode();

            if (_mode == ApiMode.YearSpecific)
            {
                requiredNumericSuffixMessage = "'ODS Instance School Year' must not be empty.";
                maxValue = 2099;
                inRangeMessage = "'ODS Instance School Year' must be between 1900 and 2099.";
                beUniqueValidationMsg = "school year";
                minValue = 1900;
            }

            RuleFor(m => m.NumericSuffix).NotEmpty().WithMessage(requiredNumericSuffixMessage);

            RuleFor(m => m.NumericSuffix)
                .Must(x=> x <= maxValue && x >= minValue)
                .When(x => x.NumericSuffix != null)
                .WithMessage(inRangeMessage);

            RuleFor(m => m.NumericSuffix)
                .Must(BeAUniqueInstanceName)
                .When(x => x.NumericSuffix != null)
                .WithMessage(
                    x => $"An instance for this {beUniqueValidationMsg}{(validationMessageWithDetails ? $" ({x.NumericSuffix})":"")} already exists.");

            RuleFor(m => m.NumericSuffix)
                .Must(BeValidOdsInstanceDatabase)
                .When(x => x.NumericSuffix != null)
                .WithMessage(
                    x => $"Could not connect to an ODS instance database for this {beUniqueValidationMsg}{(validationMessageWithDetails ? $" ({x.NumericSuffix})":"")}.");

            RuleFor(m => m.Description).NotEmpty();

            RuleFor(m => m.Description)
                .Must(BeAUniqueInstanceDescription)
                .When(x => x.Description != null)
                .WithMessage(
                    x => $"An instance with this description{(validationMessageWithDetails ? $" ({beUniqueValidationMsg}: {x.NumericSuffix}, Description: {x.Description})":"")} already exists.");
        }

        private static bool BeValidOdsInstanceDatabase(int? newInstanceNumericSuffix)
        {
            return _databaseValidationService.IsValidDatabase(newInstanceNumericSuffix.Value, _mode);
        }

        private static bool BeAUniqueInstanceName(int? newInstanceNumericSuffix)
        {
            var newInstanceName = newInstanceNumericSuffix.ToString();
            return newInstanceName != "" && !_database.OdsInstanceRegistrations.Any(x => x.Name == newInstanceName);
        }

        private static bool BeAUniqueInstanceDescription(string newInstanceDescription)
        {
            var trim = newInstanceDescription.Trim();

            return !_database.OdsInstanceRegistrations.Any(x => x.Description == trim);
        }
    }
}
