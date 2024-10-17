// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Features.OdsInstances;
using EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadOdsInstances : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/odsinstances", GetOdsInstances)
           .BuildForVersions();
    }

    internal Task<IResult> GetOdsInstances()
    {
        using (StreamReader r = new StreamReader("Mockdata/data-odsinstances.json"))
        {
            string json = r.ReadToEnd();
            OdsInstance result = JsonConvert.DeserializeObject<OdsInstance>(json);
            return Task.FromResult(Results.Ok(result));
        }
    }
}
