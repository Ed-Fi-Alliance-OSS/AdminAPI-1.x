// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.Common.Helpers;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;

public class UserProfile : IModel
{
    public int? DocId { get; set; }
    public required int InstanceId { get; set; }
    public required int TenantId { get; set; }
    public int? EdOrgId { get; set; }
    [JsonConverter(typeof(StringToJsonDocumentConverter))]
    public required string Document { get; set; }
}
