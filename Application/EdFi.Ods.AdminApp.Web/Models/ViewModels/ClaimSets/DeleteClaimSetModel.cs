// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class DeleteClaimSetModel: IDeleteClaimSetModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsEditable { get; set; }
        public int VendorApplicationCount { get; set; }
    }

    public class DeleteClaimSetModelValidator : AbstractValidator<DeleteClaimSetModel>
    {
        private readonly IGetClaimSetByIdQuery _getClaimSetByIdQuery;

        public DeleteClaimSetModelValidator(IGetClaimSetByIdQuery getClaimSetByIdQuery)
        {
            _getClaimSetByIdQuery = getClaimSetByIdQuery;

            RuleFor(m => m.Id).NotEmpty()
                .Must(BeAnExistingClaimSet)
                .WithMessage("No such claim set exists in the database");
            RuleFor(m => m.VendorApplicationCount)
                .Equal(0)
                .WithMessage(m => $"Cannot delete this claim set. This claim set has {m.VendorApplicationCount} associated application(s).");
            RuleFor(m => m.IsEditable).NotEqual(false).WithMessage("Only user created claim sets can be deleted");
        }

        private bool BeAnExistingClaimSet(int id)
        {
            try
            {
                _getClaimSetByIdQuery.Execute(id);
                return true;
            }
            catch (AdminAppException)
            {
                return false;
            }
        }
    }
}