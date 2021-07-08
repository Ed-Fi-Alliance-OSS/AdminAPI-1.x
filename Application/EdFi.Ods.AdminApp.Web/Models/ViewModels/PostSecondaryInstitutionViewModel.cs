// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Models;
using System.Collections.Generic;
using EdFi.Ods.AdminApp.Web.Display.Pagination;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels
{
    public class PostSecondaryInstitutionViewModel
    {
        public List<PsiSchool> Schools { get; set; }
        public PagedList<PostSecondaryInstitution> PostSecondaryInstitutions { get; set; }
        public AddPsiSchoolModel AddPsiSchoolModel { get; set; }
        public AddPostSecondaryInstitutionModel AddPostSecondaryInstitutionModel { get; set; }
    }
}
