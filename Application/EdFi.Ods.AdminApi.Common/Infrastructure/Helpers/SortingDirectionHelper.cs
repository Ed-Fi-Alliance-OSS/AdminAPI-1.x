// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel;
using System.Runtime.Serialization;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

public static class SortingDirectionHelper
{
    public enum Direction
    {
        [Description("Ascending")]
        [EnumMember(Value = "Asc")]
        Ascending,

        [Description("Descending")]
        [EnumMember(Value = "Desc")]
        Descending
    }

    public static bool IsDescendingSorting(string? direction)
    {
        direction = GetNonEmptyOrDefault(direction);

        bool result = direction.ToLowerInvariant() switch
        {
            "asc" or "ascending" => false,
            "desc" or "descending" => true,
            _ => false,
        };

        return result;
    }

    public static string GetNonEmptyOrDefault(string? direction)
    {
        if (!string.IsNullOrEmpty(direction))
            return direction;
        return Direction.Ascending.ToString();
    }
}
