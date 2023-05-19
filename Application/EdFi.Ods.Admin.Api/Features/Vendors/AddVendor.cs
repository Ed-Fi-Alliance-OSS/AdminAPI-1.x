// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.Admin.Api.Infrastructure.Database.Commands;
using EdFi.Ods.Admin.Api.Infrastructure.Database.Queries;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Features.Vendors;

public class AddVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
            .MapPost(endpoints, "/vendors", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<VendorModel>(201))
            .BuildForVersions(AdminApiVersions.V1);
    }

    public async Task<IResult> Handle(Validator validator, AddVendorCommand addVendorCommand, IMapper mapper, Request request)
    {
        await validator.GuardAsync(request);
        var addedVendor = addVendorCommand.Execute(request);
        var model = mapper.Map<VendorModel>(addedVendor);
        return AdminApiResponse<VendorModel>.Created(model, "Vendor", $"/vendors/{model.VendorId}");
    }

    [SwaggerSchema(Title = "AddVendorRequest")]
    public class Request : IAddVendorModel
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

    public class Validator : AbstractValidator<Request>
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
