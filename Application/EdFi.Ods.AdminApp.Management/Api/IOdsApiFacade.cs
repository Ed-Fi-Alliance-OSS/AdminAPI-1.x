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
        List<LocalEducationAgency> GetAllLocalEducationAgencies();
        List<LocalEducationAgency> GetLocalEducationAgenciesByPage(int offset = 0, int limit = 50);
        OdsApiResult DeleteLocalEducationAgency(string id);
        OdsApiResult DeleteSchool(string id);
        OdsApiResult AddLocalEducationAgency(LocalEducationAgency newLocalEducationAgency);
        OdsApiResult AddSchool(School newSchool);
        List<SelectOptionModel> GetLocalEducationAgencyCategories();
        List<SelectOptionModel> GetAllGradeLevels();
        IReadOnlyList<string> GetAllDescriptors();
        List<Descriptor> GetDescriptorsByPath(string descriptorPath);
        bool DoesApiDataExist();
        bool DoesLearningStandardsDataExist();
        LocalEducationAgency GetLocalEducationAgencyById(string id);
        OdsApiResult EditLocalEducationAgency(LocalEducationAgency editedLocalEducationAgency);
        School GetSchoolById(string id);
        OdsApiResult EditSchool(School editedSchool);
        List<SelectOptionModel> GetAllStateAbbreviations();
    }
}