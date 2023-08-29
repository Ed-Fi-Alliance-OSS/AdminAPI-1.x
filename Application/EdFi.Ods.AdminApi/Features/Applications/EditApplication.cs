// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using FluentValidation;
using FluentValidation.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Applications;

public class EditApplication : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPut(endpoints, "/applications/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ApplicationModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(IEditApplicationCommand editApplicationCommand, IMapper mapper,
        Validator validator, IUsersContext db, Request request, int id)
    {
        request.Id = id;
        await validator.GuardAsync(request);
        GuardAgainstInvalidEntityReferences(request, db);

        var updatedApplication = editApplicationCommand.Execute(request);
        var model = mapper.Map<ApplicationModel>(updatedApplication);
        return Results.Ok();
    }

    private static void GuardAgainstInvalidEntityReferences(Request request, IUsersContext db)
    {
        if (null == db.Vendors.Find(request.VendorId))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.VendorId), $"Vendor with ID {request.VendorId} not found.") });

        ValidateProfileIds(request, db);

        if (null == db.OdsInstances.Find(request.OdsInstanceId))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.OdsInstanceId), $"ODS instance with ID {request.OdsInstanceId} not found.") });
    }

    private static void ValidateProfileIds(Request request, IUsersContext db)
    {
        var allProfileIds = db.Profiles.Select(p => p.ProfileId).ToList();
        if ((request.ProfileIds != null && request.ProfileIds.Count() > 0) && allProfileIds.Count == 0)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.ProfileIds), $"The following ProfileIds were not found in database: {string.Join(", ", request.ProfileIds)}") });
        }
        if ((request.ProfileIds != null && request.ProfileIds.Count() > 0) && (!request.ProfileIds.All(p => allProfileIds.Contains(p))))
        {
            var notExist = request.ProfileIds.Where(p => !allProfileIds.Contains(p));
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.ProfileIds), $"The following ProfileIds were not found in database: {string.Join(", ", notExist)}") });
        }
    }

    [SwaggerSchema(Title = "EditApplicationRequest")]
    public class Request : IEditApplicationModel
    {
        [SwaggerSchema(Description = "Application id", Nullable = false)]
        public int Id { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ApplicationNameDescription, Nullable = false)]
        public string? ApplicationName { get; set; }

        [SwaggerSchema(Description = FeatureConstants.VendorIdDescription, Nullable = false)]
        public int VendorId { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
        public string? ClaimSetName { get; set; }

        [SwaggerOptional]
        [SwaggerSchema(Description = FeatureConstants.ProfileIdDescription)]
        public IEnumerable<int>? ProfileIds { get; set; }

        [SwaggerSchema(Description = FeatureConstants.EducationOrganizationIdsDescription, Nullable = false)]
        public IEnumerable<int>? EducationOrganizationIds { get; set; }

        [SwaggerSchema(Description = FeatureConstants.OdsInstanceIdDescription, Nullable = false)]
        public int OdsInstanceId { get; set; }
    }

    public class Validator : AbstractValidator<IEditApplicationModel>
    {
        public Validator()
        {
            RuleFor(m => m.Id).NotEmpty();
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
            RuleFor(m => m.OdsInstanceId).Must(id => id > 0).WithMessage(FeatureConstants.OdsInstanceIdValidationMessage);

            static bool BeWithinApplicationNameMaxLength<T>(IEditApplicationModel model, string? applicationName, ValidationContext<T> context)
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
