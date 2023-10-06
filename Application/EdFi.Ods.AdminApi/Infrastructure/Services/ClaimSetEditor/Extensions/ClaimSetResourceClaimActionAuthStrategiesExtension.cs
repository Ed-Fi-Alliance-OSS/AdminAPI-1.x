// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.Extensions;
using Action = EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor.Action;

namespace EdFi.Ods.AdminApi.Infrastructure.Services.ClaimSetEditor.Extensions;

public static class ClaimSetResourceClaimActionAuthStrategiesExtension
{
    public static ClaimSetResourceClaimActionAuthStrategies? Create(this ClaimSetResourceClaimActionAuthStrategies?[] claimSetResourceClaimActionAuthStrategies)
    {
        return claimSetResourceClaimActionAuthStrategies[0];
    }

    public static ClaimSetResourceClaimActionAuthStrategies? Read(this ClaimSetResourceClaimActionAuthStrategies?[] claimSetResourceClaimActionAuthStrategies)
    {
        return claimSetResourceClaimActionAuthStrategies[1];
    }

    public static ClaimSetResourceClaimActionAuthStrategies? Update(this ClaimSetResourceClaimActionAuthStrategies?[] claimSetResourceClaimActionAuthStrategies)
    {
        return claimSetResourceClaimActionAuthStrategies[2];
    }

    public static ClaimSetResourceClaimActionAuthStrategies? Delete(this ClaimSetResourceClaimActionAuthStrategies?[] claimSetResourceClaimActionAuthStrategies)
    {
        return claimSetResourceClaimActionAuthStrategies[3];
    }

    public static ClaimSetResourceClaimActionAuthStrategies? ReadChanges(this ClaimSetResourceClaimActionAuthStrategies?[] claimSetResourceClaimActionAuthStrategies)
    {
        return claimSetResourceClaimActionAuthStrategies[4];
    }

    public static ClaimSetResourceClaimActionAuthStrategies?[] AddAuthorizationStrategyOverrides(this ClaimSetResourceClaimActionAuthStrategies?[] claimSetResourceClaimActionAuthStrategies,
        string actionName, AuthorizationStrategy? strategy)
    {
        if (strategy != null)
        {
            if (actionName == Action.Create.Value)
            {
                if (claimSetResourceClaimActionAuthStrategies[0] == null)
                    claimSetResourceClaimActionAuthStrategies[0] = new ClaimSetResourceClaimActionAuthStrategies();
                claimSetResourceClaimActionAuthStrategies[0]!.AuthorizationStrategies.Add(strategy);
            }
            else if (actionName == Action.Read.Value)
            {
                if (claimSetResourceClaimActionAuthStrategies[1] == null)
                    claimSetResourceClaimActionAuthStrategies[1] = new ClaimSetResourceClaimActionAuthStrategies();
                claimSetResourceClaimActionAuthStrategies[1]!.AuthorizationStrategies.Add(strategy);
            }
            else if (actionName == Action.Update.Value)
            {
                if (claimSetResourceClaimActionAuthStrategies[2] == null)
                    claimSetResourceClaimActionAuthStrategies[2] = new ClaimSetResourceClaimActionAuthStrategies();
                claimSetResourceClaimActionAuthStrategies[2]!.AuthorizationStrategies.Add(strategy);
            }
            else if (actionName == Action.Delete.Value)
            {
                if (claimSetResourceClaimActionAuthStrategies[3] == null)
                    claimSetResourceClaimActionAuthStrategies[3] = new ClaimSetResourceClaimActionAuthStrategies();
                claimSetResourceClaimActionAuthStrategies[3]!.AuthorizationStrategies.Add(strategy);
            }
            else if (actionName == Action.ReadChanges.Value)
            {
                if (claimSetResourceClaimActionAuthStrategies[4] == null)
                    claimSetResourceClaimActionAuthStrategies[4] = new ClaimSetResourceClaimActionAuthStrategies();
                claimSetResourceClaimActionAuthStrategies[4]!.AuthorizationStrategies.Add(strategy);
            }
        }
        return claimSetResourceClaimActionAuthStrategies;
    }
}
