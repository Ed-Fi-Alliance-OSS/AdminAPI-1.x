// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.Admin.Api.ActionFilters
{
    public class OrderOperationsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new List<KeyValuePair<string, OpenApiPathItem>>();

            foreach (var path in swaggerDoc.Paths)
            {
                var operations = new Dictionary<KeyValuePair<OperationType, OpenApiOperation>, int>();
                foreach (var operation in path.Value.Operations)
                {
                    if (context.ApiDescriptions.FirstOrDefault(MatchesUrlAndVerb(path, operation))
                        ?.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is OperationOrderAttribute)
                        is OperationOrderAttribute orderAttributeValue)
                    {
                        operations.Add(operation, orderAttributeValue.Order);
                    }
                    else
                    {
                        operations.Add(operation, 0);
                    }
                }
                var orderedOpertions = operations.OrderBy(x => x.Value).ToList();
                path.Value.Operations.Clear();
                orderedOpertions.ForEach(x => path.Value.Operations.Add(x.Key.Key, x.Key.Value));
                paths.Add(path);
            }
            swaggerDoc.Paths.Clear();
            paths.ForEach(x => swaggerDoc.Paths.Add(x.Key, x.Value));
        }

        private static Func<ApiDescription, bool> MatchesUrlAndVerb(KeyValuePair<string, OpenApiPathItem> path, KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            return apiDescription =>
            {
                var pathMatches = apiDescription.RelativePath!.Replace("/", string.Empty)
                    .Equals(path.Key.Replace("/", string.Empty), StringComparison.InvariantCultureIgnoreCase);
                var verbMatches = operation.Key.ToString().ToLower().Equals(apiDescription.HttpMethod!.ToLower());
                return pathMatches && verbMatches;
            };
        }
    }
}
