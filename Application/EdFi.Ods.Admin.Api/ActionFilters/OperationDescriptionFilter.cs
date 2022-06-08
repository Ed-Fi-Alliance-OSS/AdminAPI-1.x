// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.Admin.Api.ActionFilters
{
    public class OperationDescriptionFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var description = context.ApiDescription.ActionDescriptor?.
                EndpointMetadata?.FirstOrDefault(x => x is OperationDescriptionAttribute) as OperationDescriptionAttribute;

            if(description != null)
            {
                operation.Description = description.Description;
                operation.Summary = description.Summary;
            }
        }
    }
}
