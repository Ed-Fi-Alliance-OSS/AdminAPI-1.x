// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.Admin.Api.Infrastructure;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets
{
    public class AddClaimSet : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiEndpointBuilder.MapPost(endpoints, "/claimsets", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(201))
            .BuildForVersions(AdminApiVersions.V1);
        }

        public async Task<IResult> Handle(Validator validator, AddClaimSetCommand addClaimSetCommand,
            AddOrEditResourcesOnClaimSetCommand addOrEditResourcesOnClaimSetCommand,
            IGetClaimSetByIdQuery getClaimSetByIdQuery,
            IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
            IGetApplicationsByClaimSetIdQuery getApplications,
            IAuthStrategyResolver strategyResolver,
            IMapper mapper,
            Request request)
        {
            await validator.GuardAsync(request);
            var addedClaimSetId = addClaimSetCommand.Execute(new AddClaimSetModel
            {
                ClaimSetName = request.Name
            });

            var resourceClaims = mapper.Map<List<ResourceClaim>>(request.ResourceClaims);
            var resolvedResourceClaims = strategyResolver.ResolveAuthStrategies(resourceClaims).ToList();

            addOrEditResourcesOnClaimSetCommand.Execute(addedClaimSetId, resolvedResourceClaims);

            var claimSet = getClaimSetByIdQuery.Execute(addedClaimSetId);

            var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
            model.ApplicationsCount = getApplications.ExecuteCount(addedClaimSetId);
            model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(addedClaimSetId)
                .Select(r => mapper.Map<ResourceClaimModel>(r)).ToList();

            return AdminApiResponse<ClaimSetDetailsModel>.Created(model, "ClaimSet", $"/claimsets/{addedClaimSetId}");
        }

        [SwaggerSchema(Title = "AddClaimSetRequest")]
        public class Request : IAddClaimSetModel
        {
            [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
            public string? Name { get; set; }

            [SwaggerSchema(Description = FeatureConstants.ResourceClaimsDescription, Nullable = false)]
            public List<ResourceClaimModel>? ResourceClaims { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            private GetAllClaimSetsQuery _getAllClaimSetsQuery;

            public Validator(GetAllClaimSetsQuery getAllClaimSetsQuery,
                GetResourceClaimsAsFlatListQuery getResourceClaimsAsFlatListQuery,
                GetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery)
            {
                _getAllClaimSetsQuery = getAllClaimSetsQuery;

                var resourceClaims = (Lookup<string, ResourceClaim>)getResourceClaimsAsFlatListQuery.Execute()
                    .ToLookup(rc => rc.Name.ToLower());

                var authStrategyNames = getAllAuthorizationStrategiesQuery.Execute()
                    .Select(a => a.AuthStrategyName).ToList();

                RuleFor(m => m.Name).NotEmpty()
                    .Must(BeAUniqueName)
                    .WithMessage(FeatureConstants.ClaimSetAlreadyExistsMessage);

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

            private bool BeAUniqueName(string? name)
            {
                return _getAllClaimSetsQuery.Execute().All(x => x.Name != name);
            }
        }
    }
}
