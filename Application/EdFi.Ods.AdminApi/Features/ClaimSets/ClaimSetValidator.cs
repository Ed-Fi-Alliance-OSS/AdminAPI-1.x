// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using FluentValidation;

namespace EdFi.Ods.AdminApi.Features.ClaimSets;

public class AddClaimSetValidator : AbstractValidator<AddClaimSetRequest>
{
    private readonly IGetAllClaimSetsQuery _getAllClaimSetsQuery;

    public AddClaimSetValidator(IGetAllClaimSetsQuery getAllClaimSetsQuery,
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

public class EditClaimSetValidator : AbstractValidator<EditClaimSetRequest>
{
    private readonly IGetClaimSetByIdQuery _getClaimSetByIdQuery;
    private readonly IGetAllClaimSetsQuery _getAllClaimSetsQuery;

    public EditClaimSetValidator(IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetAllClaimSetsQuery getAllClaimSetsQuery,
        IGetResourceClaimsAsFlatListQuery getResourceClaimsAsFlatListQuery,
        IGetAllAuthorizationStrategiesQuery getAllAuthorizationStrategiesQuery)
    {
        _getClaimSetByIdQuery = getClaimSetByIdQuery;
        _getAllClaimSetsQuery = getAllClaimSetsQuery;

        var resourceClaims = (Lookup<string, ResourceClaim>)getResourceClaimsAsFlatListQuery.Execute()
            .ToLookup(rc => rc.Name?.ToLower());

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
        catch (AdminApiException)
        {
            throw new NotFoundException<int>("claimSet", id);
        }
    }

    private bool NameIsChanged(EditClaimSetRequest model)
    {
        return _getClaimSetByIdQuery.Execute(model.Id).Name != model.Name;
    }

    private bool BeAUniqueName(string? name)
    {
        return _getAllClaimSetsQuery.Execute().All(x => x.Name != name);
    }
}
