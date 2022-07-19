// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.Admin.Api.Infrastructure.Helpers;

namespace EdFi.Ods.Admin.Api.Features.Information;

public class ReadInformation : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("", GetInformation)
            .WithMetadata(new OperationDescriptionAttribute("Retrieve API informational metadata", null))
            .WithTags("Information")
            .AllowAnonymous();
    }

    internal InformationResult GetInformation()
    {
        var content = new InformationResult()
        {
            Version = ConstantsHelpers.Version,
            Build = ConstantsHelpers.Build
        };

        return content;
    }
}
