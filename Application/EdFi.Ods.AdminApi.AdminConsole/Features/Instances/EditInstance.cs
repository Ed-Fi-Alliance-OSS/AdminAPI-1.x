// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using System.ComponentModel.DataAnnotations;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using FluentValidation;
using static EdFi.Ods.AdminApi.AdminConsole.Features.Instances.AddInstance;
using System.Dynamic;
using System.Text.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

public class EditInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPatch(endpoints, "/instances/{odsinstanceid}", Execute)
            .WithRouteOptions(b => b.WithResponseCode(204))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    public async Task<IResult> Execute(Validator validator, IEditInstanceCommand editInstanceCommand, EditInstanceRequest request, int odsInstanceId)
    {
        await validator.GuardAsync(request);
        var instance = await editInstanceCommand.Execute(odsInstanceId, request);
        return Results.NoContent();
    }

    public class EditInstanceRequest : IEditInstanceModel
    {
        [Required]
        public ExpandoObject Document { get; set; }
    }

    public class Validator : AbstractValidator<EditInstanceRequest>
    {
        public Validator()
        {
            RuleFor(m => m.Document)
             .NotNull()
             .NotEmpty()
             .Must(BeValidDocument).WithMessage("Document must be a valid JSON.");
        }

        private bool BeValidDocument(ExpandoObject document)
        {
            try
            {
                var jDocument = JsonSerializer.Serialize(document);
                Newtonsoft.Json.Linq.JToken.Parse(jDocument);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
