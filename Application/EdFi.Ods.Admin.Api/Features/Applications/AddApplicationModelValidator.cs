// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Commands;
using FluentValidation;

namespace EdFi.Ods.Admin.Api.Features.Applications
{
    public class AddApplicationModelValidator : AbstractValidator<IAddApplicationModel>
    {
        public AddApplicationModelValidator()
        {
            RuleFor(m => m.ApplicationName)
                .NotEmpty();

            RuleFor(m => m.ApplicationName)
                .Must(BeWithinApplicationNameMaxLength)
                .WithMessage("The Application Name {ApplicationName} would be too long for Admin App to set up necessary Application records." +
                " Consider shortening the name by {ExtraCharactersInName} character(s).")
                .When(x => x.ApplicationName != null);

            RuleFor(m => m.ClaimSetName)
                .NotEmpty()
                .WithMessage("You must choose a Claim Set");

            RuleFor(m => m.EducationOrganizationIds)
                .NotEmpty()
                .WithMessage("You must choose at least one Education Organization");

            RuleFor(m => m.VendorId).NotEmpty();
        }

        private bool BeWithinApplicationNameMaxLength<T>(IAddApplicationModel model, string applicationName, ValidationContext<T> context)
        {
            var extraCharactersInName = applicationName.Length - ValidationConstants.MaximumApplicationNameLength;
            if (extraCharactersInName <= 0)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("ApplicationName", applicationName);
            context.MessageFormatter.AppendArgument("ExtraCharactersInName", extraCharactersInName);

            return false;
        }
    }
}
