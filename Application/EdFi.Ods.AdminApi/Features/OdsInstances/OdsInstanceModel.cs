// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.ODSInstances;

[SwaggerSchema(Title = "ODS Instance")]
public class OdsInstanceModel
{
    public int OdsInstanceId { get; set; }
    public string Name { get; set; }
    public string InstanceType { get; set; }
    public string Status { get; set; }
    public bool IsExtended { get; set; }
    public string Version { get; set; }
}
