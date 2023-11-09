// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.AdminApi.Infrastructure.Documentation
{
    public class SwaggerTenantIdHeaderFilter: IOperationFilter
    {
        private readonly bool multiTenancyEnabled;

        public SwaggerTenantIdHeaderFilter(IOptions<AppSettings> options)
        {
            multiTenancyEnabled = options.Value.MultiTenancy;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (multiTenancyEnabled)
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "tenant",
                    In = ParameterLocation.Header,
                    Description = "Tenant identifier",
                    Required = true
                });
            }
        }
    }
}
