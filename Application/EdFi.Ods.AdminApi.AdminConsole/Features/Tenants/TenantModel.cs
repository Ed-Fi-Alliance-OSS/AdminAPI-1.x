// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Newtonsoft.Json;
using EdFi.Ods.AdminApi.AdminConsole.Features.Steps;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;

public class ApplicationRole
{
    [JsonProperty("roleName")]
    public string RoleName { get; set; }

    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; }

    [JsonProperty("isAvailableForTenants")]
    public bool IsAvailableForTenants { get; set; }
}

public class Domain
{
    [JsonProperty("domainStatus")]
    public int DomainStatus { get; set; }

    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("domainName")]
    public string DomainName { get; set; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; }

    [JsonProperty("lastModifiedBy")]
    public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}

public class OnBoarding
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("progressPercentage")]
    public int ProgressPercentage { get; set; }

    [JsonProperty("totalSteps")]
    public int TotalSteps { get; set; }

    [JsonProperty("lastCompletedStep")]
    public int LastCompletedStep { get; set; }

    [JsonProperty("startedAt")]
    public DateTime StartedAt { get; set; }

    [JsonProperty("completedAt")]
    public DateTime CompletedAt { get; set; }

    [JsonProperty("steps")]
    public List<Step> Steps { get; set; }
}

public class Organization
{
    [JsonProperty("identifierType")]
    public string IdentifierType { get; set; }

    [JsonProperty("identifierValue")]
    public string IdentifierValue { get; set; }

    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("includeInJwt")]
    public bool IncludeInJwt { get; set; }

    [JsonProperty("shortNameOfInstitution")]
    public string ShortNameOfInstitution { get; set; }

    [JsonProperty("nameOfInstitution")]
    public string NameOfInstitution { get; set; }
}

public class Tenant
{
    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("tenantType")]
    public int TenantType { get; set; }

    [JsonProperty("tenantStatus")]
    public int TenantStatus { get; set; }

    [JsonProperty("organizationIdentifier")]
    public string OrganizationIdentifier { get; set; }

    [JsonProperty("organizationName")]
    public string OrganizationName { get; set; }

    [JsonProperty("isDemo")]
    public bool IsDemo { get; set; }

    [JsonProperty("enforceMfa")]
    public bool EnforceMfa { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("subscriptionsMigrated")]
    public bool SubscriptionsMigrated { get; set; }

    [JsonProperty("subscriptions")]
    public List<Subscription> Subscriptions { get; set; }

    [JsonProperty("domains")]
    public List<Domain> Domains { get; set; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; }

    [JsonProperty("lastModifiedBy")]
    public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }

    [JsonProperty("identityProviders")]
    public List<int> IdentityProviders { get; set; }

    [JsonProperty("onBoarding")]
    public OnBoarding OnBoarding { get; set; }

    [JsonProperty("organizations")]
    public List<Organization> Organizations { get; set; }

    [JsonProperty("settings")]
    public List<Setting> Settings { get; set; }
}

public class Setting
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("dataType")]
    public string DataType { get; set; }
}

public class Subscription
{
    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("subscriptionId")]
    public string SubscriptionId { get; set; }

    [JsonProperty("applicationId")]
    public string ApplicationId { get; set; }

    [JsonProperty("applicationName")]
    public string ApplicationName { get; set; }

    [JsonProperty("applicationRoles")]
    public List<ApplicationRole> ApplicationRoles { get; set; }

    [JsonProperty("startDateTime")]
    public string StartDateTime { get; set; }

    [JsonProperty("endDateTime")]
    public string EndDateTime { get; set; }

    [JsonProperty("actualEndDateTime")]
    public string ActualEndDateTime { get; set; }

    [JsonProperty("gracePeriod")]
    public int GracePeriod { get; set; }

    [JsonProperty("numberOfLicenses")]
    public int NumberOfLicenses { get; set; }

    [JsonProperty("assignedLicenses")]
    public int AssignedLicenses { get; set; }

    [JsonProperty("maxAssignedLicenses")]
    public int MaxAssignedLicenses { get; set; }

    [JsonProperty("lastMaxAssignedLicensesDateTime")]
    public string LastMaxAssignedLicensesDateTime { get; set; }

    [JsonProperty("licenseType")]
    public string LicenseType { get; set; }

    [JsonProperty("subscriptionStatus")]
    public string SubscriptionStatus { get; set; }

    [JsonProperty("autoAssign")]
    public bool AutoAssign { get; set; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; }

    [JsonProperty("lastModifiedBy")]
    public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}


