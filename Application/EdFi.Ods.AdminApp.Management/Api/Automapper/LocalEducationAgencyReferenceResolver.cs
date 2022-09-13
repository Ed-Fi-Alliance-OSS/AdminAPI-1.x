// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using School = EdFi.Ods.AdminApp.Management.Api.Models.School;

namespace EdFi.Ods.AdminApp.Management.Api.Automapper
{
    public class LocalEducationAgencyReferenceResolver : IValueResolver<School, EdFiSchool, EdFiLocalEducationAgencyReference>
    {
        public EdFiLocalEducationAgencyReference Resolve(School source, EdFiSchool destination, EdFiLocalEducationAgencyReference destMember,
            ResolutionContext context)
        {
            var id = source.LocalEducationAgencyId ?? destination.LocalEducationAgencyReference?.LocalEducationAgencyId;

            return id != null ? new EdFiLocalEducationAgencyReference(id) :  null;
        }

        public static EdFiLocalEducationAgencyReference Resolve(School source, ResolutionContext context)
        {
            var id = source.LocalEducationAgencyId;

            return id != null ? new EdFiLocalEducationAgencyReference(id) : null;
        }
    }
}
