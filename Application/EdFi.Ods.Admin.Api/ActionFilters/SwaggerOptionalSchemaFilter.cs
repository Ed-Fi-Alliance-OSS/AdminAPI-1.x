// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EdFi.Ods.Admin.Api.ActionFilters
{
    public class SwaggerOptionalSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var properties = context.Type.GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute(typeof(SwaggerOptionalAttribute));

                if (attribute == null)
                {
                    var propertyNameInCamelCasing = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);

                    if (schema.Required == null)
                    {
                        schema.Required = new HashSet<string>() { propertyNameInCamelCasing };
                    }
                    else
                    {
                        schema.Required.Add(propertyNameInCamelCasing);
                    }
                }
            }
        }
    }
}
