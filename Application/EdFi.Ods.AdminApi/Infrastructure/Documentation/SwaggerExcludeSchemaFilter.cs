// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EdFi.Ods.AdminApi.Infrastructure.Documentation;

[AttributeUsage(AttributeTargets.Property)]
public class SwaggerExcludeAttribute : Attribute
{
    public EdFiOdsSecurityModelCompatibility VersionCompatibility { get; set; }
    public SwaggerExcludeAttribute(EdFiOdsSecurityModelCompatibility version = EdFiOdsSecurityModelCompatibility.Both)
    {
        VersionCompatibility = version;
    }
}

public class SwaggerExcludeSchemaFilter : ISchemaFilter
{
    private readonly IOdsSecurityModelVersionResolver _odsSecurityModelResolver;

    public SwaggerExcludeSchemaFilter(IOdsSecurityModelVersionResolver odsSecurityModelResolver)
    {
        _odsSecurityModelResolver = odsSecurityModelResolver;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var properties = context.Type.GetProperties();
        var version = _odsSecurityModelResolver.DetermineSecurityModel();
        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute(typeof(SwaggerExcludeAttribute));
            var propertyNameInCamelCasing = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];

            if (attribute != null)
            {
                var swaggerExcludeAttribute = property.CustomAttributes.First(a => a.AttributeType.Name == nameof(SwaggerExcludeAttribute));
                var odsVersionValue = swaggerExcludeAttribute.ConstructorArguments.First().Value;
                if (odsVersionValue != null &&
                    ((EdFiOdsSecurityModelCompatibility)odsVersionValue == EdFiOdsSecurityModelCompatibility.Both ||
                    (EdFiOdsSecurityModelCompatibility)odsVersionValue == version))
                    schema.Properties.Remove(propertyNameInCamelCasing);
            }
        }
    }
}
