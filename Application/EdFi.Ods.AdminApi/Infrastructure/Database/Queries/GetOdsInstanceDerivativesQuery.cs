// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;


public interface IGetOdsInstanceDerivativesQuery
{
    List<OdsInstanceDerivative> Execute();
    List<OdsInstanceDerivative> Execute(int offset, int limit);
}

public class GetOdsInstanceDerivativesQuery : IGetOdsInstanceDerivativesQuery
{
    private readonly IUsersContext _usersContext;

    public GetOdsInstanceDerivativesQuery(IUsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public List<OdsInstanceDerivative> Execute()
    {
        return _usersContext.OdsInstanceDerivatives.OrderBy(p => p.DerivativeType).ToList();
    }

    public List<OdsInstanceDerivative> Execute(int offset, int limit)
    {
        return _usersContext.OdsInstanceDerivatives.OrderBy(p => p.DerivativeType).Skip(offset).Take(limit).ToList();
    }
}

