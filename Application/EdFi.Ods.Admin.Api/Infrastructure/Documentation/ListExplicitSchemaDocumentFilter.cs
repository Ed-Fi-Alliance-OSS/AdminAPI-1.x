// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Features;
using EdFi.Ods.Admin.Api.Features.Connect;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.Admin.Api.Infrastructure.Documentation;

public class ListExplicitSchemaDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(RegisterService.Request), context.SchemaRepository);
        context.SchemaGenerator.GenerateSchema(typeof(AdminApiResponse<object>), context.SchemaRepository);
        context.SchemaGenerator.GenerateSchema(typeof(AdminApiError), context.SchemaRepository);

    }
}
