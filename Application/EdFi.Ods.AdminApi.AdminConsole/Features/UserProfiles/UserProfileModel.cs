// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;
public class Extension
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("data")]
    public string Data { get; set; }
}

public class Preference
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}

public class UserProfile
{
    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("preferences")]
    public List<Preference> Preferences { get; set; }

    [JsonProperty("extensions")]
    public List<Extension> Extensions { get; set; }

    [JsonProperty("tenants")]
    public List<UserProfileTenant> Tenants { get; set; }

    [JsonProperty("selectedTenant")]
    public SelectedTenant SelectedTenant { get; set; }

    [JsonProperty("tenantsTotalCount")]
    public int? TenantsTotalCount { get; set; }
}

public class SelectedTenant
{
    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime? CreatedDateTime { get; set; }

    [JsonProperty("domains")]
    public List<string> Domains { get; set; }

    [JsonProperty("isDemo")]
    public bool? IsDemo { get; set; }

    [JsonProperty("isIdentityProviders")]
    public List<string> IsIdentityProviders { get; set; }

    [JsonProperty("lastModifiedBy")]
    public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDateTime")]
    public DateTime? LastModifiedDateTime { get; set; }

    [JsonProperty("organizationIdentifier")]
    public string OrganizationIdentifier { get; set; }

    [JsonProperty("organizationName")]
    public string OrganizationName { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("subscriptions")]
    public List<object> Subscriptions { get; set; }

    [JsonProperty("subscriptionsMigrated")]
    public bool? SubscriptionsMigrated { get; set; }

    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("tenantStatus")]
    public string TenantStatus { get; set; }

    [JsonProperty("tenantType")]
    public string TenantType { get; set; }
}

public class UserProfileTenant
{
    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime? CreatedDateTime { get; set; }

    [JsonProperty("domains")]
    public List<string> Domains { get; set; }

    [JsonProperty("isDemo")]
    public bool? IsDemo { get; set; }

    [JsonProperty("isIdentityProviders")]
    public List<string> IsIdentityProviders { get; set; }

    [JsonProperty("lastModifiedBy")]
    public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDateTime")]
    public DateTime? LastModifiedDateTime { get; set; }

    [JsonProperty("organizationIdentifier")]
    public string OrganizationIdentifier { get; set; }

    [JsonProperty("organizationName")]
    public string OrganizationName { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("subscriptions")]
    public List<object> Subscriptions { get; set; }

    [JsonProperty("subscriptionsMigrated")]
    public bool? SubscriptionsMigrated { get; set; }

    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("tenantStatus")]
    public string TenantStatus { get; set; }

    [JsonProperty("tenantType")]
    public string TenantType { get; set; }
}

