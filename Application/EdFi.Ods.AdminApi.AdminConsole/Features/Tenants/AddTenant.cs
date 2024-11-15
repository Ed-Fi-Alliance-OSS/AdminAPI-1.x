// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Commands;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
internal class AddTenant : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapPost(endpoints, "/tenants", Execute)
            .WithRouteOptions(b => b.WithResponseCode(201))
            .BuildForVersions();
    }

    public async Task<IResult> Execute(Validator validator, IAddTenantCommand addTenantCommand, AddTenantRequest request)
    {
        await validator.GuardAsync(request);
        var addedTenant = await addTenantCommand.Execute(request);
        return Results.Created($"/tenants/{addedTenant.TenantId}", null);
    }

    public class AddTenantRequest : IAddTenantModel
    {
        [Required]
        public int InstanceId { get; set; }
        [Required]
        public int EdOrgId { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public string Document { get; set; }
    }
    internal class Validator : AbstractValidator<AddTenantRequest>
    {
        public Validator()
        {
            RuleFor(m => m.InstanceId)
                 .NotNull();

            RuleFor(m => m.EdOrgId)
                 .NotNull();

            RuleFor(m => m.TenantId)
                 .NotNull();

            RuleFor(m => m.Document)
                 .NotNull()
                 .Must(BeValidDocument).WithMessage("Document must be a valid JSON.");
        }

        private bool BeValidDocument(string document)
        {
            try
            {
                Newtonsoft.Json.Linq.JToken.Parse(document);
                return true;
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return false;
            }
        }
    }
}

