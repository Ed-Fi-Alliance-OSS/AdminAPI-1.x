// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using FluentValidation;

namespace EdFi.Ods.Admin.Api.Features.Vendors
{
    public class EditVendorModel : IEditVendor
    {
        public int VendorId { get; set; }
        public string? Company { get; set; }
        public string? NamespacePrefixes { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEmailAddress { get; set; }
    }

    public class EditVendorModelValidator : AbstractValidator<EditVendorModel>
    {
        public EditVendorModelValidator()
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
