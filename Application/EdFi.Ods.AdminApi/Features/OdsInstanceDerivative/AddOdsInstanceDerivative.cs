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
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.OdsInstanceDerivative;

public class AddOdsInstanceDerivative : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
           .MapPost(endpoints, "/odsInstancesDerivatives", Handle)
           .WithDefaultDescription()
           .WithRouteOptions(b => b.WithResponseCode(201))
           .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, IAddOdsInstanceDerivativeCommand addOdsInstanceDerivativeCommand, IMapper mapper, AddOdsInstanceDerivativeRequest request)
    {
        await validator.GuardAsync(request);
        var addedOdsInstanceDerivative = addOdsInstanceDerivativeCommand.Execute(request);
        return Results.Created($"/odsInstancesDerivatives/{addedOdsInstanceDerivative.OdsInstanceDerivativeId}", null);
    }

  
    [SwaggerSchema(Title = "AddOdsInstanceDerivativeRequest")]
    public class AddOdsInstanceDerivativeRequest : IAddOdsInstanceDerivativeModel
    {
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeOdsInstanceIdDescription, Nullable = false)]
        public int OdsInstanceId { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeDerivativeTypeDescription, Nullable = false)]
        public string? DerivativeType { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeConnectionStringDescription, Nullable = false)]
        public string? ConnectionString { get; set; }
    }

    public class Validator : AbstractValidator<AddOdsInstanceDerivativeRequest>
    {
        private readonly IGetOdsInstanceQuery _getOdsInstanceQuery;
        private readonly string _databaseEngine;
        public Validator(IGetOdsInstanceQuery getOdsInstanceQuery, IOptions<AppSettings> options)
        {
            _getOdsInstanceQuery = getOdsInstanceQuery;
            _databaseEngine = options.Value.DatabaseEngine ?? throw new NotFoundException<string>("AppSettings", "DatabaseEngine");

            RuleFor(m => m.DerivativeType).NotEmpty();

            RuleFor(m => m.DerivativeType)
                .Matches("^(ReadReplica|Snapshot)$")
                .WithMessage(FeatureConstants.OdsInstanceDerivativeDerivativeTypeNotValid)
                .When(m => !string.IsNullOrEmpty(m.DerivativeType));

            RuleFor(m => m.OdsInstanceId)
                .NotEqual(0)
                .WithMessage(FeatureConstants.OdsInstanceIdValidationMessage);

            RuleFor(m => m.OdsInstanceId)
                .Must(BeAnExistingOdsInstance)
                .When(m => !m.OdsInstanceId.Equals(0));

            RuleFor(m => m.ConnectionString)
                .NotEmpty();

            RuleFor(m => m.ConnectionString)
                .Must(BeAValidConnectionString)
                .WithMessage(FeatureConstants.OdsInstanceConnectionStringInvalid)
                .When(m => !string.IsNullOrEmpty(m.ConnectionString));

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

        private bool BeAValidConnectionString(string? connectionString)
        {
            return ConnectionStringHelper.ValidateConnectionString(_databaseEngine, connectionString);
        }

    }
}
