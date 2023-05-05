// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using ClaimSet = EdFi.Ods.AdminApp.Management.ClaimSetEditor.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor;

/// <summary>
/// Compatibility copy of EdFi.Ods.AdminApp.Management.ClaimSetEditor.GetAllClaimSetsQuery
///
/// Since the projected ClaimSet does not include the new columns from v6.1, this query does not
/// require multiple versions. However, in order to preserve consistent test results, we must
/// construct dependent services using a query against the same database.
/// </summary>
internal class GetAllClaimSets53Query : IGetAllClaimSetsQuery
{
    private readonly ISecurityContext _securityContext;

    public GetAllClaimSets53Query(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }

    public IEnumerable<ClaimSet> Execute()
    {
        return _securityContext.ClaimSets
            .Select(x => new ClaimSet
            {
                Id = x.ClaimSetId,
                Name = x.ClaimSetName
            })
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }
}

