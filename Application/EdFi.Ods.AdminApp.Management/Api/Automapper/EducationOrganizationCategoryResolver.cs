// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using LocalEducationAgency = EdFi.Ods.AdminApp.Management.Api.Models.LocalEducationAgency;

using School = EdFi.Ods.AdminApp.Management.Api.Models.School;

namespace EdFi.Ods.AdminApp.Management.Api.Automapper
{
    public class SchoolCategoryResolver : IValueResolver<School, EdFiSchool, List<EdFiEducationOrganizationCategory>>
    {
        public List<EdFiEducationOrganizationCategory> Resolve(School source, EdFiSchool destination,
            List<EdFiEducationOrganizationCategory> destMember, ResolutionContext context)
        {
            destMember?.Clear(); // by default, automapper combines the source list with the destination list. we want to replace the destination list instead
            return EducationOrganizationCategoryResolver.GetNewEducationCategory(source.EducationOrganizationCategory);
        }
    }

    public class LocalEducationAgencyCategoryResolver : IValueResolver<LocalEducationAgency, EdFiLocalEducationAgency, List<EdFiEducationOrganizationCategory>>
    {
        public List<EdFiEducationOrganizationCategory> Resolve(LocalEducationAgency source, EdFiLocalEducationAgency destination, List<EdFiEducationOrganizationCategory> destMember, ResolutionContext context)
        {
            destMember?.Clear(); // by default, automapper combines the source list with the destination list. we want to replace the destination list instead
            return EducationOrganizationCategoryResolver.GetNewEducationCategory(source.EducationOrganizationCategory);
        }
    }

    public static class EducationOrganizationCategoryResolver
    {
        public static List<EdFiEducationOrganizationCategory> GetNewEducationCategory(string newCategory)
        {
            var edFiEducationOrganizationCategory =
                new EdFiEducationOrganizationCategory("uri://ed-fi.org/EducationOrganizationCategoryDescriptor#" +
                                                      newCategory);
            return new List<EdFiEducationOrganizationCategory>
            {
                edFiEducationOrganizationCategory
            };
        }

        public static List<EdFiEducationOrganizationCategory> Resolve(LocalEducationAgency source)
        {
            return GetNewEducationCategory(source.EducationOrganizationCategory);
        }

        public static List<EdFiEducationOrganizationCategory> Resolve(School source)
        {
            return GetNewEducationCategory(source.EducationOrganizationCategory);
        }
    }
}
