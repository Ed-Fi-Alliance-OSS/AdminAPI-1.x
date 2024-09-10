// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Security.DataAccess.Contexts;
using ClaimSet = EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSet;

namespace EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor;


public class GetAllClaimSetsQueryV6Service
{
    private readonly ISecurityContext _securityContext;

    public GetAllClaimSetsQueryV6Service(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }

    public IReadOnlyList<ClaimSet> Execute()
    {
        return _securityContext.ClaimSets
            .Select(x => new ClaimSet
            {
                Id = x.ClaimSetId,
                Name = x.ClaimSetName,
                IsEditable = !x.ForApplicationUseOnly && !x.IsEdfiPreset &&
                !Constants.SystemReservedClaimSets.Contains(x.ClaimSetName)
            })
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }

    public IReadOnlyList<ClaimSet> Execute(CommonQueryParams commonQueryParams)
    {
        return _securityContext.ClaimSets
            .Select(x => new ClaimSet
            {
                Id = x.ClaimSetId,
                Name = x.ClaimSetName,
                IsEditable = !x.ForApplicationUseOnly && !x.IsEdfiPreset &&
                !Constants.SystemReservedClaimSets.Contains(x.ClaimSetName)
            })
            .Distinct()
            .OrderBy(x => x.Name)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit)
            .ToList();
    }
}
