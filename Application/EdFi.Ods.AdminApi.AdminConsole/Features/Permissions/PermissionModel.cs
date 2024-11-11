// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Permissions;

public class PermissionModel
{
    [JsonProperty("DocId")]
    public int DocId { get; set; }

    [JsonProperty("PermissionId")]
    public int? PermissionId { get; set; }

    [JsonProperty("TenantId")]
    public int? TenantId { get; set; }

    [JsonProperty("EdOrgId")]
    public int? EdOrgId { get; set; }

    [JsonProperty("Document")]
    public string? Document { get; set; }
}
