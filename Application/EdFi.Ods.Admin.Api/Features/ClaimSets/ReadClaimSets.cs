// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets;

public class ReadClaimSets : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetWithDefaultOptions($"/{FeatureConstants.ClaimSets}", GetClaimSets, FeatureConstants.ClaimSets);
    }

    internal Task<IResult> GetClaimSets(GetClaimSetNamesQuery getClaimSetsQuery)
    {
        var calimSets = getClaimSetsQuery.Execute().ToList();
        return Task.FromResult(AdminApiResponse<List<string>>.Ok(calimSets));
    }
}
