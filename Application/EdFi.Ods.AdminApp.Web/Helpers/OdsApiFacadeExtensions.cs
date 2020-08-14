// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class OdsApiFacadeExtensions
    {
        public static List<EducationOrganizationModel> GetAllEducationOrganizations(this IOdsApiFacade odsApiFacade, IMapper mapper)
        {
            return odsApiFacade.GetAllLocalEducationAgencies().Select(mapper.Map<EducationOrganizationModel>)
                    .Union(odsApiFacade.GetAllSchools().Select(mapper.Map<EducationOrganizationModel>))
                    .ToList();
        }

        public static void WarmUp(this IOdsApiFacade odsApiFacade)
        {
            odsApiFacade.GetAllLocalEducationAgencies();
            odsApiFacade.GetAllGradeLevels();
        }
    }
}