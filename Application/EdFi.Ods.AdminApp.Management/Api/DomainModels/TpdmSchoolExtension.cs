// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Common;

namespace EdFi.Ods.AdminApp.Management.Api.DomainModels
{
    public class TpdmSchoolExtension
    {
        public TpdmSchoolExtension(
            string accreditationStatusDescriptor,
            string federalLocaleCodeDescriptor,
            bool? improvingSchool,
            EdFiPostSecondaryInstitutionReference postSecondaryInstitutionReference = null
            )
        {
            AccreditationStatusDescriptor = accreditationStatusDescriptor.MaxLength(306, nameof(accreditationStatusDescriptor), GetType().Name);
            FederalLocaleCodeDescriptor = federalLocaleCodeDescriptor.MaxLength(306, nameof(federalLocaleCodeDescriptor), GetType().Name); ;
            ImprovingSchool = improvingSchool;
            PostSecondaryInstitutionReference = postSecondaryInstitutionReference;
        }

        public string AccreditationStatusDescriptor { get; set; }
        public string FederalLocaleCodeDescriptor { get; set; }
        public bool? ImprovingSchool { get; set; }
        public EdFiPostSecondaryInstitutionReference PostSecondaryInstitutionReference { get; set; }
    }
}
