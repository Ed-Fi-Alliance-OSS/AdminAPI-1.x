// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Api.Models;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations
{
    public class AddPostSecondaryInstitutionModel
    {
        [Display(Name = "Post-Secondary Institution ID")]
        public int? PostSecondaryInstitutionId { get; set; }
        [Display(Name = "Name of Institution")]
        public string Name { get; set; }
        [Display(Name = "Address")]
        public string StreetNumberName { get; set; }
        [Display(Name = "Suite")]
        public string ApartmentRoomSuiteNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        [Display(Name = "Post-Secondary Institution Level")]
        public string PostSecondaryInstitutionLevel { get; set; }
        public string AdministrativeFundingControl { get; set; }
        public List<SelectOptionModel> PostSecondaryInstitutionLevelOptions { get; set; }
        public List<SelectOptionModel> AdministrativeFundingControlOptions { get; set; }
        public List<SelectOptionModel> StateOptions { get; set; }
        public bool RequiredApiDataExist { get; set; }
    }

    public class AddPostSecondaryInstitutionModelValidator : AbstractValidator<AddPostSecondaryInstitutionModel>
    {
        private readonly IOdsApiFacade _apiFacade;

        public AddPostSecondaryInstitutionModelValidator(IOdsApiFacadeFactory odsApiFacadeFactory)
        {
            _apiFacade = odsApiFacadeFactory.Create().GetAwaiter().GetResult();
            RuleFor(m => m.PostSecondaryInstitutionId).NotEmpty();
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.StreetNumberName).NotEmpty();
            RuleFor(m => m.State).NotEmpty();
            RuleFor(m => m.City).NotEmpty();
            RuleFor(m => m.ZipCode).NotEmpty();
            RuleFor(m => m.PostSecondaryInstitutionId)
                .Must(i => BeUniqueId(i, _apiFacade)).When(m => m.PostSecondaryInstitutionId != null)
                .WithMessage("This 'Post-Secondary Institution ID' is already associated with another Education Organization. Please provide a unique value.");
        }

        public static bool BeUniqueId(int? id, IOdsApiFacade apiFacade)
        {
            return id != null
                   && apiFacade.GetAllPostSecondaryInstitutions().Find(x => x.EducationOrganizationId == id) == null
                   && apiFacade.GetAllLocalEducationAgencies().Find(x => x.EducationOrganizationId == id) == null
                   && apiFacade.GetAllSchools().Find(x => x.EducationOrganizationId == id) == null;
        }
    }
}
