// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor.Extensions
{
    public static class AuthorizationStrategiesExtension
    {
        public static AuthorizationStrategy Create(this AuthorizationStrategy[] authorizationStrategies)
        {
            return authorizationStrategies[0];
        }

        public static AuthorizationStrategy Read(this AuthorizationStrategy[] authorizationStrategies)
        {
            return authorizationStrategies[1];
        }

        public static AuthorizationStrategy Update(this AuthorizationStrategy[] authorizationStrategies)
        {
            return authorizationStrategies[2];
        }

        public static AuthorizationStrategy Delete(this AuthorizationStrategy[] authorizationStrategies)
        {
            return authorizationStrategies[3];
        }

        public static AuthorizationStrategy[] AddAuthorizationStrategyOverrides(this AuthorizationStrategy[] authorizationStrategies,
            Security.DataAccess.Models.Action action, AuthorizationStrategy strategy)
        {
            if (action.ActionName == Action.Create.Value)
                authorizationStrategies[0] = strategy;
            else if (action.ActionName == Action.Read.Value)
                authorizationStrategies[1] = strategy;
            else if (action.ActionName == Action.Update.Value)
                authorizationStrategies[2] = strategy;
            else if (action.ActionName == Action.Delete.Value)
                authorizationStrategies[3] = strategy;

            return authorizationStrategies;
        }
    }
}
