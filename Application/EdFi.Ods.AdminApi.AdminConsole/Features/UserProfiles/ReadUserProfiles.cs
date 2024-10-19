// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadUserProfiles : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/userprofile", GetUserProfiles)
           .BuildForVersions();
    }

    internal Task<IResult> GetUserProfiles()
    {
        using (StreamReader r = new StreamReader("Mockdata/data-userprofile.json"))
        {
            string json = r.ReadToEnd();
            ExpandoObject result = JsonConvert.DeserializeObject<ExpandoObject>(json);
            return Task.FromResult(Results.Ok(result));
        }
    }
}


public interface IFeature
{
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
