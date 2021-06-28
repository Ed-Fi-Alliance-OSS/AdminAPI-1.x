// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Api.Common;

namespace EdFi.Ods.AdminApp.Management.Api.DomainModels
{
    public class EdFiSchool
    {
        public EdFiSchool(
            string id,
            string nameOfInstitution,
            int? schoolId,
            List<EdFiEducationOrganizationAddress> addresses,
            List<EdFiEducationOrganizationCategory> educationOrganizationCategories,
            List<EdFiSchoolGradeLevel> gradeLevels,
            EdFiLocalEducationAgencyReference localEducationAgencyReference = null,
            SchoolExtensions ext = null
        )
        {
            var resourceName = GetType().Name;
            Id = id.IsRequired(nameof(id), resourceName);
            Addresses = addresses.IsRequired(nameof(addresses), resourceName);
            EducationOrganizationCategories =
                educationOrganizationCategories.IsRequired(nameof(educationOrganizationCategories), resourceName);
            GradeLevels = gradeLevels.IsRequired(nameof(gradeLevels), resourceName);
            SchoolId = schoolId.IsRequired(nameof(schoolId), resourceName);
            NameOfInstitution = nameOfInstitution.IsRequired(nameof(nameOfInstitution), resourceName);
            LocalEducationAgencyReference = localEducationAgencyReference;
            Ext = ext;
        }

        public string Id { get; }
        public string NameOfInstitution { get; }
        public int? SchoolId { get; }
        public EdFiLocalEducationAgencyReference LocalEducationAgencyReference { get; set; }
        public List<EdFiEducationOrganizationAddress> Addresses { get; }
        public List<EdFiSchoolGradeLevel> GradeLevels { get; }
        public List<EdFiEducationOrganizationCategory> EducationOrganizationCategories { get; }
        public SchoolExtensions Ext { get; set; }
    }
}