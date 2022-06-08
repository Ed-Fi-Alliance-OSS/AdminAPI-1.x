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
    public class EditVendor : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut($"/{FeatureConstants.Vendors}" + "/{id}", Handle).RequireAuthorization()
                .WithTags(FeatureConstants.Vendors)
                .WithMetadata(new OperationOrderAttribute(4));
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

        public class Request : IEditVendor
        {
            public int VendorId { get; set; }
            public string? Company { get; set; }
            public string? NamespacePrefixes { get; set; }
            public string? ContactName { get; set; }
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
}
