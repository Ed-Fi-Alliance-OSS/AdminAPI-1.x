// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Models;
using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IOdsApiFacade
    {
        List<School> GetAllSchools();
        List<PsiSchool> GetAllPsiSchools();
        List<LocalEducationAgency> GetAllLocalEducationAgencies();
        List<LocalEducationAgency> GetLocalEducationAgenciesByPage(int offset = 0, int limit = 50);
        OdsApiResult DeleteLocalEducationAgency(string id);
        OdsApiResult DeleteSchool(string id);
        OdsApiResult AddLocalEducationAgency(LocalEducationAgency newLocalEducationAgency);
        OdsApiResult AddSchool(School newSchool);
        OdsApiResult AddPsiSchool(PsiSchool newSchool);
        List<SelectOptionModel> GetLocalEducationAgencyCategories();
        List<SelectOptionModel> GetAllGradeLevels();
        IReadOnlyList<string> GetAllDescriptors();
        List<Descriptor> GetDescriptorsByPath(string descriptorPath);
        bool DoesApiDataExist();
        bool DoesLearningStandardsDataExist();
        LocalEducationAgency GetLocalEducationAgencyById(string id);
        OdsApiResult EditLocalEducationAgency(LocalEducationAgency editedLocalEducationAgency);
        School GetSchoolById(string id);
        PsiSchool GetPsiSchoolById(string id);
        OdsApiResult EditSchool(School editedSchool);
        OdsApiResult EditPsiSchool(PsiSchool model);
        List<SelectOptionModel> GetAllStateAbbreviations();
        List<SelectOptionModel> GetPostSecondaryInstitutionLevels();
        List<SelectOptionModel> GetAdministrativeFundingControls();
        List<SelectOptionModel> GetAccreditationStatusOptions();
        List<SelectOptionModel> GetFederalLocaleCodes();
        List<PostSecondaryInstitution> GetAllPostSecondaryInstitutions();
        List<PostSecondaryInstitution> GetPostSecondaryInstitutionsByPage(int offset = 0, int limit = 50);
        OdsApiResult AddPostSecondaryInstitution(Models.PostSecondaryInstitution newPostSecondaryInstitution);
        PostSecondaryInstitution GetPostSecondaryInstitutionById(string id);
        OdsApiResult EditPostSecondaryInstitution(Models.PostSecondaryInstitution model);
        OdsApiResult DeletePostSecondaryInstitution(string id);
    }
}