// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Security.DataAccess.Contexts;
using Microsoft.Extensions.Options;
using ClaimSet = EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.ClaimSet;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetAllClaimSetsQuery
{
    IReadOnlyList<ClaimSet> Execute();
    IReadOnlyList<ClaimSet> Execute(CommonQueryParams commonQueryParams, int? id, string? name);
}

public class GetAllClaimSetsQuery : IGetAllClaimSetsQuery
{
    private readonly ISecurityContext _securityContext;
    private readonly IOptions<AppSettings> _options;
    public GetAllClaimSetsQuery(ISecurityContext securityContext, IOptions<AppSettings> options)
    {
        _securityContext = securityContext;
        _options = options;
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

    public IReadOnlyList<ClaimSet> Execute(CommonQueryParams commonQueryParams, int? id, string? name)
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
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}
