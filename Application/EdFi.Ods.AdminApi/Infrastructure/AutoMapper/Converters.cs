// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

namespace EdFi.Ods.AdminApi.Infrastructure.AutoMapper;

public class AuthStrategyIdsConverter : IValueConverter<List<string>, List<int>>
{
    private readonly IGetAllAuthorizationStrategiesQuery _getAllAuthorizationStrategiesQuery;
    public AuthStrategyIdsConverter(IGetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery)
    {
        _getAllAuthorizationStrategiesQuery = getAllAuthorizationStrategiesQuery;
    }
    public List<int> Convert(List<string> authStrategyNames, ResolutionContext context)
    {
        var ids = new List<int>();
        if (authStrategyNames != null)
        {
            var unavailableAuthStrategies = string.Empty;
            foreach (var authStrategyName in authStrategyNames)
            {
                var authStrategy = _getAllAuthorizationStrategiesQuery.Execute()
                   .FirstOrDefault(a => authStrategyName.Equals(a.AuthStrategyName, StringComparison.InvariantCultureIgnoreCase));

                if (authStrategy == null)
                {
                    unavailableAuthStrategies = string.Join(",", authStrategyName);
                }
                else
                {
                    ids.Add(authStrategy!.AuthStrategyId);
                }
            }
            if (!string.IsNullOrEmpty(unavailableAuthStrategies))
            {
                throw new Exception($"Error transforming the ID for the AuthStrategyNames {unavailableAuthStrategies!}");
            }
        }
        return ids;
    }
}
