// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.Admin.Api.Infrastructure.Database.Queries;

public interface IGetResourceClaimsAsFlatListQuery
{
    IReadOnlyList<ResourceClaim> Execute();
}

public class GetResourceClaimsAsFlatListQuery : IGetResourceClaimsAsFlatListQuery
{
    private readonly ISecurityContext _securityContext;

    public GetResourceClaimsAsFlatListQuery(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }

    public IReadOnlyList<ResourceClaim> Execute()
    {
        return _securityContext.ResourceClaims
            .Select(x => new ResourceClaim
            {
                Id = x.ResourceClaimId,
                Name = x.ResourceName,
                ParentId = x.ParentResourceClaimId ?? 0,
                ParentName = x.ParentResourceClaim.ResourceName
            })
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }
}
