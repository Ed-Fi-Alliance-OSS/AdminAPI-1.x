// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;

public interface IGetTenantsQuery
{
    Task<IEnumerable<Tenant>> Execute();
}

public class GetTenantsQuery : IGetTenantsQuery
{
    private readonly IQueriesRepository<Tenant> _tenantQuery;

    public GetTenantsQuery(IQueriesRepository<Tenant> tenantQuery)
    {
        _tenantQuery = tenantQuery;
    }

    public async Task<IEnumerable<Tenant>> Execute()
    {
        return await _tenantQuery.GetAllAsync();
    }
}

