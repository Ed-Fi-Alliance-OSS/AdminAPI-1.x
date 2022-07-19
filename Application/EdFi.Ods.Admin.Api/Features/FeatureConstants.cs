// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Features
{
    public class FeatureConstants
    {
        public const string Information = "information";
        public const string Vendors = "vendors";
        public const string AddVendorDisplayName = "AddVendor";
        public const string EditVendorDisplayName = "EditVendor";
        public const string VedorIdDescription = "Vendor/ company id";
        public const string VendorNameDescription = "Vendor/ company name";
        public const string VendorNamespaceDescription = "Namespace prefix for the vendor. Multiple namespace prefixes can be provided as comma separated list if required.";
        public const string VendorContactDescription = "Vendor contact name";
        public const string VendorContactEmailDescription = "Vendor contact email id";
        public const string RegisterClientId = "Client id";
        public const string RegisterClientSecret = "Client secret";
        public const string RegisterDisplayName = "Client display name";
        public const string Applications = "applications";
        public const string ClaimSets = "claimsets";
        public const string AddApplicationDisplayName = "AddApplication";
        public const string EditApplicationDisplayName = "EditApplication";
        public const string ApplicationIdDescription = "Application id";
        public const string ApplicationNameDescription = "Application name";
        public const string ClaimSetNameDescription = "Claim set name";
        public const string ProfileIdDescription = "Profile id";
        public const string EducationOrganizationIdsDescription = "Education organization ids";
        public const string ApplicationNameLengthValidationMessage = "The Application Name {ApplicationName} would be too long for Admin App to set up necessary Application records." +
                            " Consider shortening the name by {ExtraCharactersInName} character(s).";
        public const string ClaimSetNameValidationMessage = "Please provide a valid claim set name.";
        public const string EdOrgIdsValidationMessage = "Please provide at least one education organization id.";
        public const string VendorIdValidationMessage = "Please provide valid vendor id.";
    }
}
