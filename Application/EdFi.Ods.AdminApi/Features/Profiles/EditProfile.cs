// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FluentValidation;
using System.Xml;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;

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

    [XmlRequestPayload]
    public async Task<IResult> Handle(Validator validator, IEditProfileCommand editProfileCommand, IMapper mapper, HttpRequest xmlRequest, int id)   
    {       
        var request = new EditProfileRequest();
        var reader = new StreamReader(xmlRequest.Body);
        request.Definition = reader.ReadToEndAsync().Result;
        await validator.GuardAsync(request);

        var doc = new XmlDocument();
        doc.LoadXml(request.Definition);
        var root = doc.DocumentElement;
        var profileName = root?.Attributes["name"]?.Value;

        if (!string.IsNullOrEmpty(profileName))
        {
            request.Id = id;
            request.Name = profileName;
            editProfileCommand.Execute(request);
            return Results.Ok();
        }
        return Results.BadRequest("Profile name is empty on the provided xml.");
    }  

    public class EditProfileRequest : IEditProfile
    {       
        public string? Name { get; set; }       
        public string? Definition { get; set; }
        public int Id { get; set; }
    }

    public class Validator : AbstractValidator<EditProfileRequest>
    {
        public Validator()
        {         
            RuleFor(m => m.Definition).NotEmpty();       
            RuleFor(m => m).Custom((profile, context) =>
            {
                if (!string.IsNullOrEmpty(profile.Definition))
                {
                    var validator = new ProfileValidator();
                    validator.Validate(profile.Definition, context);
                }
            });
        }
    }   
}
