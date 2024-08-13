// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure;

public struct CommonQueryParams
{
    [FromQuery(Name = "offset")]
    public int? Offset { get; set; }
    [FromQuery(Name = "limit")]
    public int? Limit { get; set; }
    [FromQuery(Name = "orderBy")]
    public string? OrderBy { get; set; }
    [BindNever]
    public string? OrderByDefault { get; set; }
    [FromQuery(Name = "direction")]
    public string? Direction { get; set; }
    [BindNever]
    public bool? IsDescending { get; set; }
    public CommonQueryParams() { }
    public CommonQueryParams(int? offset, int? limit)
    {
        Offset = offset;
        Limit = limit;
    }
    public CommonQueryParams(int? offset, int? limit, string? orderBy, string? direction, string? orderByDefault = "")
    {
        Offset = offset;
        Limit = limit;
        OrderBy = orderBy;
        OrderByDefault = orderByDefault;
        if (!string.IsNullOrEmpty(direction))
        {
            Direction = SortingDirectionHelper.GetNonEmptyOrDefault(direction);
            IsDescending = SortingDirectionHelper.IsDescendingSorting(Direction);
        }
    }

}

