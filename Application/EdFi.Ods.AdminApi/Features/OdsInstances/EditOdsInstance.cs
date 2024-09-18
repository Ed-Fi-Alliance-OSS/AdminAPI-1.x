// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.OdsInstances;

public class EditOdsInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
            .MapPut(endpoints, "/odsInstances/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponseCode(200))
            .BuildForVersions(AdminApiVersions.V1);
    }

    public async Task<IResult> Handle(Validator validator, IEditOdsInstanceCommand editOdsInstanceCommand, IMapper mapper, EditOdsInstanceRequest request, int id)
    {
        request.Id = id;
        await validator.GuardAsync(request);
        editOdsInstanceCommand.Execute(request);
        return Results.Ok();
    }

    [SwaggerSchema(Title = "EditOdsInstanceRequest")]
    public class EditOdsInstanceRequest : IEditOdsInstanceModel
    {
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceName, Nullable = false)]
        public string? Name { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceInstanceType, Nullable = true)]
        public string? InstanceType { get; set; }
        [SwaggerExclude]
        public int Id { get; set; }
    }

    public class Validator : AbstractValidator<IEditOdsInstanceModel>
    {
        private readonly IGetOdsInstancesQuery _getOdsInstancesQuery;
        private readonly IGetOdsInstanceQuery _getOdsInstanceQuery;
        private readonly string _databaseEngine;

        public Validator(IGetOdsInstancesQuery getOdsInstancesQuery, IGetOdsInstanceQuery getOdsInstanceQuery)
        {
            _getOdsInstancesQuery = getOdsInstancesQuery;
            _getOdsInstanceQuery = getOdsInstanceQuery;

            RuleFor(m => m.Name)
                .NotEmpty()
                .Must(BeAUniqueName)
                .WithMessage(FeatureConstants.ClaimSetAlreadyExistsMessage)
                .When(m => BeAnExistingOdsInstance(m.Id) && NameIsChanged(m));

            RuleFor(m => m.InstanceType)
                .MaximumLength(100)
                .When(m => !string.IsNullOrEmpty(m.InstanceType));
        }

        private bool BeAnExistingOdsInstance(int id)
        {
            _getOdsInstanceQuery.Execute(id);
            return true;
        }

        private bool NameIsChanged(IEditOdsInstanceModel model)
        {
            return _getOdsInstanceQuery.Execute(model.Id).Name != model.Name;
        }

        private bool BeAUniqueName(string? name)
        {
            return _getOdsInstancesQuery.Execute().TrueForAll(x => x.Name != name);
        }
    }
}
