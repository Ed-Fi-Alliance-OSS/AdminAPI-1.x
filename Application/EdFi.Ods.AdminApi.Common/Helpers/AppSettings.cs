// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Common.Helpers;
public class AppSettings
{
    public int DefaultPageSizeOffset { get; set; }
    public int DefaultPageSizeLimit { get; set; }
    public string? DatabaseEngine { get; set; }
    public bool MultiTenancy { get; set; }
    public bool PreventDuplicateApplications { get; set; }
}

public class SwaggerSettings
{
    public bool EnableSwagger { get; set; }
    public string? DefaultTenant { get; set; }
}
