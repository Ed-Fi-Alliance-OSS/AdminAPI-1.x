// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

namespace EdFi.Ods.AdminApi.Infrastructure.AutoMapper;

public class AuthStrategyIdConverter : IValueConverter<string, int>
{
    private readonly IGetAllAuthorizationStrategiesQuery _getAllAuthorizationStrategiesQuery;
    public AuthStrategyIdConverter(IGetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery)
    {
        _getAllAuthorizationStrategiesQuery = getAllAuthorizationStrategiesQuery;
    }
    public int Convert(string sourceMember, ResolutionContext context)
    {
        var result = 0;
        var authStrategy = _getAllAuthorizationStrategiesQuery.Execute()
            .FirstOrDefault(a => a.AuthStrategyName == sourceMember);

        if (authStrategy == null)
        {
            throw new Exception("Error transforming the ID for the AuthStrategyName");
        }
        else
        {
            result = authStrategy!.AuthStrategyId;
        }

        return result;

    }
}
