// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Profiles;

public class EditProfile : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
            .MapPut(endpoints, "/profiles/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponseCode(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    [ProfileRequestExample]
    public async Task<IResult> Handle(Validator validator, IEditProfileCommand editProfileCommand, IMapper mapper, EditProfileRequest request, int id)   
    {
        await validator.GuardAsync(request);
        request.Id = id;
        editProfileCommand.Execute(request);
        return Results.Ok();
    }

    [SwaggerSchema(Title = "EditProfileRequest")]
    public class EditProfileRequest : IEditProfileModel
    {
        [SwaggerSchema(Description = FeatureConstants.ProfileName, Nullable = false)]
        public string? Name { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ProfileDefinition, Nullable = false)]
        public string? Definition { get; set; }

        [SwaggerExclude]
        [SwaggerSchema(Description = FeatureConstants.ProfileIdDescription, Nullable = false)]
        public int Id { get; set; }
    }

    public class Validator : AbstractValidator<EditProfileRequest>
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
