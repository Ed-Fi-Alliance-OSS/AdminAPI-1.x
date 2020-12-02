// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using FluentValidation;
using FluentValidation.Validators;

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
        private static IDatabaseConnectionProvider _databaseConnectionProvider;
        private static ApiMode _mode;

        public RegisterOdsInstanceModelValidator(AdminAppDbContext database
            , ICloudOdsAdminAppSettingsApiModeProvider apiModeProvider
            , IDatabaseValidationService databaseValidationService
            , IDatabaseConnectionProvider databaseConnectionProvider
            , bool validationMessageWithDetails = false)
        {
            _database = database;
            _databaseValidationService = databaseValidationService;
            _databaseConnectionProvider = databaseConnectionProvider;
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
                .Must(BeWithinApplicationNameMaxLength)
                .When(x => x.NumericSuffix != null);

            RuleFor(m => m.NumericSuffix)
                .Must(x=> x <= maxValue && x >= minValue)
                .When(x => x.NumericSuffix != null)
                .WithMessage(inRangeMessage);

            RuleFor(m => m.NumericSuffix)
                .Must(BeAUniqueInstanceName)
                .When(x => x.NumericSuffix != null)
                .WithMessage(
                    x => $"An instance for this {beUniqueValidationMsg}{(validationMessageWithDetails ? $"({x.NumericSuffix})":"")} already exists.");

            RuleFor(m => m.NumericSuffix)
                .Must(BeValidOdsInstanceDatabase)
                .When(x => x.NumericSuffix != null)
                .WithMessage(
                    x => $"Could not connect to an ODS instance database for this {beUniqueValidationMsg}{(validationMessageWithDetails ? $"({x.NumericSuffix})":"")}.");

            RuleFor(m => m.Description).NotEmpty();

            RuleFor(m => m.Description)
                .Must(BeAUniqueInstanceDescription)
                .When(x => x.Description != null)
                .WithMessage(
                    x => $"An instance with this description{(validationMessageWithDetails ? $"({beUniqueValidationMsg}: {x.NumericSuffix}, Description: {x.Description})":"")} already exists.");
        }

        private static bool BeValidOdsInstanceDatabase(int? newInstanceNumericSuffix)
        {
            return _databaseValidationService.IsValidDatabase(newInstanceNumericSuffix.Value, _mode);
        }

        private static bool BeAUniqueInstanceName(int? newInstanceNumericSuffix)
        {
            var newInstanceDatabaseName = InferInstanceDatabaseName(newInstanceNumericSuffix);
            return newInstanceDatabaseName != "" && !_database.OdsInstanceRegistrations.Any(x => x.Name == newInstanceDatabaseName);
        }

        private static bool BeWithinApplicationNameMaxLength(RegisterOdsInstanceModel model, int? newInstanceNumericSuffix, PropertyValidatorContext context)
        {
            var newInstanceDatabaseName = InferInstanceDatabaseName(newInstanceNumericSuffix);
            var extraCharactersInPrefix = newInstanceDatabaseName.GetAdminApplicationName().Length - ApplicationExtensions.MaximumApplicationNameLength;
            if (extraCharactersInPrefix <= 0)
            {
                return true;
            }

            context.Rule.MessageBuilder = c
                => $"The resulting database name {newInstanceDatabaseName} would be too long for Admin App to set up necessary Application records." +
                   $" Consider shortening the naming convention prefix in the database names and corresponding Web.config entries by {extraCharactersInPrefix} characters.";

            return  false;
        }

        private static string InferInstanceDatabaseName(int? newInstanceNumericSuffix)
        {
            using (var connection = _databaseConnectionProvider.CreateNewConnection(newInstanceNumericSuffix.Value, _mode))
                return connection.Database;
        }

        private static bool BeAUniqueInstanceDescription(string newInstanceDescription)
        {
            var trim = newInstanceDescription.Trim();

            return !_database.OdsInstanceRegistrations.Any(x => x.Description == trim);
        }       
    }
}
