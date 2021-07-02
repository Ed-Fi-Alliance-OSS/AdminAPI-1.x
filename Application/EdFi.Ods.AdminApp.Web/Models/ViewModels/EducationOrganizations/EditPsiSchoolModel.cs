// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Api.Models;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations
{
    public class EditPsiSchoolModel: EditSchoolModel
    {
        public int? PostSecondaryInstitutionId { get; set; }
        public string AccreditationStatus { get; set; }
        public List<SelectOptionModel> AccreditationStatusOptions { get; set; }
        public string FederalLocaleCode { get; set; }
        public List<SelectOptionModel> FederalLocaleCodeOptions { get; set; }
    }

    public class EditPsiSchoolModelValidator : EditSchoolModelValidatorBase<EditPsiSchoolModel>
    { }
}
