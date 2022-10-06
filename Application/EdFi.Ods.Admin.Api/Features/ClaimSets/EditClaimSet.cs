// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets
{
    public class EditClaimSet : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(200))
            .BuildForVersions(AdminApiVersions.V1);
        }

        public async Task<IResult> Handle(Validator validator, EditClaimSetCommand editClaimSetCommand,
            UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
            IGetClaimSetByIdQuery getClaimSetByIdQuery,
            IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
            IGetApplicationsByClaimSetIdQuery getApplications,
            IMapper mapper, ISecurityContext securityContext,
            Request request, int id)
        {
            request.Id = id;
            await validator.GuardAsync(request);
            var updatedClaimSetId = editClaimSetCommand.Execute(new EditClaimSetModel
            {
                ClaimSetName = request.Name,
                ClaimSetId = id
            });

            request.ResourceClaims?.ResolveAuthStrategies(securityContext);
            updateResourcesOnClaimSetCommand.Execute(new UpdateResourcesOnClaimSetModel
            { ClaimSetId = updatedClaimSetId, ResourceClaims = mapper.Map<List<ResourceClaim>>(request.ResourceClaims) } );

            var claimSet = getClaimSetByIdQuery.Execute(updatedClaimSetId);
            var allResources = getResourcesByClaimSetIdQuery.AllResources(updatedClaimSetId);
            var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
            model.ApplicationsCount = getApplications.ExecuteCount(updatedClaimSetId);
            model.ResourceClaims = mapper.Map<List<ResourceClaimModel>>(allResources.ToList());

            return AdminApiResponse<ClaimSetDetailsModel>.Updated(model, "ClaimSet");
        }

        [SwaggerSchema(Title = "EditClaimSetRequest")]
        public class Request : IEditClaimSetAndResourcesModel
        {
            [SwaggerSchema(Description = "ClaimSet id", Nullable = false)]
            public int Id { get; set; }

            [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
            public string? Name { get; set; }

            [SwaggerSchema(Description = FeatureConstants.ResourceClaimsDescription, Nullable = false)]
            public List<ResourceClaimModel>? ResourceClaims { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            private readonly ISecurityContext _securityContext;
            public Validator(ISecurityContext securityContext)
            {
                _securityContext = securityContext;

                RuleFor(m => m.Id).NotEmpty();

                RuleFor(m => m.Id)
                    .Must(BeAnExistingClaimSet)
                    .WithMessage(FeatureConstants.ClaimSetNotFound);

                RuleFor(m => m.Name)
                .NotEmpty()
                .Must(BeAUniqueName)
                .WithMessage(FeatureConstants.ClaimSetAlreadyExistsMessage)
                .When(NameIsChanged);

                RuleFor(m => m.Name)
                    .MaximumLength(255)
                    .WithMessage(FeatureConstants.ClaimSetNameMaxLengthMessage);

                RuleFor(m => m).Custom((claimSet, context) =>
                {
                    if (claimSet.ResourceClaims != null && claimSet.ResourceClaims.Any())
                    {
                        foreach (var resourceClaim in claimSet.ResourceClaims)
                        {
                            ResourceClaimValidator.Validate(securityContext.ResourceClaims,
                                securityContext.AuthorizationStrategies, resourceClaim, claimSet.ResourceClaims, context, claimSet.Name);
                        }
                    }
                });
            }

            private bool BeAnExistingClaimSet(int id)
            {
                return _securityContext.ClaimSets.SingleOrDefault(x => x.ClaimSetId == id) != null;
            }

            private bool NameIsChanged(Request model)
            {
                return _securityContext.ClaimSets.Single(x => x.ClaimSetId == model.Id).ClaimSetName != model.Name;
            }

            private bool BeAUniqueName(string? name)
            {
                return !_securityContext.ClaimSets.Any(x => x.ClaimSetName == name);
            }
        }
    }
}
