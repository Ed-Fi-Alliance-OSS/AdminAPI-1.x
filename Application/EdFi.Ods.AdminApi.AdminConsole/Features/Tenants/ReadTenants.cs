// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadTenants : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/tenants", GetTenants)
           .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/tenant", GetTenant)
           .BuildForVersions();
    }

    internal Task<IResult> GetTenants()
    {
        using (StreamReader r = new StreamReader("Mockdata/data-tenants.json"))
        {
            string json = r.ReadToEnd();
            List<Tenant> result = JsonConvert.DeserializeObject<List<Tenant>>(json);
            return Task.FromResult(Results.Ok(result));
        }
    }

    internal Task<IResult> GetTenant(int id)
    {
        using (StreamReader r = new StreamReader("Mockdata/data-tenant.json"))
        {
            string json = r.ReadToEnd();
            Tenant result = JsonConvert.DeserializeObject<Tenant>(json);
            return Task.FromResult(Results.Ok(result));
        }
    }
}
