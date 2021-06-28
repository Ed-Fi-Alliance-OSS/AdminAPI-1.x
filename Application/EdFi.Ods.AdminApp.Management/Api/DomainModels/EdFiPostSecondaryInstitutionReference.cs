// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Common;

namespace EdFi.Ods.AdminApp.Management.Api.DomainModels
{
    public class EdFiPostSecondaryInstitutionReference
    {
        public EdFiPostSecondaryInstitutionReference(int? postSecondaryInstitutionId)
        {
            PostSecondaryInstitutionId = postSecondaryInstitutionId.IsRequired(nameof(postSecondaryInstitutionId), GetType().Name);
        }

        public int? PostSecondaryInstitutionId { get; set; }
    }
}
