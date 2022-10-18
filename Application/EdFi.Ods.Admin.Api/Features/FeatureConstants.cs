// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Features
{
    public class FeatureConstants
    {
        public const string VedorIdDescription = "Vendor/ company id";
        public const string VendorNameDescription = "Vendor/ company name";
        public const string VendorNamespaceDescription = "Namespace prefix for the vendor. Multiple namespace prefixes can be provided as comma separated list if required.";
        public const string VendorContactDescription = "Vendor contact name";
        public const string VendorContactEmailDescription = "Vendor contact email id";
        public const string ApplicationNameDescription = "Application name";
        public const string ClaimSetNameDescription = "Claim set name";
        public const string ProfileIdDescription = "Profile id";
        public const string EducationOrganizationIdsDescription = "Education organization ids";
        public const string ResourceClaimsDescription = "Resource Claims";
        public const string ApplicationNameLengthValidationMessage = "The Application Name {ApplicationName} would be too long for Admin App to set up necessary Application records." +
                            " Consider shortening the name by {ExtraCharactersInName} character(s).";
        public const string ClaimSetNameValidationMessage = "Please provide a valid claim set name.";
        public const string EdOrgIdsValidationMessage = "Please provide at least one education organization id.";
        public const string VendorIdValidationMessage = "Please provide valid vendor id.";
        public const string DeletedSuccessResponseDescription = "Resource was successfully deleted.";
        public const string InternalServerErrorResponseDescription = "Internal server error. An unhandled error occurred on the server. See the response body for details.";
        public const string BadRequestResponseDescription = "Bad Request. The request was invalid and cannot be completed. See the response body for details.";
        public const string ClaimSetAlreadyExistsMessage = "A claim set with this name already exists in the database. Please enter a unique name.";
        public const string ClaimSetNameMaxLengthMessage = "The claim set name must be less than 255 characters.";
        public const string ClaimSetResourceNotFoundMessage = "This Claim Set contains a resource which is not in the system. Claimset Name: '{ClaimSetName}' Resource name: '{ResourceClaimName}'.\n";
        public const string ClaimSetAuthStrategyNotFoundMessage = "This resource claim contains an authorization strategy which is not in the system. Claimset Name: '{ClaimSetName}' Resource name: '{ResourceClaimName}' Authorization strategy: '{AuthStrategyName}'.\n";
        public const string ClaimSetNotFound = "No such claim set exists in the database.";
        public const string ClaimSetResourceClaimWithNoActionMessage = "Only valid resources can be added. A resource must have at least one action associated with it to be added. The following is an invalid resource: '{ResourceClaimName}'";
        public const string ClaimSetDuplicateResourceMessage = "Only unique resource claims can be added. The following is a duplicate resource: '{ResourceClaimName}'";
        public const string WrongChildResourceMessage = "'{ChildResource}' can not be added as a child resource.";
        public const string ChildToWrongParentResourceMessage = "Child resource: '{ChildResource}' added to the wrong parent resource. Correct parent resource is: '{CorrectParentResource}'";
    }
}
