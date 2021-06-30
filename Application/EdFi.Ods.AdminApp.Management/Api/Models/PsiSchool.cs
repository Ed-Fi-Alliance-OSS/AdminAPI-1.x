// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Api.Models
{
    public class PsiSchool : School
    {
        public int? PostSecondaryInstitutionId { get; set; }
        public string AccreditationStatus { get; set; }
        public string FederalLocaleCode { get; set; }
        public bool? ImprovingSchool { get; set; }
    }
}
