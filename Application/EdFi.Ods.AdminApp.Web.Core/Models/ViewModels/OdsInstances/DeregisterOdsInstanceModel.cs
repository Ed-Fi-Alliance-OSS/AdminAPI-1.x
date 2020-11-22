// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Instances;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances
{
    public class DeregisterOdsInstanceModel: IDeregisterOdsInstanceModel
    {
        public int OdsInstanceId { get; set; }

        [Display(Name="ODS Instance Database")]
        public string Name { get; set; }

        [Display(Name="ODS Instance Description")]
        public string Description { get; set; }
    }

    public class DeregisterOdsInstanceModelValidator : AbstractValidator<DeregisterOdsInstanceModel>
    {
        private static AdminAppDbContext _database;

        public DeregisterOdsInstanceModelValidator(AdminAppDbContext database)
        {
            _database = database;

            RuleFor(m => m.OdsInstanceId)
                .NotEmpty()
                .Must(BeAnExistingOdsInstance).WithMessage("The instance you are trying to deregister does not exist in the database.");
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.Description).NotEmpty();
        }

        private bool BeAnExistingOdsInstance(int odsInstanceId)
        {
            return _database.OdsInstanceRegistrations.Any(x => x.Id == odsInstanceId);
        }
    }
}