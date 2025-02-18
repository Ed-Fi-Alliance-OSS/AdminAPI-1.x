// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static EdFi.Ods.AdminApi.AdminConsole.Features.Instances.EditInstance;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;
public class DeleteInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapDelete(endpoints, "/instances/{odsinstanceid}", Execute)
            .WithRouteOptions(b => b.WithResponseCode(200, FeatureCommonConstants.DeletedSuccessResponseDescription))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    public async Task<IResult> Execute(IDeleteInstanceCommand deleteInstanceCommand, int odsInstanceId)
    {
        await deleteInstanceCommand.Execute(odsInstanceId);
        return Results.Ok();
    }
}
