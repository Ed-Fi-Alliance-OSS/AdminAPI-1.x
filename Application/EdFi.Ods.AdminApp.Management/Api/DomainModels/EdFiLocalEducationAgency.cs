// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Api.Common;

namespace EdFi.Ods.AdminApp.Management.Api.DomainModels
{
    public class EdFiLocalEducationAgency
    {
        public EdFiLocalEducationAgency(
            string id,
            List<EdFiEducationOrganizationAddress> addresses,
            List<EdFiEducationOrganizationCategory> categories,
            int? localEducationAgencyId,
            string localEducationAgencyCategoryDescriptor,
            string nameOfInstitution,
            EdFiStateEducationAgencyReference stateEducationAgencyReference = null
        )
        {
            var resourceName = GetType().Name;
            Id = id.IsRequired(nameof(Id), resourceName);
            Addresses = addresses.IsRequired(nameof(Addresses), resourceName);
            Categories = categories.IsRequired(nameof(Categories), resourceName);
            LocalEducationAgencyId = localEducationAgencyId.IsRequired(nameof(localEducationAgencyId), resourceName);
            LocalEducationAgencyCategoryDescriptor = localEducationAgencyCategoryDescriptor.IsRequired(nameof(LocalEducationAgencyCategoryDescriptor), resourceName);
            NameOfInstitution = nameOfInstitution.IsRequired(nameof(NameOfInstitution), resourceName);
            StateEducationAgencyReference = stateEducationAgencyReference;
        }

        public string Id { get; }
        public int? LocalEducationAgencyId { get; }
        public string NameOfInstitution { get; }
        public List<EdFiEducationOrganizationAddress> Addresses { get; }
        public string LocalEducationAgencyCategoryDescriptor { get; }
        public EdFiStateEducationAgencyReference StateEducationAgencyReference { get; set; }
        public List<EdFiEducationOrganizationCategory> Categories { get; }
    }
}