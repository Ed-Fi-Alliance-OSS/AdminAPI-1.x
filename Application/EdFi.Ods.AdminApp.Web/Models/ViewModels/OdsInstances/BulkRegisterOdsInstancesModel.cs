// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Web;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.OdsInstanceServices;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using FluentValidation;
using log4net;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances
{
    public class BulkRegisterOdsInstancesModel : IBulkRegisterOdsInstancesModel
    {
        [Accept(".csv")]
        [Display(Name = "Instances Data File")]
        public HttpPostedFileBase OdsInstancesFile { get; set; }
      
    }

    public class BulkRegisterOdsInstancesModelValidator : AbstractValidator<BulkRegisterOdsInstancesModel>
    {
        private readonly ILog _logger = LogManager.GetLogger("BulkRegisterOdsInstancesLog");

        public BulkRegisterOdsInstancesModelValidator(AdminAppDbContext database
            , ICloudOdsAdminAppSettingsApiModeProvider apiModeProvider
            , IDatabaseValidationService databaseValidationService
            , IDatabaseConnectionProvider databaseConnectionProvider)
        {
            RuleFor(m => m.OdsInstancesFile).NotEmpty();

            When(
                m => m.OdsInstancesFile != null, () =>
                {
                    RuleFor(x => x.OdsInstancesFile)
                        .SafeCustom(
                            (model, context) =>
                            {
                                var validator = new RegisterOdsInstanceModelValidator(
                                    database, apiModeProvider, databaseValidationService,
                                    databaseConnectionProvider, true);

                                foreach (var record in model.DataRecords())
                                {
                                    var results = validator.Validate(record);
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
    }
}
