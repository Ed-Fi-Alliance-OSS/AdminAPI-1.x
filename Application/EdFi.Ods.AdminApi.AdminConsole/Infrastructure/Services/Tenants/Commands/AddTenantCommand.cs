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

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Commands;

public interface IAddTenantCommand
{
    Task<Tenant> Execute(IAddTenantModel newTenant);
}

internal class AddTenantCommand : IAddTenantCommand
{
    private readonly ICommandRepository<Tenant> _tenantCommand;
    public AddTenantCommand(ICommandRepository<Tenant> tenantCommand)
    {
        _tenantCommand = tenantCommand;
    }
    public async Task<Tenant> Execute(IAddTenantModel newTenant)
    {
        return await _tenantCommand.AddAsync(new Tenant
        {
            Document = newTenant.Document,
            InstanceId = newTenant.InstanceId,
            TenantId = newTenant.TenantId,
            EdOrgId = newTenant.EdOrgId
        });
    }
}

public interface IAddTenantModel
{
    int InstanceId { get; }
    int EdOrgId { get; }
    int TenantId { get; }
    string Document { get; }
}



