// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using FluentValidation;
using FluentValidation.Results;
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
            IAuthStrategyResolver strategyResolver,
            IMapper mapper,
            Request request, int id)
        {
            request.Id = id;
            await validator.GuardAsync(request);

            var editClaimSetModel = new EditClaimSetModel
            {
                ClaimSetName = request.Name,
                ClaimSetId = id
            };

            int updatedClaimSetId;
            try
            {
                updatedClaimSetId = editClaimSetCommand.Execute(editClaimSetModel);
            }
            catch (AdminAppException exception)
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(id), exception.Message) });
            }

            var resourceClaims = mapper.Map<List<ResourceClaim>>(request.ResourceClaims);
            var resolvedResourceClaims = strategyResolver.ResolveAuthStrategies(resourceClaims).ToList();

            updateResourcesOnClaimSetCommand.Execute(
                new UpdateResourcesOnClaimSetModel { ClaimSetId = updatedClaimSetId, ResourceClaims = resolvedResourceClaims });

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
            private readonly IGetClaimSetByIdQuery _getClaimSetByIdQuery;
            private readonly GetAllClaimSetsQuery _getAllClaimSetsQuery;

            public Validator(IGetClaimSetByIdQuery getClaimSetByIdQuery,
                GetAllClaimSetsQuery getAllClaimSetsQuery,
                GetResourceClaimsAsFlatListQuery getResourceClaimsAsFlatListQuery,
                GetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery)
            {
                _getClaimSetByIdQuery = getClaimSetByIdQuery;
                _getAllClaimSetsQuery = getAllClaimSetsQuery;

                var resourceClaims = (Lookup<string, ResourceClaim>)getResourceClaimsAsFlatListQuery.Execute()
                    .ToLookup(rc => rc.Name.ToLower());

                var authStrategyNames = getAllAuthorizationStrategiesQuery.Execute()
                    .Select(a => a.AuthStrategyName).ToList();

                RuleFor(m => m.Id).NotEmpty();

                RuleFor(m => m.Id)
                    .Must(BeAnExistingClaimSet)
                    .WithMessage(FeatureConstants.ClaimSetNotFound);

                RuleFor(m => m.Name)
                .NotEmpty()
                .Must(BeAUniqueName)
                .WithMessage(FeatureConstants.ClaimSetAlreadyExistsMessage)
                .When(m => BeAnExistingClaimSet(m.Id) && NameIsChanged(m));

                RuleFor(m => m.Name)
                    .MaximumLength(255)
                    .WithMessage(FeatureConstants.ClaimSetNameMaxLengthMessage);

                RuleFor(m => m).Custom((claimSet, context) =>
                {
                    var resourceClaimValidator = new ResourceClaimValidator();

                    if (claimSet.ResourceClaims != null && claimSet.ResourceClaims.Any())
                    {
                        foreach (var resourceClaim in claimSet.ResourceClaims)
                        {
                            resourceClaimValidator.Validate(resourceClaims, authStrategyNames,
                                resourceClaim, claimSet.ResourceClaims, context, claimSet.Name);
                        }
                    }
                });
            }

            private bool BeAnExistingClaimSet(int id)
            {
                try
                {
                    _getClaimSetByIdQuery.Execute(id);
                    return true;
                }
                catch (AdminAppException)
                {
                    return false;
                }
            }

            private bool NameIsChanged(Request model)
            {
                return _getClaimSetByIdQuery.Execute(model.Id).Name != model.Name;
            }

            private bool BeAUniqueName(string? name)
            {
                return _getAllClaimSetsQuery.Execute().All(x => x.ClaimSetName != name);
            }
        }
    }
}
