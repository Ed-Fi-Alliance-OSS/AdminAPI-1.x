// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Application
{
    public class EditApplicationModel : IEditApplicationModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int VendorId { get; set; }
        public string ClaimSetName { get; set; }

        [Display(Name = "API Profile Id")]
        public int? ProfileId { get; set; }

        [Display(Name = "Education Organizations")]
        public IEnumerable<int> EducationOrganizationIds { get; set; }
    }

    public class EditApplicationViewModel : EditApplicationModel
    {
        public string VendorName { get; set; }
        public List<string> ClaimSetNames { get; set; }
        public List<ProfileModel> Profiles { get; set; }
        public List<LocalEducationAgency> LocalEducationAgencies { get; set; }
        public List<PostSecondaryInstitution> PostSecondaryInstitutions { get; set; }
        public List<School> Schools { get; set; }
        public ApplicationEducationOrganizationType EducationOrganizationType { get; set; }
        public bool TpdmEnabled { get; set; }
    }

    public class EditApplicationModelValidator : AbstractValidator<EditApplicationModel>
    {
        public EditApplicationModelValidator()
        {
            RuleFor(m => m.ApplicationId).NotEmpty();
            RuleFor(m => m.ApplicationName).NotEmpty();
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

        private bool BeWithinApplicationNameMaxLength<T>(EditApplicationModel model, string applicationName, ValidationContext<T> context)
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
