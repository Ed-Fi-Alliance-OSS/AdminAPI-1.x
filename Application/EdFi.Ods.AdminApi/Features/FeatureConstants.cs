// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Security.Policy;

namespace EdFi.Ods.AdminApi.Features;

public class FeatureConstants
{
    public const string VendorIdDescription = "Vendor/ company id";
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
    public const string ClaimSetNotFound = "No such claim set exists in the database.";
    public const string InvalidResourceClaimActions = "Please provide a valid resourceClaimActions object.";
    public const string ResourceClaimOneActionNotSet = "A resource must have at least one action associated with it to be added.";
    public const string OdsInstanceIdDescription = "ODS instance id";
    public const string OdsInstanceIdValidationMessage = "Please provide valid ods instance id.";
    public const string ClaimSetIdToCopy = "ClaimSet id to copy";
    public const string ProfileName = "Profile name";
    public const string ProfileDefinition = "Profile definition";
}
