// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Security.DataAccess.Contexts;
using Microsoft.Extensions.Options;
using Action = EdFi.Security.DataAccess.Models.Action;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetAllActionsQuery
{
    IReadOnlyList<Action> Execute();
    IReadOnlyList<Action> Execute(CommonQueryParams commonQueryParams, int? id, string? name);
}

public class GetAllActionsQuery : IGetAllActionsQuery
{
    private readonly ISecurityContext _securityContext;
    private readonly IOptions<AppSettings> _options;

    public GetAllActionsQuery(ISecurityContext securityContext, IOptions<AppSettings> options)
    {
        _securityContext = securityContext;
        _options = options;
    }

    public IReadOnlyList<Action> Execute()
    {
        return _securityContext.Actions.ToList();
    }

    public IReadOnlyList<Action> Execute(CommonQueryParams commonQueryParams, int? id, string? name)
    {
        return _securityContext.Actions
            .Where(a => id == null || a.ActionId == id)
            .Where(a => name == null || a.ActionName == name)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}
