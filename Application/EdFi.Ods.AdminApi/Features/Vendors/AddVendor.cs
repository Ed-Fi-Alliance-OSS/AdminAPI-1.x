// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Vendors;

public class AddVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
            .MapPost(endpoints, "/vendors", Handle)
            .WithDefaultSummaryAndDescription()
            .WithRouteOptions(b => b.WithResponseCode(201))
            .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, AddVendorCommand addVendorCommand, IMapper mapper, AddVendorRequest request)
    {
        await validator.GuardAsync(request);
        var addedVendor = addVendorCommand.Execute(request);
        return Results.Created($"/vendors/{addedVendor.VendorId}", null);
    }

    [SwaggerSchema(Title = "AddVendorRequest")]
    public class AddVendorRequest : IAddVendorModel
    {
        [SwaggerSchema(Description = FeatureConstants.VendorNameDescription, Nullable = false)]
        public string? Company { get; set; }

        [SwaggerSchema(Description = FeatureConstants.VendorNamespaceDescription, Nullable = false)]
        public string? NamespacePrefixes { get; set; }

        [SwaggerSchema(Description = FeatureConstants.VendorContactDescription, Nullable = false)]
        public string? ContactName { get; set; }

        [SwaggerSchema(Description = FeatureConstants.VendorContactEmailDescription, Nullable = false)]
        public string? ContactEmailAddress { get; set; }
    }

    public class Validator : AbstractValidator<AddVendorRequest>
    {
        public Validator()
        {
            RuleFor(m => m.Company).NotEmpty();
            RuleFor(m => m.Company)
                .Must(name => !VendorExtensions.IsSystemReservedVendorName(name))
                .WithMessage(p => $"'{p.Company}' is a reserved name and may not be used. Please choose another name.");

            RuleFor(m => m.ContactName).NotEmpty();
            RuleFor(m => m.ContactEmailAddress).NotEmpty().EmailAddress();
        }
    }
}
