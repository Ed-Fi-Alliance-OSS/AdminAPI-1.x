// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using EdFi.Ods.AdminApp.Management.Api.Models;

namespace EdFi.Ods.AdminApp.Management.Api.Automapper
{
    public class StateEducationAgencyReferenceResolver : IValueResolver<LocalEducationAgency, EdFiLocalEducationAgency, EdFiStateEducationAgencyReference>
    {
        public EdFiStateEducationAgencyReference Resolve(LocalEducationAgency source,
            EdFiLocalEducationAgency destination, EdFiStateEducationAgencyReference destMember,
            ResolutionContext context)
        {
            var id = source.StateOrganizationId ?? destination.StateEducationAgencyReference?.StateEducationAgencyId;

            return id != null ? new EdFiStateEducationAgencyReference(id) : null;
        }
    }
}
