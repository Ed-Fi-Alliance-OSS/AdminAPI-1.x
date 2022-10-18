// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Api.Common
{
    public class ResourcePaths
    {
        public const string Schools = "/ed-fi/schools";
        public const string LocalEducationAgencyCategoryDescriptors = "/ed-fi/localEducationAgencyCategoryDescriptors";
        public const string LocalEducationAgencies = "/ed-fi/localEducationAgencies";
        public const string GradeLevelDescriptors = "/ed-fi/gradeLevelDescriptors";
        public const string LocalEducationAgencyById = "/ed-fi/localEducationAgencies/{id}";
        public const string SchoolById = "/ed-fi/schools/{id}";
        public const string StateAbbreviationDescriptors = "/ed-fi/stateAbbreviationDescriptors";
        public const string PostSecondaryInstitutions = "/ed-fi/postSecondaryInstitutions";
        public const string PostSecondaryInstitutionById = "/ed-fi/postSecondaryInstitutions/{id}";
        public const string PostSecondaryInstitutionLevelDescriptors = "/ed-fi/postSecondaryInstitutionLevelDescriptors";
        public const string AdministrativeFundingControlDescriptors = "/ed-fi/administrativeFundingControlDescriptors";

        public const string AccreditationStatusDescriptors = "/tpdm/accreditationStatusDescriptors";
        public const string FederalLocaleCodeDescriptors = "/tpdm/federalLocaleCodeDescriptors";
    }
}