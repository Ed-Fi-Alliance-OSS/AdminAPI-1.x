// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using School = EdFi.Ods.AdminApp.Management.Api.Models.PsiSchool;

namespace EdFi.Ods.AdminApp.Management.Api.Automapper
{
    public class SchoolExtensionsResolver : IValueResolver<School, EdFiSchool, SchoolExtensions>
    {
        public SchoolExtensions Resolve(School source, EdFiSchool destination, SchoolExtensions destMember,
            ResolutionContext context)
        {
            var psiId = source.PostSecondaryInstitutionId ?? destination._ext?.TPDM?.PostSecondaryInstitutionReference?.PostSecondaryInstitutionId;
            var postSecondaryInstitutionReference = psiId != null ? new EdFiPostSecondaryInstitutionReference(psiId) : null;
            var tpdmExtension = new TpdmSchoolExtension(source.AccreditationStatus, source.FederalLocaleCode, source.ImprovingSchool, postSecondaryInstitutionReference);
            var schoolExtensions = new SchoolExtensions(tpdmExtension);
            return schoolExtensions;
        }
    }
}
