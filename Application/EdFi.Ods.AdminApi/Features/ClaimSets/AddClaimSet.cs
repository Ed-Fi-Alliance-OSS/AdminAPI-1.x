// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Features.Applications;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.ClaimSets;

public class AddClaimSet : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/claimSets", Handle)
        .WithDefaultDescription()
        .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(201))
        .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, AddClaimSetCommand addClaimSetCommand,
        AddOrEditResourcesOnClaimSetCommand addOrEditResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IGetApplicationsByClaimSetIdQuery getApplications,
        IAuthStrategyResolver strategyResolver,
        IMapper mapper,
        AddClaimSetRequest request)
    {
        await validator.GuardAsync(request);
        var addedClaimSetId = addClaimSetCommand.Execute(new AddClaimSetModel
        {
            ClaimSetName = request.Name ?? string.Empty
        });

        var resourceClaims = mapper.Map<List<ResourceClaim>>(request.ResourceClaims);
        var resolvedResourceClaims = strategyResolver.ResolveAuthStrategies(resourceClaims).ToList();

        addOrEditResourcesOnClaimSetCommand.Execute(addedClaimSetId, resolvedResourceClaims);

        var claimSet = getClaimSetByIdQuery.Execute(addedClaimSetId);

        var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
        var applications = getApplications.Execute(addedClaimSetId);
        if (applications != null)
        {
            model.Applications = mapper.Map<List<SimpleApplicationModel>>(applications);
        }
        model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(addedClaimSetId)
            .Select(r => mapper.Map<ClaimSetResourceClaimModel>(r)).ToList();

        return Results.Created($"/claimSets/{addedClaimSetId}", model);
    }

    [SwaggerSchema(Title = "AddClaimSetRequest")]
    public class AddClaimSetRequest
    {
        [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
        public string? Name { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ResourceClaimsDescription, Nullable = false)]
        public List<ClaimSetResourceClaimModel>? ResourceClaims { get; set; }
    }

    public class Validator : AbstractValidator<AddClaimSetRequest>
    {
        private readonly IGetAllClaimSetsQuery _getAllClaimSetsQuery;

        public Validator(IGetAllClaimSetsQuery getAllClaimSetsQuery,
            IGetResourceClaimsAsFlatListQuery getResourceClaimsAsFlatListQuery,
            IGetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery)
        {
            _getAllClaimSetsQuery = getAllClaimSetsQuery;

            var resourceClaims = (Lookup<string, ResourceClaim>)getResourceClaimsAsFlatListQuery.Execute()
                .ToLookup(rc => rc.Name?.ToLower());

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
