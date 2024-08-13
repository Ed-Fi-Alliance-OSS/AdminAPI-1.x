// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetAuthStrategiesQuery
{
    List<AuthorizationStrategy> Execute();
    List<AuthorizationStrategy> Execute(CommonQueryParams commonQueryParams);
}

public class GetAuthStrategiesQuery : IGetAuthStrategiesQuery
{
    private readonly ISecurityContext _context;
    private readonly IOptions<AppSettings> _options;

    public GetAuthStrategiesQuery(ISecurityContext context, IOptions<AppSettings> options)
    {
        _context = context;
        _options = options;
    }

    public List<AuthorizationStrategy> Execute()
    {
        return _context.AuthorizationStrategies.OrderBy(v => v.AuthorizationStrategyName).ToList();
    }

    public List<AuthorizationStrategy> Execute(CommonQueryParams commonQueryParams)
    {
        return _context.AuthorizationStrategies
            .OrderBy(v => v.AuthorizationStrategyName)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}
