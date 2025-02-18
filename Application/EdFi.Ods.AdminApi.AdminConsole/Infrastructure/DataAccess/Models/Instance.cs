// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EdFi.Ods.AdminApi.Common.Helpers;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;

public class Instance
{
    public int? DocId { get; set; }
    public required int OdsInstanceId { get; set; }
    public required int TenantId { get; set; }
    public int? EdOrgId { get; set; }
    [JsonConverter(typeof(StringToJsonDocumentConverter))]
    public required string Document { get; set; }
    [JsonConverter(typeof(StringToJsonDocumentConverter))]
    public required string ApiCredentials { get; set; }
}
