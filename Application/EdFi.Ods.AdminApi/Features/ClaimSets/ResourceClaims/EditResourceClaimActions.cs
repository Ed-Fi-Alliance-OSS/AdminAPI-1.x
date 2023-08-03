// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims;

public class EditResourceClaimActions : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/claimsets/{claimsetid}/resourceclaimActions", HandleAddResourceClaims)
       .WithDefaultDescription()
       .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{claimsetid}/resourceclaimActions/{resourceclaimid}", HandleEditResourceClaims)
       .WithDefaultDescription()
       .WithRouteOptions(b => b.WithResponse<ClaimSetDetailsModel>(200))
       .BuildForVersions(AdminApiVersions.V2);
    }

    internal async Task<IResult> HandleAddResourceClaims(ResourceClaimClaimSetValidator validator,
        EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand,
        UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IMapper mapper,
        AddResourceClaimOnClaimSetRequest request, int claimsetid)
    {
        request.ClaimSetId = claimsetid;
        await validator.GuardAsync(request);
        await ExecuteHandle(editResourcesOnClaimSetCommand, updateResourcesOnClaimSetCommand, getResourcesByClaimSetIdQuery, mapper, request);
        return Results.Ok();
    }

    internal async Task<IResult> HandleEditResourceClaims(ResourceClaimClaimSetValidator validator,
        EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand,
        UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IMapper mapper,
        EditResourceClaimOnClaimSetRequest request, int claimsetid, int resourceclaimid)
    {
        request.ClaimSetId = claimsetid;
        request.ResourceClaimId = resourceclaimid;
        await validator.GuardAsync(request);

        await ExecuteHandle(editResourcesOnClaimSetCommand, updateResourcesOnClaimSetCommand, getResourcesByClaimSetIdQuery, mapper, request);
        var claimSet = getClaimSetByIdQuery.Execute(claimsetid);
        var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
        model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(claimsetid)
            .Select(r => mapper.Map<ClaimSetResourceClaimModel>(r)).ToList();

        return Results.Ok(model);
    }

    private static async Task ExecuteHandle(EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand, UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand, IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery, IMapper mapper, IResourceClaimOnClaimSetRequest request)
    {
        var editResourceOnClaimSetModel = mapper.Map<EditResourceOnClaimSetModel>(request);
        editResourceOnClaimSetModel.ResourceClaim!.Id = request.ResourceClaimId;
        var resourceClaims = await Task.FromResult(getResourcesByClaimSetIdQuery.AllResources(request.ResourceClaimId));
        var parentResourceClaim = resourceClaims.FirstOrDefault(rc => rc.Id == request.ParentResourceClaimId.GetValueOrDefault());
        if (parentResourceClaim != null)
        {
            if (!parentResourceClaim.Children.Any(c => c.Id == editResourceOnClaimSetModel.ResourceClaim!.Id))
            {
                foreach (var rc in resourceClaims)
                {
                    if (rc.Id == editResourceOnClaimSetModel.ResourceClaim!.Id)
                    {
                        rc.Children.Add(editResourceOnClaimSetModel.ResourceClaim!);
                    }
                }
                updateResourcesOnClaimSetCommand.Execute(
                    new UpdateResourcesOnClaimSetModel
                    {
                        ClaimSetId = request.ClaimSetId,
                        ResourceClaims = mapper.Map<List<ResourceClaim>>(resourceClaims)
                    });
            }

        }

        editResourcesOnClaimSetCommand.Execute(editResourceOnClaimSetModel);
    }


    [SwaggerSchema(Title = "AddResourceClaimActionsOnClaimSetRequest")]
    public class AddResourceClaimOnClaimSetRequest : IResourceClaimOnClaimSetRequest
    {
        [SwaggerExclude]
        public int ClaimSetId { get; set; }

        [SwaggerSchema(Description = "ResourceClaim id", Nullable = false)]
        public int ResourceClaimId { get; set; }

        [SwaggerSchema(Description = "Parent ResourceClaim id", Nullable = true)]
        public int? ParentResourceClaimId { get; set; }

        [SwaggerSchema(Nullable = false)]
        public ResourceClaimActionModel ResourceClaimActions { get; set; } = new ResourceClaimActionModel();
    }

    [SwaggerSchema(Title = "EditResourceClaimActionsOnClaimSetRequest")]
    public class EditResourceClaimOnClaimSetRequest : IResourceClaimOnClaimSetRequest
    {
        [SwaggerExclude]
        public int ClaimSetId { get; set; }

        [SwaggerExclude]
        public int ResourceClaimId { get; set; }

        [SwaggerSchema(Description = "Parent ResourceClaim id", Nullable = true)]
        public int? ParentResourceClaimId { get; set; }

        [SwaggerSchema(Nullable = false)]
        public ResourceClaimActionModel ResourceClaimActions { get; set; } = new ResourceClaimActionModel();
    }

    public class ResourceClaimClaimSetValidator : AbstractValidator<IResourceClaimOnClaimSetRequest>
    {
        private readonly IGetClaimSetByIdQuery _getClaimSetByIdQuery;
        private ClaimSet? _claimSet;

        public ResourceClaimClaimSetValidator(IGetClaimSetByIdQuery getClaimSetByIdQuery,
            IGetResourceClaimsAsFlatListQuery getResourceClaimsAsFlatListQuery)
        {
            _getClaimSetByIdQuery = getClaimSetByIdQuery;

            var resourceClaims = getResourceClaimsAsFlatListQuery.Execute();
            var resourceClaimsById = (Lookup<int, ResourceClaim>)resourceClaims
                .ToLookup(rc => rc.Id);

            RuleFor(m => m.ClaimSetId).NotEmpty();

            RuleFor(m => m.ClaimSetId)
                .Must(BeAnExistingClaimSet)
                .WithMessage(FeatureConstants.ClaimSetNotFound);

            RuleFor(m => m).Custom((resourceClaimOnClaimSetRequest, context) =>
            {
                var resourceClaimValidator = new ResourceClaimValidator();

                if (resourceClaimOnClaimSetRequest.ResourceClaimActions != null)
                {
                    resourceClaimValidator.Validate(resourceClaimsById, resourceClaimOnClaimSetRequest, context, _claimSet!.Name);
                }
                else
                {
                    context.AddFailure(FeatureConstants.ResourceClaimNotFound);
                }
            });
        }

        private bool BeAnExistingClaimSet(int id)
        {
            try
            {
                _claimSet = _getClaimSetByIdQuery.Execute(id);
                return true;
            }
            catch (AdminApiException)
            {
                throw new NotFoundException<int>("claimSet", id);
            }
        }
    }


}


