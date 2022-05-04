// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace EdFi.Ods.Admin.Api.Features;

public class ErrorHandler : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.Map("/error", HandleError);
    }

    internal Task<IResult> HandleError(HttpContext context, ILogger<ErrorHandler> logger)
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerFeature?.Error ?? new Exception();

        if (exception is ValidationException validationException)
        {
            logger.LogDebug(exception, "Validation error");
            return Task.FromResult(AdminApiError.Validation(validationException.Errors));
        }

        if (exception is NotFoundException<int>)
            return HandleNotFound<int>(logger, exception);
        if (exception is NotFoundException<Guid>)
            return HandleNotFound<Guid>(logger, exception);
        if (exception is NotFoundException<string>)
            return HandleNotFound<string>(logger, exception);

        logger.LogError(exception, "An uncaught error has occurred");
        return Task.FromResult(AdminApiError.Unexpected(exception));
    }

    private Task<IResult> HandleNotFound<T>(ILogger<ErrorHandler> logger, Exception exception)
    {
        if(exception is not NotFoundException<T> notFoundException)
            throw new ArgumentException("HandleNotFound<T>() must be called with a NotFoundException");

        logger.LogDebug(notFoundException, "Resource not found");
        return Task.FromResult(AdminApiError.NotFound(notFoundException.ResourceName, notFoundException.Id));
    }
}
