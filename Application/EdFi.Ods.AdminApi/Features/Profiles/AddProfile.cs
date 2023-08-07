// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using System.Xml;
using System.Xml.Schema;

namespace EdFi.Ods.AdminApi.Features.Profiles;

using EdFi.Ods.AdminApi.Features.Vendors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AddProfile : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
            .MapPost(endpoints, "/profiles", Handle)          
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ProfileModel>(201))            
            .BuildForVersions(AdminApiVersions.V2);
    }
    
    [Consumes("application/xml")]
    public async Task<IResult> Handle(Validator validator, IAddProfileCommand addProfileCommand, IMapper mapper, HttpRequest xmlRequest)   
    {       
        var request = new AddProfileRequest();
        var reader = new StreamReader(xmlRequest.Body);
        request.Definition = reader.ReadToEndAsync().Result;
        await validator.GuardAsync(request);

        var doc = new XmlDocument();
        doc.LoadXml(request.Definition);
        var root = doc.DocumentElement;
        var profileName = root?.Attributes["name"]?.Value;

        if (!string.IsNullOrEmpty(profileName))
        {
            var addedProfile = addProfileCommand.Execute(new AddProfileRequest
            {
                Name= profileName,
                Definition = request.Definition
            });
            var model = mapper.Map<ProfileModel>(addedProfile);
            return Results.Created($"/profiles/{model.Id}", model);
        }
        return Results.BadRequest("Profile name is empty on the provided xml.");       
    }  

    public class AddProfileRequest : IAddProfileModel
    {       
        public string? Name { get; set; }       
        public string? Definition { get; set; }
    }

    public class Validator : AbstractValidator<AddProfileRequest>
    {
        public Validator()
        {            
            RuleFor(m => m.Definition).NotEmpty();       
            RuleFor(m => m).Custom((profile, context) =>
            {
                if (!string.IsNullOrEmpty(profile.Definition))
                {
                    var document = new XmlDocument();
                    void EventHandler(object? sender, ValidationEventArgs e)
                    {
                        if (e.Severity == XmlSeverityType.Error)
                        {
                            context.AddFailure($"Error: {e.Message}");
                        }
                    }
                    document.LoadXml(profile.Definition);
                    //document.Validate(EventHandler);
                }
                else
                {
                    context.AddFailure("Input xml is empty. Please provide valid xml.");
                }            

            });

        }
    }   
}
