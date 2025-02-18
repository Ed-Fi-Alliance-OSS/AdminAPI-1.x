// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using Microsoft.Extensions.Configuration;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Extensions;

public static class IConfigurationExtensions
{
    public static T Get<T>(this IConfiguration configuration, string key, T defaultValue)
    {
        return configuration.GetValue<T>(key, defaultValue) ?? defaultValue;
    }

    public static T Get<T>(this IConfiguration configuration, string key)
    {
        return configuration.GetValue<T>(key)
            ?? throw new AdminApiException($"Unable to load {key} from appSettings");
    }

    public static string GetConnectionStringByName(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name)
            ?? throw new AdminApiException($"Missing connection string {name}.");
    }
}
