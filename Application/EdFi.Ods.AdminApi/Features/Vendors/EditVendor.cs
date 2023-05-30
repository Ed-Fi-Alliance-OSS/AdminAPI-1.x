// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Vendors;

public class EditVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPut(endpoints, "/vendors/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<VendorModel>(200))
            .BuildForVersions(AdminApiVersions.V1);
    }

    public async Task<IResult> Handle(EditVendorCommand editVendorCommand, IMapper mapper,
                       Validator validator, Request request, int id)
    {
        request.VendorId = id;
        await validator.GuardAsync(request);
        var updatedVendor = editVendorCommand.Execute(request);
        var model = mapper.Map<VendorModel>(updatedVendor);
        return AdminApiResponse<VendorModel>.Updated(model, "Vendor");
    }

    [SwaggerSchema(Title = "EditVendorRequest")]
    public class Request : IEditVendor
    {
        [SwaggerSchema(Description = FeatureConstants.VedorIdDescription, Nullable = false)]
        public int VendorId { get; set; }

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
            RuleFor(m => m.VendorId).Must(id => id > 0).WithMessage("Please provide valid Vendor Id.");
            RuleFor(m => m.Company).NotEmpty();
            RuleFor(m => m.Company)
                .Must(name => !VendorExtensions.IsSystemReservedVendorName(name))
                .WithMessage(p => $"'{p.Company}' is a reserved name and may not be used. Please choose another name.");

            RuleFor(m => m.ContactName).NotEmpty();
            RuleFor(m => m.ContactEmailAddress).NotEmpty().EmailAddress();
        }
    }
}
