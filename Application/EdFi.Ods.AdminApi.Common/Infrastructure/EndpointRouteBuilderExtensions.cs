// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Common.Infrastructure;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder WithResponseCode(this RouteHandlerBuilder builder, int code, string? description = null)
    {
        builder.Produces(code);
        builder.WithMetadata(new SwaggerResponseAttribute(code, description));
        return builder;
    }

    public static RouteHandlerBuilder WithResponse<T>(this RouteHandlerBuilder builder, int code, string? description = null)
    {
        builder.Produces(code, responseType: typeof(T));
        builder.WithMetadata(new SwaggerResponseAttribute(code, description, typeof(T)));
        return builder;
    }
}
