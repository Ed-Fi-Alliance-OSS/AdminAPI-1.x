// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using AutoMapper;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class AutoMapperExtensions
    {
        private static string EducationOrganizationsKey = "EducationOrganizations";

        public static List<EducationOrganizationModel> GetEducationOrganizations(this ResolutionContext resolutionContext)
        {
            if (resolutionContext.Items.ContainsKey(EducationOrganizationsKey))
            {
                var edOrgs = resolutionContext.Items[EducationOrganizationsKey] as List<EducationOrganizationModel>;
                return edOrgs ?? new List<EducationOrganizationModel>();
            }

            return new List<EducationOrganizationModel>();
        }

        public static IMappingOperationOptions WithEducationOrganizations(this IMappingOperationOptions mappingOperationOptions, List<EducationOrganizationModel> educationOrganization)
        {
            mappingOperationOptions.Items[EducationOrganizationsKey] = educationOrganization ?? new List<EducationOrganizationModel>();
            return mappingOperationOptions;
        }
    }
}