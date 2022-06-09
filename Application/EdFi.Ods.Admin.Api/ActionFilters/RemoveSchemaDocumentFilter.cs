// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.Admin.Api.ActionFilters
{
    public class RemoveSchemaDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var existingSchemas = swaggerDoc.Components.Schemas;
            var filered = new Dictionary<string, OpenApiSchema>();
            foreach (var schemaToRemove in existingSchemas.Where(x => x.Key.ToLower().Contains("iresult")))
            {
                swaggerDoc.Components.Schemas.Remove(schemaToRemove.Key);
            }
        }
    }
}
