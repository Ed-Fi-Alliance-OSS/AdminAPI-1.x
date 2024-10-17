// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Permissions;

public class Permission
{
    [JsonProperty("data")]
    public List<object> Data { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}
