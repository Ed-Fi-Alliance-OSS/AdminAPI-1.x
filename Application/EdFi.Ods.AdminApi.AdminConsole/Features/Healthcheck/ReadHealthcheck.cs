// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck
{
    public class ReadHealthcheck : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/healthcheck", GetHealthcheck)
          .BuildForVersions();
        }

        internal Task<IResult> GetHealthcheck()
        {
            using (StreamReader r = new StreamReader("Mockdata/data-healthcheck.json"))
            {
                string json = r.ReadToEnd();
                HealthcheckModel? result = JsonConvert.DeserializeObject<HealthcheckModel>(json);
                return Task.FromResult(Results.Ok(result));
            }
        }
    }
}
