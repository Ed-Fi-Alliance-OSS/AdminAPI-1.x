// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public interface IAuthStrategyResolver
{
    IEnumerable<ResourceClaim> ResolveAuthStrategies(IEnumerable<ResourceClaim> resourceClaims);
}

public class AuthStrategyResolver : IAuthStrategyResolver
{
    private readonly ISecurityContext _securityContext;

    public AuthStrategyResolver(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }

    public IEnumerable<ResourceClaim> ResolveAuthStrategies(IEnumerable<ResourceClaim> resourceClaims)
    {
        var dbAuthStrategies = _securityContext.AuthorizationStrategies;

        foreach (var claim in resourceClaims)
        {
            if (claim.AuthStrategyOverridesForCRUD != null && claim.AuthStrategyOverridesForCRUD.Any())
            {
                foreach (var authStrategyOverride in claim.AuthStrategyOverridesForCRUD.Where(x => x != null))
                {
                    if (authStrategyOverride is null) continue;

                    var authStrategy = dbAuthStrategies.SingleOrDefault(
                        x => x.AuthorizationStrategyName.Equals(
                            authStrategyOverride.AuthStrategyName,
                            StringComparison.InvariantCultureIgnoreCase));

                    if (authStrategy != null)
                    {
                        authStrategyOverride.AuthStrategyId = authStrategy.AuthorizationStrategyId;
                        authStrategyOverride.DisplayName = authStrategy.DisplayName;
                    }
                }
            }

            if (claim.Children?.Any() ?? false)
            {
                claim.Children = ResolveAuthStrategies(claim.Children).ToList();
            }

            yield return claim;
        }
    }
}
