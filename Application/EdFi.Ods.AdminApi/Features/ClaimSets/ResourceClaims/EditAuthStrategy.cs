// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FluentValidation;
using FluentValidation.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims;

public class EditAuthStrategy : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}/overrideAuthorizationStrategy", HandleOverrideAuthStrategies)
       .WithDefaultDescription()
       .WithRouteOptions(b => b.WithResponseCode(201))
       .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapPost(endpoints, "/claimSets/{claimSetId}/resourceClaimActions/{resourceClaimId}/resetAuthorizationStrategies", HandleResetAuthStrategies)
        .WithDefaultDescription()
        .WithRouteOptions(b => b.WithResponseCode(201))
        .BuildForVersions(AdminApiVersions.V2);
    }

    internal async Task<IResult> HandleOverrideAuthStrategies(OverrideAuthStategyOnClaimSetValidator validator,
      OverrideDefaultAuthorizationStrategyCommand overrideDefaultAuthorizationStrategyCommand, IMapper mapper,
      OverrideAuthStategyOnClaimSetRequest request, int claimSetId, int resourceClaimId)
    {
        request.ClaimSetId = claimSetId;
        request.ResourceClaimId = resourceClaimId;
        await validator.GuardAsync(request);
        var model = mapper.Map<OverrideAuthStrategyOnClaimSetModel>(request);
        overrideDefaultAuthorizationStrategyCommand.ExecuteOnSpecificAction(model);

        return Results.Ok();
    }

    internal async Task<IResult> HandleResetAuthStrategies(IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        OverrideDefaultAuthorizationStrategyCommand overrideDefaultAuthorizationStrategyCommand, IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IMapper mapper, int claimSetId, int resourceClaimId)
    {
        var claimSet = getClaimSetByIdQuery.Execute(claimSetId);

        if (!claimSet.IsEditable)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(claimSetId), $"Claim set ({claimSet.Name}) is system reserved. May not be modified.") });
        }

        var resourceClaims = getResourcesByClaimSetIdQuery.AllResources(claimSetId);
        if (!resourceClaims.Any(rc => rc.Id == resourceClaimId))
        {
            throw new NotFoundException<int>("ResourceClaim", resourceClaimId);
        }
        else
        {
            overrideDefaultAuthorizationStrategyCommand.ResetAuthorizationStrategyOverrides(
                new OverrideAuthStrategyOnClaimSetModel()
                {
                    ClaimSetId = claimSetId,
                    ResourceClaimId = resourceClaimId
                });
        }

        return await Task.FromResult(Results.Ok());
    }


    public class OverrideAuthStategyOnClaimSetValidator : AbstractValidator<OverrideAuthStategyOnClaimSetRequest>
    {
        public OverrideAuthStategyOnClaimSetValidator(IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery, IGetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery, IGetAllActionsQuery getAllActionsQuery, IGetClaimSetByIdQuery getClaimSetByIdQuery)
        {
            RuleFor(m => m.ClaimSetId).NotEqual(0);
            RuleFor(m => m.ResourceClaimId).NotEqual(0);
            RuleFor(m => m.ActionName).NotEmpty();
            RuleFor(m => m.AuthorizationStrategyName).NotEmpty();

            RuleFor(m => m).Custom((overrideAuthStategyOnClaimSetRequest, context) =>
            {

                var resoureClaim = getResourcesByClaimSetIdQuery.SingleResource(overrideAuthStategyOnClaimSetRequest.ClaimSetId, overrideAuthStategyOnClaimSetRequest.ResourceClaimId);
                if (resoureClaim == null)
                {
                    context.AddFailure("ResourceClaim", "Resource claim doesn't exist for the Claim set provided");
                }

                var claimSet = getClaimSetByIdQuery.Execute(overrideAuthStategyOnClaimSetRequest.ClaimSetId);
                if (!claimSet.IsEditable)
                {
                    context.AddFailure("ClaimSetId", $"Claim set ({claimSet.Name}) is system reserved. May not be modified.");
                }

                var authStrategyName = getAllAuthorizationStrategiesQuery.Execute().ToList()
                .FirstOrDefault(a => a.AuthStrategyName!.ToLower() == overrideAuthStategyOnClaimSetRequest.AuthorizationStrategyName!.ToLower());

                if (authStrategyName == null)
                {
                    context.AddFailure("AuthorizationStrategyName", "AuthorizationStrategyName doesn't exist.");
                }

                var actionName = getAllActionsQuery.Execute().ToList()
                .FirstOrDefault(a => a.ActionName.ToLower() == overrideAuthStategyOnClaimSetRequest.ActionName!.ToLower());

                if (actionName == null)
                {
                    context.AddFailure("ActionName", "ActionName doesn't exist.");
                }
            });
        }
    }


    [SwaggerSchema(Title = "OverrideAuthStategyOnClaimSetRequest")]
    public class OverrideAuthStategyOnClaimSetRequest : OverrideAuthStrategyOnClaimSetModel
    {
        [SwaggerSchema(Description = "AuthorizationStrategy name", Nullable = false)]
        public string? AuthorizationStrategyName { get; set; }
    }
}
