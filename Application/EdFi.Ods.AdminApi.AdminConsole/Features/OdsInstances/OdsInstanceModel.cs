// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Features.Steps;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.OdsInstances;

public class Instance
{
    [JsonProperty("instanceId")]
    public string InstanceId { get; set; }

    [JsonProperty("tenantId")]
    public string TenantId { get; set; }

    [JsonProperty("instanceName")]
    public string InstanceName { get; set; }

    [JsonProperty("instanceType")]
    public string InstanceType { get; set; }

    [JsonProperty("connectionType")]
    public string ConnectionType { get; set; }

    [JsonProperty("clientId")]
    public string ClientId { get; set; }

    [JsonProperty("clientSecret")]
    public string ClientSecret { get; set; }

    [JsonProperty("baseUrl")]
    public string BaseUrl { get; set; }

    [JsonProperty("authenticationUrl")]
    public string AuthenticationUrl { get; set; }

    [JsonProperty("resourcesUrl")]
    public string ResourcesUrl { get; set; }

    [JsonProperty("schoolYears")]
    public List<int> SchoolYears { get; set; }

    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; }

    [JsonProperty("verificationStatus")]
    public VerificationStatus VerificationStatus { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }
}

public class OdsInstance
{
    [JsonProperty("data")]
    public List<Instance> Data { get; set; }
}

public class VerificationStatus
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


