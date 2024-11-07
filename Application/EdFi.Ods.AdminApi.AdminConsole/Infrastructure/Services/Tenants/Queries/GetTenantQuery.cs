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

public interface IGetTenantQuery
{
    Task<Tenant> Get(int tenantId);
    Task<IEnumerable<Tenant>> GetAll();
}

public class GetTenantQuery : IGetTenantQuery
{
    private readonly IQueriesRepository<Tenant> _tenantQuery;

    public GetTenantQuery(IQueriesRepository<Tenant> tenantQuery)
    {
        _tenantQuery = tenantQuery;
    }
    public async Task<Tenant> Get(int tenantId)
    {
        return await _tenantQuery.Query().SingleOrDefaultAsync(tenant => tenant.TenantId == tenantId)
        ?? throw new Exception($"Not found {nameof(Tenant)} for Tenant Id: {tenantId}");
    }

    public async Task<IEnumerable<Tenant>> GetAll()
    {
        return await _tenantQuery.GetAllAsync();
    }
}

