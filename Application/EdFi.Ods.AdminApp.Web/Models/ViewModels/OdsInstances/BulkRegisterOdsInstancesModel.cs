// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using FluentValidation;
using log4net;
using FluentValidation.Validators;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances
{
    public class BulkRegisterOdsInstancesModel
    {
        private bool _streamWasRead = false;
        private IList<RegisterOdsInstanceModel> _dataRecords;
        private IList<string> _missingHeaders;

        [Accept(".csv")]
        [Display(Name = "Instances Data File")]
        public IFormFile OdsInstancesFile { get; set; }

        public IList<RegisterOdsInstanceModel> DataRecords()
        {
            EnsureReadStreamOnce();
            return _dataRecords;
        }

        public IEnumerable<IRegisterOdsInstanceModel> FilteredDataRecords { get; set; }

        public IList<string> MissingHeaders()
        {
            EnsureReadStreamOnce();
            return _missingHeaders;
        }

        private void EnsureReadStreamOnce()
        {
            if (_streamWasRead)
                return;

            _dataRecords = InputFileHelper.DataRecords(OdsInstancesFile.OpenReadStream(), out _missingHeaders);
            _streamWasRead = true;
        }
    }

    public class BulkRegisterOdsInstancesModelValidator : AbstractValidator<BulkRegisterOdsInstancesModel>
    {
        private readonly ILog _logger = LogManager.GetLogger("BulkRegisterOdsInstancesLog");
        private static AdminAppDbContext _database;
        private static IDatabaseConnectionProvider _databaseConnectionProvider;
        private static ApiMode _mode;        

        private bool UniquenessRuleFailed { get; set; }

        private bool ValidHeadersRuleFailed { get; set; }

        public BulkRegisterOdsInstancesModelValidator(AdminAppDbContext database
            , ICloudOdsAdminAppSettingsApiModeProvider apiModeProvider
            , IDatabaseValidationService databaseValidationService
            , IDatabaseConnectionProvider databaseConnectionProvider
            , IBulkRegisterOdsInstancesFiltrationService dataFilterService)
        {
            var mode = apiModeProvider.GetApiMode();

            RuleFor(m => m.OdsInstancesFile)
                .NotEmpty();

            RuleFor(m => m.OdsInstancesFile.FileName).NotNull().Must(x => x.ToLower().EndsWith(".csv"))
                .WithMessage("Please select a file with .csv format.");

            When(m => m.OdsInstancesFile != null, () =>
                {
                    RuleFor(m => m).SafeCustom(HaveValidHeaders);
                });

            When(m => m.OdsInstancesFile != null && !ValidHeadersRuleFailed, () =>
                {
                    RuleFor(m => m).SafeCustom(HaveUniqueRecords);
                });

            When(
                m => m.OdsInstancesFile != null && !UniquenessRuleFailed && !ValidHeadersRuleFailed, () =>
                {
                    RuleFor(x => x)
                        .SafeCustom(
                            (model, context) =>
                            {
                                var validator = new RegisterOdsInstanceModelValidator(
                                    database, apiModeProvider, databaseValidationService,
                                    databaseConnectionProvider, true);

                                var newOdsInstancesToRegister = dataFilterService.FilteredRecords(model.DataRecords(), mode).ToList();
                                model.FilteredDataRecords = newOdsInstancesToRegister;

                                foreach (var record in newOdsInstancesToRegister)
                                {
                                    var data = (RegisterOdsInstanceModel)record;
                                    var results = validator.Validate(data);
                                    if (!results.IsValid)
                                    {
                                        foreach (var failure in results.Errors)
                                        {
                                            _logger.Error($"Property: {failure.PropertyName} failed validation. Error: {failure.ErrorMessage}");
                                        }
                                    }
                                    context.AddFailures(results);
                                }
                            });
                });
        }

        public void GetDuplicates(List<RegisterOdsInstanceModel> dataRecords, out List<int?> duplicateNumericSuffixes, out List<string> duplicateDescriptions)
        {
            duplicateNumericSuffixes = dataRecords.GroupBy(x => x.NumericSuffix)
                .Where(g => g.Count() > 1).Select(x => x.Key).ToList();
            duplicateDescriptions = dataRecords.GroupBy(x => x.Description)
                .Where(g => g.Count() > 1).Select(x => x.Key).ToList();
        }

        private void HaveValidHeaders(BulkRegisterOdsInstancesModel model, CustomContext context)
        {
            var missingHeaders = model.MissingHeaders();

            if (missingHeaders == null || !missingHeaders.Any())
            {
                return;
            }

            ValidHeadersRuleFailed = true;
            context.AddFailure($"Missing Headers: {string.Join(",", model.MissingHeaders())}");
        }

        private void HaveUniqueRecords(BulkRegisterOdsInstancesModel model, CustomContext context)
        {
            GetDuplicates(model.DataRecords().ToList(), out var duplicateNumericSuffixes, out var duplicateDescriptions);

            var errorMessage = "";

            if (duplicateNumericSuffixes.Any())
            {
                errorMessage += $"The following instance numeric suffixes have duplicates : {string.Join(", ", duplicateNumericSuffixes)} \n";
            }

            if (duplicateDescriptions.Any())
            {
                errorMessage += $"The following instance descriptions have duplicates : {string.Join(", ", duplicateDescriptions)}";
            }

            if (errorMessage == "")
            {
                return;
            }

            UniquenessRuleFailed = true;
            context.AddFailure(errorMessage);
        }
    }
}
