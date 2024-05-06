// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Common.Utils.Extensions;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace EdFi.Ods.AdminApi.Features;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
    {
        try
        {
            if (context.Request.Path.StartsWithSegments(new PathString("/.well-known")))
            {
                // Requests to the OpenId Connect ".well-known" endpoint are too chatty for informational logging, but could be useful in debug logging.
                logger.LogDebug(JsonSerializer.Serialize(new { path = context.Request.Path.Value, traceId = context.TraceIdentifier }));
            }
            else
            {
                logger.LogInformation(JsonSerializer.Serialize(new { path = context.Request.Path.Value, traceId = context.TraceIdentifier }));
            }
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (ex)
            {
                case ValidationException validationException:
                    var validationResponse = new
                    {
                        title = "Validation failed",
                        errors = new Dictionary<string, List<string>>()
                    };

                    validationException.Errors.ForEach(x =>
                    {
                        if (!validationResponse.errors.ContainsKey(x.PropertyName))
                        {
                            validationResponse.errors[x.PropertyName] = new List<string>();
                        }
                        validationResponse.errors[x.PropertyName].Add(x.ErrorMessage.Replace("\u0027", "'"));
                    });

#pragma warning disable S6667 // Logging in a catch clause should pass the caught exception as a parameter.
                    logger.LogDebug(JsonSerializer.Serialize(new { message = validationResponse, traceId = context.TraceIdentifier }));
#pragma warning restore S6667 // Logging in a catch clause should pass the caught exception as a parameter.

                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync(JsonSerializer.Serialize(validationResponse));
                    break;

                case INotFoundException notFoundException:
                    var notFoundResponse = new
                    {
                        title = notFoundException.Message,
                    };
                    logger.LogDebug(JsonSerializer.Serialize(new { message = notFoundResponse, traceId = context.TraceIdentifier }));

                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    await response.WriteAsync(JsonSerializer.Serialize(notFoundResponse));
                    break;

                case IAdminApiException adminApiException:
                    logger.LogError(JsonSerializer.Serialize(new { message = "An uncaught error has occurred", error = new { ex.Message, ex.StackTrace }, traceId = context.TraceIdentifier }));
                    response.StatusCode = adminApiException.StatusCode.HasValue ? (int)adminApiException.StatusCode : 500;
                    await response.WriteAsync(JsonSerializer.Serialize(new { message = "The server encountered an unexpected condition that prevented it from fulfilling the request." }));
                    break;

                default:
                    logger.LogError(JsonSerializer.Serialize(new { message = "An uncaught error has occurred", error = new { ex.Message, ex.StackTrace }, traceId = context.TraceIdentifier }));
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await response.WriteAsync(JsonSerializer.Serialize(new { message = "The server encountered an unexpected condition that prevented it from fulfilling the request." }));
                    break;
            }
        }
    }
}
