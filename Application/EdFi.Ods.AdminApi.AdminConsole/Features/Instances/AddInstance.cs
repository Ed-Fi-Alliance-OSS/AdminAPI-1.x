// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

public class AddInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/instances", Execute)
      .WithRouteOptions(b => b.WithResponseCode(201))
      .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    public async Task<IResult> Execute(Validator validator, IAddInstanceCommand addInstanceCommand, AddInstanceRequest request)
    {
        await validator.GuardAsync(request);
        var addedInstanceResult = await addInstanceCommand.Execute(request);

        return Results.Created($"/instances/{addedInstanceResult.TenantId}/{addedInstanceResult.DocId}", addedInstanceResult);
    }

    public class AddInstanceRequest : IAddInstanceModel
    {
        [Required]
        public int OdsInstanceId { get; set; }
        public int? EdOrgId { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public ExpandoObject Document { get; set; }
        [Required]
        public ExpandoObject ApiCredentials { get; set; }
    }

    public class Validator : AbstractValidator<AddInstanceRequest>
    {
        public Validator()
        {
            RuleFor(m => m.OdsInstanceId)
             .NotNull();

            RuleFor(m => m.EdOrgId)
             .NotNull();

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
