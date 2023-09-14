// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;


public interface IGetOdsInstanceContextsQuery
{
    List<OdsInstanceContext> Execute();
    List<OdsInstanceContext> Execute(int offset, int limit);
}

public class GetOdsInstanceContextsQuery : IGetOdsInstanceContextsQuery
{
    private readonly IUsersContext _usersContext;

    public GetOdsInstanceContextsQuery(IUsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public List<OdsInstanceContext> Execute()
    {
        return _usersContext.OdsInstanceContexts.OrderBy(p => p.ContextKey).ToList();
    }

    public List<OdsInstanceContext> Execute(int offset, int limit)
    {
        return _usersContext.OdsInstanceContexts.OrderBy(p => p.ContextKey).Skip(offset).Take(limit).ToList();
    }
}

