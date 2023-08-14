// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Common.Extensions;
using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EdFi.Ods.AdminApi.Infrastructure.Documentation;

public class SwaggerDefaultParameterFilter : IOperationFilter
{
    private readonly IOptions<AppSettings> _settings;

    public SwaggerDefaultParameterFilter(IOptions<AppSettings> settings)
    {
        _settings = settings;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters)
        {
            if (parameter.Name.ToLower().Equals("offset"))
            {
                parameter.Description = "Indicates how many items should be skipped before returning results.";
                parameter.Schema.Default = new OpenApiString(_settings.Value.DefaultPageSizeOffset.ToString());
            }
            if (parameter.Name.ToLower().Equals("limit"))
            {
                parameter.Description = "Indicates the maximum number of items that should be returned in the results.";
                parameter.Schema.Default = new OpenApiString(_settings.Value.DefaultPageSizeLimit.ToString());
            }
        }
    }
}
