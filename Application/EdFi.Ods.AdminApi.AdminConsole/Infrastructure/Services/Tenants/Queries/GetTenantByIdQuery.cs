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
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;

public interface IGetTenantByIdQuery
{
    Task<Tenant> Execute(int docId);
}

public class GetTenantByIdQuery : IGetTenantByIdQuery
{
    private readonly IQueriesRepository<Tenant> _tenantQuery;

    public GetTenantByIdQuery(IQueriesRepository<Tenant> tenantQuery)
    {
        _tenantQuery = tenantQuery;
    }
    public async Task<Tenant> Execute(int docId)
    {
        return await _tenantQuery.Query().SingleOrDefaultAsync(tenant => tenant.DocId == docId)
        ?? throw new Exception($"Not found {nameof(Tenant)} for Tenant Id: {docId}");
    }
}

