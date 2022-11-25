// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor;

public class GetAllAuthorizationStrategiesQuery
{
    private ISecurityContext _securityContext;

    public GetAllAuthorizationStrategiesQuery(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }

    public List<AuthorizationStrategy> Execute()
    {
        return _securityContext.AuthorizationStrategies
            .OrderBy(x => x.AuthorizationStrategyName)
            .Select(x => new AuthorizationStrategy
            {
                AuthStrategyId = x.AuthorizationStrategyId,
                AuthStrategyName = x.AuthorizationStrategyName,
                DisplayName = x.DisplayName
            }).ToList();
    }
}
