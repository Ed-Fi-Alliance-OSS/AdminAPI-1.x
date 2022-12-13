// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using VendorApplication = EdFi.Ods.AdminApp.Management.ClaimSetEditor.Application;
using FluentValidation;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ErrorHandling;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class EditClaimSetModel: IEditClaimSetModel
    {
        public string ClaimSetName { get; set; }
        public int ClaimSetId { get; set; }
        public IEnumerable<VendorApplication> Applications { get; set; }
        public IEnumerable<ResourceClaim> ResourceClaims { get; set; }
        public List<SelectListItem> AllResourceClaims { get; set; }
    }

    public class EditClaimSetModelValidator : AbstractValidator<EditClaimSetModel>
    {
        private IGetAllClaimSetsQuery _getAllClaimSetsQuery;
        private readonly IGetClaimSetByIdQuery _getClaimSetByIdQuery;

        public EditClaimSetModelValidator(IGetAllClaimSetsQuery getAllClaimSetsQuery, IGetClaimSetByIdQuery getClaimSetByIdQuery)
        {
            _getAllClaimSetsQuery = getAllClaimSetsQuery;
            _getClaimSetByIdQuery = getClaimSetByIdQuery;

            RuleFor(m => m.ClaimSetName)
                .NotEmpty()
                .Must(BeAUniqueName)
                .WithMessage("A claim set with this name already exists in the database. Please enter a unique name.")
                .When(NameIsChanged);

            RuleFor(m => m.ClaimSetName)
                .MaximumLength(255)
                .WithMessage("The claim set name must be less than 255 characters.");
        }

        private bool NameIsChanged(EditClaimSetModel model)
        {
            try
            {
                var existingClaimSet = _getClaimSetByIdQuery.Execute(model.ClaimSetId);
                return existingClaimSet.Name != model.ClaimSetName;
            }
            catch (AdminAppException)
            {
                return false;
            }
        }

        private bool BeAUniqueName(string newName)
        {
            return _getAllClaimSetsQuery.Execute().All(x => x.Name != newName);
        }
    }
}
