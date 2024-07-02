// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Security.DataAccess.Contexts;
using ClaimSet = EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSet;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetAllClaimSetsQuery
{
    IReadOnlyList<ClaimSet> Execute();
    IReadOnlyList<ClaimSet> Execute(int offset, int limit, int? id, string? name);
}

public class GetAllClaimSetsQuery : IGetAllClaimSetsQuery
{
    private readonly ISecurityContext _securityContext;

    public GetAllClaimSetsQuery(ISecurityContext securityContext)
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
                IsEditable = !x.IsEdfiPreset && !x.ForApplicationUseOnly
            })
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }

    public IReadOnlyList<ClaimSet> Execute(int offset, int limit, int? id, string? name)
    {
        return _securityContext.ClaimSets
            .Where(c => id == null || c.ClaimSetId == id)
            .Where(c => name == null || c.ClaimSetName == name)
            .Select(x => new ClaimSet
            {
                Id = x.ClaimSetId,
                Name = x.ClaimSetName,
                IsEditable = !x.IsEdfiPreset && !x.ForApplicationUseOnly
            })
            .Distinct()
            .OrderBy(x => x.Name)
            .Skip(offset)
            .Take(limit)
            .ToList();
    }
}
