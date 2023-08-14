// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Security.DataAccess.Contexts;
using Action = EdFi.Security.DataAccess.Models.Action;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetAllActionsQuery
{
    IReadOnlyList<Action> Execute();
}

public class GetAllActionsQuery : IGetAllActionsQuery
{
    private readonly ISecurityContext _securityContext;

    public GetAllActionsQuery(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }
    public IReadOnlyList<Action> Execute()
    {
        return _securityContext.Actions.ToList();
    }
}
