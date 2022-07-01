// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.Admin.Api.ActionFilters
{
    public class OperationResponsesDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new List<KeyValuePair<string, OpenApiPathItem>>();

            // Responses
            var getOkResponse = new OpenApiResponse() { Description = "The requested resource was successfully retrieved." };
            var unAuthorizedResponse = new OpenApiResponse() { Description = "Unauthorized. The request requires authentication" };
            var unhandledErrorResponse = new OpenApiResponse() { Description = "An unhandled error occurred on the server. See the response body for details." };
            var deleteSuccessResponse = new OpenApiResponse() { Description = "The resource was successfully deleted." };
            var resourceNotFoundResponse = new OpenApiResponse() { Description = "The resource could not be found." };
            var postCreatedResponse = new OpenApiResponse() { Description = "The resource was created." };
            var badRequestResponse = new OpenApiResponse() { Description = "Bad Request. The request was invalid and cannot be completed." };
            var putUpdatedResponse = new OpenApiResponse() { Description = "The resource was updated." };

            foreach (var path in swaggerDoc.Paths)
            {
                var operations = new List<KeyValuePair<OperationType, OpenApiOperation>>();
                foreach (var operation in path.Value.Operations)
                {
                    // Add appropriate responses
                    operation.Value.Responses.Clear();
                    operation.Value.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), unAuthorizedResponse);
                    operation.Value.Responses.Add(StatusCodes.Status500InternalServerError.ToString(), unhandledErrorResponse);
                    switch (operation.Key)
                    {
                        case OperationType.Get:
                            operation.Value.Responses.Add(StatusCodes.Status200OK.ToString(), getOkResponse);
                            operation.Value.Responses.Add(StatusCodes.Status404NotFound.ToString(), resourceNotFoundResponse);
                            break;
                        case OperationType.Delete:
                            operation.Value.Responses.Add(StatusCodes.Status204NoContent.ToString(), deleteSuccessResponse);
                            operation.Value.Responses.Add(StatusCodes.Status404NotFound.ToString(), resourceNotFoundResponse);
                            break;
                        case OperationType.Post:
                            operation.Value.Responses.Add(StatusCodes.Status201Created.ToString(), postCreatedResponse);
                            operation.Value.Responses.Add(StatusCodes.Status400BadRequest.ToString(), badRequestResponse);
                            break;
                        case OperationType.Put:
                            operation.Value.Responses.Add(StatusCodes.Status204NoContent.ToString(), putUpdatedResponse);
                            operation.Value.Responses.Add(StatusCodes.Status400BadRequest.ToString(), badRequestResponse);
                            break;
                    }
                    operations.Add(operation);
                }
                path.Value.Operations.Clear();
                operations.ForEach(x => path.Value.Operations.Add(x.Key, x.Value));
                paths.Add(path);
            }
            swaggerDoc.Paths.Clear();
            paths.ForEach(x => swaggerDoc.Paths.Add(x.Key, x.Value));
        }
    }
}
