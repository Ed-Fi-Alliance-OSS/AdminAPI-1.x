// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetOdsInstancesQuery
{
    List<OdsInstance> Execute();
    List<OdsInstance> Execute(int offset, int limit, int? id, string? name);
}

public class GetOdsInstancesQuery : IGetOdsInstancesQuery
{
    private readonly IUsersContext _usersContext;

    public GetOdsInstancesQuery(IUsersContext userContext)
    {
        _usersContext = userContext;
    }

    public List<OdsInstance> Execute()
    {
        return _usersContext.OdsInstances.OrderBy(odsInstance => odsInstance.Name).ToList();
    }

    public List<OdsInstance> Execute(int offset, int limit, int? id, string? name)
    {
        return _usersContext.OdsInstances
            .Where(o => id == null || o.OdsInstanceId == id)
            .Where(o => name == null || o.Name == name)
            .OrderBy(odsInstance => odsInstance.Name)
            .Skip(offset).Take(limit).ToList();
    }
}
