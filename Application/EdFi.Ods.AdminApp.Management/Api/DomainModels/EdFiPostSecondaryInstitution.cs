// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Api.Common;

namespace EdFi.Ods.AdminApp.Management.Api.DomainModels
{
    public class EdFiPostSecondaryInstitution
    {
        public EdFiPostSecondaryInstitution(
            string id,
            List<EdFiEducationOrganizationAddress> addresses,
            List<EdFiEducationOrganizationCategory> categories,
            int? postSecondaryInstitutionId,
            string postSecondaryInstitutionLevelDescriptor,
            string administrativeFundingControlDescriptor,
            string nameOfInstitution
            )
        {
            var resourceName = GetType().Name;
            Id = id.IsRequired(nameof(Id), resourceName);
            Addresses = addresses.IsRequired(nameof(Addresses), resourceName);
            Categories = categories.IsRequired(nameof(Categories), resourceName);
            PostSecondaryInstitutionId = postSecondaryInstitutionId.IsRequired(nameof(postSecondaryInstitutionId), resourceName);
            PostSecondaryInstitutionLevelDescriptor = postSecondaryInstitutionLevelDescriptor.IsRequired(nameof(PostSecondaryInstitutionLevelDescriptor), resourceName);
            AdministrativeFundingControlDescriptor = administrativeFundingControlDescriptor.IsRequired(nameof(AdministrativeFundingControlDescriptor), resourceName);
            NameOfInstitution = nameOfInstitution.IsRequired(nameof(NameOfInstitution), resourceName);
        }

        public string Id { get; }
        public int? PostSecondaryInstitutionId { get; }
        public string NameOfInstitution { get; }
        public List<EdFiEducationOrganizationAddress> Addresses { get; }
        public string PostSecondaryInstitutionLevelDescriptor { get; }
        public string AdministrativeFundingControlDescriptor { get; }
        public List<EdFiEducationOrganizationCategory> Categories { get; }
    }
}
