// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ErrorHandling;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EdFi.Ods.Admin.Api.Features;

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
                    response.StatusCode = (int)HttpStatusCode.BadRequest;

                    var message = new
                    {
                        message = "Validation failed",
                        errors = validationException.Errors.Select(x => new
                        {
                            property = x.PropertyName,
                            message = x.ErrorMessage.Replace("\u0027", "'")
                        })
                    };
                    logger.LogDebug(JsonSerializer.Serialize(new { message, traceId = context.TraceIdentifier }));

                    await response.WriteAsync(JsonSerializer.Serialize(message));
                    break;

                case INotFoundException notFoundException:
                    logger.LogDebug(JsonSerializer.Serialize(new { message = notFoundException.Message, traceId = context.TraceIdentifier }));
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    logger.LogError(JsonSerializer.Serialize(new { message = "An uncaught error has occurred", error = ex, traceId = context.TraceIdentifier }));
                    await response.WriteAsync(JsonSerializer.Serialize(new { message = ex?.Message }));
                    break;
            }
        }
    }
}
