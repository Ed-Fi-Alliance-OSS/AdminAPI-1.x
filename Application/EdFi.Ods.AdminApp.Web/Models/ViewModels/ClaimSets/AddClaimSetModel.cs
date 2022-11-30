// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class AddClaimSetModel: IAddClaimSetModel
    {
        public string ClaimSetName { get; set; }
    }

    public class AddClaimSetModelValidator : AbstractValidator<AddClaimSetModel>
    {
        private GetAllClaimSetsQuery _getAllClaimSetsQuery;

        public AddClaimSetModelValidator(GetAllClaimSetsQuery getAllClaimSetsQuery)
        {
            _getAllClaimSetsQuery = getAllClaimSetsQuery;

            RuleFor(m => m.ClaimSetName).NotEmpty()
                .Must(BeAUniqueName)
                .WithMessage("A claim set with this name already exists in the database. Please enter a unique name.");

            RuleFor(m => m.ClaimSetName)
                .MaximumLength(255)
                .WithMessage("The claim set name must be less than 255 characters.");
        }

        private bool BeAUniqueName(string newName)
        {
            return _getAllClaimSetsQuery.Execute().All(x => x.Name != newName);
        }
    }
}
