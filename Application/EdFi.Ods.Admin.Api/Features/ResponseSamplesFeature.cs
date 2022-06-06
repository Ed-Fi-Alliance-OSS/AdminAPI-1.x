// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Features;

public class ResponseSamplesFeature : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/samples/error", Error);
        endpoints.MapPost("/samples/error", Error);
        endpoints.MapPut("/samples/error", Error);
        endpoints.MapDelete("/samples/error", Error);
    }

    internal IResult Error(HttpContext context) => throw new Exception("This is the exception message");
}
