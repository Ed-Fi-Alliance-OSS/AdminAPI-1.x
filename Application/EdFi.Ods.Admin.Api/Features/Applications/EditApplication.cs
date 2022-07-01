// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using FluentValidation;
using FluentValidation.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Features.Applications
{
    public class EditApplication : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPutWithDefaultOptions($"/{FeatureConstants.Applications}" + "/{id}", Handle, FeatureConstants.Applications);
        }

        public async Task<IResult> Handle(IEditApplicationCommand editApplicationCommand, IMapper mapper,
            Validator validator, IUsersContext db, Request request, int id)
        {
            request.ApplicationId = id;
            await validator.GuardAsync(request);
            GuardAgainstInvalidEntityReferences(request, db);

            var updatedApplication = editApplicationCommand.Execute(request);
            var model = mapper.Map<ApplicationModel>(updatedApplication);
            return AdminApiResponse<ApplicationModel>.Updated(model, "Application");
        }

        private void GuardAgainstInvalidEntityReferences(Request request, IUsersContext db)
        {
            if(null == db.Vendors.Find(request.VendorId))
                throw new ValidationException(new []{ new ValidationFailure(nameof(request.VendorId), $"Vendor with ID {request.VendorId} not found.") });

            if (request.ProfileId.HasValue && db.Profiles.Find(request.ProfileId) == null)
                throw new ValidationException(new []{ new ValidationFailure(nameof(request.ProfileId), $"Profile with ID {request.ProfileId} not found.") });
        }

        [DisplaySchemaName(FeatureConstants.EditApplicationDisplayName)]
        public class Request : IEditApplicationModel
        {
            [SwaggerSchema(Description = FeatureConstants.ApplicationIdDescription, Nullable = false)]
            public int ApplicationId { get; set; }

            [SwaggerRequired]
            [SwaggerSchema(Description = FeatureConstants.ApplicationNameDescription, Nullable = false)]
            public string? ApplicationName { get; set; }

            [SwaggerRequired]
            [SwaggerSchema(Description = FeatureConstants.VedorIdDescription, Nullable = false)]
            public int VendorId { get; set; }

            [SwaggerRequired]
            [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
            public string? ClaimSetName { get; set; }

            [SwaggerSchema(Description = FeatureConstants.ProfileIdDescription)]
            public int? ProfileId { get; set; }

            [SwaggerRequired]
            [SwaggerSchema(Description = FeatureConstants.EducationOrganizationIdsDescription, Nullable = false)]
            public IEnumerable<int>? EducationOrganizationIds { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(m => m.ApplicationId).NotEmpty();
                RuleFor(m => m.ApplicationName).NotEmpty();
                RuleFor(m => m.ApplicationName)
               .Must(BeWithinApplicationNameMaxLength)
               .WithMessage(FeatureConstants.ApplicationNameLengthValidationMessage)
               .When(x => x.ApplicationName != null);
                RuleFor(m => m.ClaimSetName)
                    .NotEmpty()
                    .WithMessage(FeatureConstants.ClaimSetNameValidationMessage);

                RuleFor(m => m.EducationOrganizationIds)
                    .NotEmpty()
                    .WithMessage(FeatureConstants.EdOrgIdsValidationMessage);

                RuleFor(m => m.VendorId).Must(id => id > 0).WithMessage(FeatureConstants.VendorIdValidationMessage);
            }

            private bool BeWithinApplicationNameMaxLength<T>(Request model, string? applicationName, ValidationContext<T> context)
            {
                var extraCharactersInName = applicationName!.Length - ValidationConstants.MaximumApplicationNameLength;
                if (extraCharactersInName <= 0)
                {
                    return true;
                }
                context.MessageFormatter.AppendArgument("ApplicationName", applicationName);
                context.MessageFormatter.AppendArgument("ExtraCharactersInName", extraCharactersInName);
                return false;
            }
        }
    }
}
