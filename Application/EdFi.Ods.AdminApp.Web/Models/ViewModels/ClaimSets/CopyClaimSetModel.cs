// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class CopyClaimSetModel: ICopyClaimSetModel
    {
        public string Name { get; set; }
        public int OriginalId { get; set; }
        public string OriginalName { get; set; }
    }

    public class CopyClaimSetModelValidator : AbstractValidator<CopyClaimSetModel>
    {
        private readonly ISecurityContext _context;

        public CopyClaimSetModelValidator(ISecurityContext context)
        {
            _context = context;
            RuleFor(m => m.Name).NotEmpty().Must(BeAUniqueName).WithMessage("The new claim set must have a unique name");

            RuleFor(m => m.Name)
                .MaximumLength(255)
                .WithMessage("The claim set name must be maximum 255 characters.");
        }

        private bool BeAUniqueName(string newName)
        {
            return !_context.ClaimSets.Any(x => x.ClaimSetName == newName);
        }
    }
}
