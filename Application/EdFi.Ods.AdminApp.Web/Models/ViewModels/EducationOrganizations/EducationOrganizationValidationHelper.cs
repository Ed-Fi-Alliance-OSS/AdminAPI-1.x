// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations
{
    public class EducationOrganizationValidationHelper
    {
        public static bool ProposedEducationOrganizationIdIsInUse(int id, IOdsApiFacade apiFacade)
        {
            return apiFacade.GetAllPostSecondaryInstitutions().Find(x => x.EducationOrganizationId == id) != null ||
                   apiFacade.GetAllLocalEducationAgencies().Find(x => x.EducationOrganizationId == id) != null ||
                   apiFacade.GetAllSchools().Find(x => x.EducationOrganizationId == id) != null;
        }
    }
}
