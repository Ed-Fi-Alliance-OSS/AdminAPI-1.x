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
    public class EducationOrganizationViewModel
    {
        public List<School> Schools { get; set; }
        public PagedList<LocalEducationAgency> LocalEducationAgencies { get; set; }
        public bool ShouldAllowMultipleDistricts { get; set; }
        public AddSchoolModel AddSchoolModel { get; set; }
        public AddLocalEducationAgencyModel AddLocalEducationAgencyModel { get; set; }
    }
}
