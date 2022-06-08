// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using EdFi.Ods.Admin.Api.ActionFilters;

namespace EdFi.Ods.Admin.Api.Features.Vendors
{
    public class AddVendor : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPostWithDefaultOptions($"/{FeatureConstants.Vendors}", Handle, FeatureConstants.Vendors);
        }

        public async Task<IResult> Handle(Validator validator, AddVendorCommand addVendorCommand, IMapper mapper, Request request)
        {
            await validator.GuardAsync(request);
            var addedVendor = addVendorCommand.Execute(request);
            var model = mapper.Map<VendorModel>(addedVendor);
            return AdminApiResponse<VendorModel>.Created(model, "Vendor", $"/{FeatureConstants.Vendors}/{model.VendorId}");
        }

        [CustomSchemaName("AddVendor")]
        public class Request : IAddVendorModel
        {
            [SwaggerSchema(Description = "Company/ Vendor name")]
            public string? Company { get; set; }
            [SwaggerSchema(Description = "Namespace prefix for vendor")]
            public string? NamespacePrefixes { get; set; }
            [SwaggerSchema(Description = "Contact name")]
            public string? ContactName { get; set; }
            [SwaggerSchema(Description = "Contact email address")]
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
}
