// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FluentValidation;
using System.Xml;
using EdFi.Ods.AdminApi.Features.Vendors;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Profiles;

public class AddProfile : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {  
        AdminApiEndpointBuilder
           .MapPost(endpoints, "/profiles/json", HandleJson)
           .WithDefaultDescription()
           .WithRouteOptions(b => b.WithResponseCode(201))
           .BuildForVersions(AdminApiVersions.V2);
    }
  
    public async Task<IResult> HandleJson(Validator validator, IAddProfileCommand addProfileCommand, IMapper mapper, AddProfileRequest request)
    {
        await validator.GuardAsync(request);
        var addedProfile = addProfileCommand.Execute(request);
        return Results.Created($"/profiles/{addedProfile.ProfileId}", null);    
    }

    [SwaggerSchema(Title = "AddProfileRequest")]
    public class AddProfileRequest : IAddProfileModel
    {
        [SwaggerSchema(Description = FeatureConstants.ProfileName, Nullable = false)]
        public string? Name { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ProfileDefinition, Nullable = false)]
        public string? Definition { get; set; }
    }

    public class Validator : AbstractValidator<AddProfileRequest>
    {
        public Validator()
        {
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.Definition).NotEmpty();
            RuleFor(m => m).Custom((profile, context) =>
            {
                if (!string.IsNullOrEmpty(profile.Definition))
                {
                    var validator = new ProfileValidator();
                    validator.Validate(profile.Name!, profile.Definition, context);
                }
            });
        }
    }   
}
