// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.OdsInstances;

public class AddOdsInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
           .MapPost(endpoints, "/odsInstances", Handle)
           .WithDefaultDescription()
           .WithRouteOptions(b => b.WithResponseCode(201))
           .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, IAddOdsInstanceCommand addOdsInstanceCommand, IMapper mapper, AddOdsInstanceRequest request)
    {
        await validator.GuardAsync(request);
        var addedProfile = addOdsInstanceCommand.Execute(request);
        return Results.Created($"/odsInstances/{addedProfile.OdsInstanceId}", null);
    }

    [SwaggerSchema(Title = "AddOdsInstanceRequest")]
    public class AddOdsInstanceRequest : IAddOdsInstanceModel
    {
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceName, Nullable = false)]
        public string? Name { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceInstanceType, Nullable = false)]
        public string? InstanceType { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceConnectionString, Nullable = false)]
        public string? ConnectionString { get; set; }

    }

    public class Validator : AbstractValidator<IAddOdsInstanceModel>
    {
        public Validator()
        {
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.InstanceType).NotEmpty();
            RuleFor(m => m.ConnectionString).NotEmpty();
            //TO-DO: Implement connection string format validator (Regex or SqlConnectionStringBuilder-NpgsqlConnectionStringBuilder)
        }
    }
}
