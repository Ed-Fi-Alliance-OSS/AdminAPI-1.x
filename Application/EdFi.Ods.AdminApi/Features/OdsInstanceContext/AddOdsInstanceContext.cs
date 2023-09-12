// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using FluentValidation;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.OdsInstanceContext;

public class AddOdsInstanceContext : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
           .MapPost(endpoints, "/odsInstancesContexts", Handle)
           .WithDefaultDescription()
           .WithRouteOptions(b => b.WithResponseCode(201))
           .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, IAddOdsInstanceContextCommand addOdsInstanceContextCommand, IMapper mapper, AddOdsInstanceContextRequest request)
    {
        await validator.GuardAsync(request);
        var addedOdsInstanceContext = addOdsInstanceContextCommand.Execute(request);
        return Results.Created($"/odsInstancesContexts/{addedOdsInstanceContext.OdsInstanceContextId}", null);
    }


    [SwaggerSchema(Title = "AddOdsInstanceContextRequest")]
    public class AddOdsInstanceContextRequest : IAddOdsInstanceContextModel
    {
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceContextOdsInstanceIdDescription, Nullable = false)]
        public int OdsInstanceId { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceContextContextKeyDescription, Nullable = false)]
        public string? ContextKey { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceContextContextValueDescription, Nullable = false)]
        public string? ContextValue { get; set; }
    }

    public class Validator : AbstractValidator<AddOdsInstanceContextRequest>
    {
        private readonly IGetOdsInstanceQuery _getOdsInstanceQuery;
        private readonly string _databaseEngine;
        public Validator(IGetOdsInstanceQuery getOdsInstanceQuery, IOptions<AppSettings> options)
        {
            _getOdsInstanceQuery = getOdsInstanceQuery;
            _databaseEngine = options.Value.DatabaseEngine ?? throw new NotFoundException<string>("AppSettings", "DatabaseEngine");

            RuleFor(m => m.ContextKey).NotEmpty();

            RuleFor(m => m.ContextValue).NotEmpty();

            RuleFor(m => m.OdsInstanceId)
                .NotEqual(0)
                .WithMessage(FeatureConstants.OdsInstanceIdValidationMessage);

            RuleFor(m => m.OdsInstanceId)
                .Must(BeAnExistingOdsInstance)
                .When(m => !m.OdsInstanceId.Equals(0));

        }

        private bool BeAnExistingOdsInstance(int id)
        {
            try
            {
                var odsInstance = _getOdsInstanceQuery.Execute(id) ?? throw new AdminApiException("Not Found");
                return true;
            }
            catch (AdminApiException)
            {
                throw new NotFoundException<int>("OdsInstanceId", id);
            }
        }
    }
}
