// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.Admin.Api.Infrastructure.Documentation;
using EdFi.Ods.Admin.Api.Infrastructure.Database.Commands;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using FluentValidation.Results;
using EdFi.Ods.Admin.Api.Infrastructure.Commands;

namespace EdFi.Ods.Admin.Api.Features.Applications;

public class AddApplication : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/applications", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ApplicationResult>(201))
            .BuildForVersions(AdminApiVersions.V1);
    }

    public async Task<IResult> Handle(Validator validator, IAddApplicationCommand addApplicationCommand, IMapper mapper, IUsersContext db, Request request)
    {
        await validator.GuardAsync(request);
        GuardAgainstInvalidEntityReferences(request, db);
        var addedApplicationResult = addApplicationCommand.Execute(request);
        var model = mapper.Map<ApplicationResult>(addedApplicationResult);
        return AdminApiResponse<ApplicationResult>.Created(model, "Application", $"/applications/{model.ApplicationId}");
    }

    private void GuardAgainstInvalidEntityReferences(Request request, IUsersContext db)
    {
        if (null == db.Vendors.Find(request.VendorId))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.VendorId), $"Vendor with ID {request.VendorId} not found.") });

        if (request.ProfileId.HasValue && db.Profiles.Find(request.ProfileId) == null)
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.ProfileId), $"Profile with ID {request.ProfileId} not found.") });
    }

    [SwaggerSchema(Title = "AddApplicationRequest")]
    public class Request : IAddApplicationModel
    {
        [SwaggerSchema(Description = FeatureConstants.ApplicationNameDescription, Nullable = false)]
        public string? ApplicationName { get; set; }

        [SwaggerSchema(Description = FeatureConstants.VedorIdDescription, Nullable = false)]
        public int VendorId { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
        public string? ClaimSetName { get; set; }

        [SwaggerOptional]
        [SwaggerSchema(Description = FeatureConstants.ProfileIdDescription)]
        public int? ProfileId { get; set; }

        [SwaggerSchema(Description = FeatureConstants.EducationOrganizationIdsDescription, Nullable = false)]
        public IEnumerable<int>? EducationOrganizationIds { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.ApplicationName)
             .NotEmpty();

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

        private bool BeWithinApplicationNameMaxLength<T>(IAddApplicationModel model, string? applicationName, ValidationContext<T> context)
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
