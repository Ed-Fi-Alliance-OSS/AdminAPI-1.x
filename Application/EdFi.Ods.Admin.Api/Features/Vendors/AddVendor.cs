// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using FluentValidation;

namespace EdFi.Ods.Admin.Api.Features.Vendors
{
    public class AddVendor : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost($"/{FeatureConstants.Vendors}", Handle).RequireAuthorization()
                .WithTags(FeatureConstants.Vendors)
                .WithMetadata(new OperationOrderAttribute(3));
        }

        public async Task<IResult> Handle(Validator validator, AddVendorCommand addVendorCommand, IMapper mapper, Request request)
        {
            await validator.GuardAsync(request);
            var addedVendor = addVendorCommand.Execute(request);
            var model = mapper.Map<VendorModel>(addedVendor);
            return AdminApiResponse<VendorModel>.Created(model, "Vendor", $"/{FeatureConstants.Vendors}/{model.VendorId}");
        }

        public class Request : IAddVendorModel
        {
            public string? Company { get; set; }
            public string? NamespacePrefixes { get; set; }
            public string? ContactName { get; set; }
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
