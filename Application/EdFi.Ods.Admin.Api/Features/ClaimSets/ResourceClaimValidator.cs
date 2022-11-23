// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets
{
    public class ResourceClaimValidator
    {
        private static List<string>? _duplicateResources;
        public ResourceClaimValidator()
        {
            _duplicateResources = new List<string>();
        }

        public void Validate<T>(IQueryable<Security.DataAccess.Models.ResourceClaim> dbResourceClaims,
                IQueryable<Security.DataAccess.Models.AuthorizationStrategy> dbAuthStrategies, ResourceClaimModel resourceClaim, List<ResourceClaimModel> existingResourceClaims,
                ValidationContext<T> context, string? claimSetName)
        {
            context.MessageFormatter.AppendArgument("ClaimSetName", claimSetName);
            context.MessageFormatter.AppendArgument("ResourceClaimName", resourceClaim.Name);

            var dbResourceClaimsAsDict = dbResourceClaims.ToDictionary(x => x.ResourceName.ToLower());
            var dbAuthStrategiesAsList = dbAuthStrategies.Select(a => a.AuthorizationStrategyName.ToLower()).ToList();

            var propertyName = "ResourceClaims";

            if (existingResourceClaims.Count(x => x.Name == resourceClaim.Name) > 1 )
            {
                if (_duplicateResources != null && resourceClaim.Name != null && !_duplicateResources.Contains(resourceClaim.Name))
                {
                    _duplicateResources.Add(resourceClaim.Name);
                    context.AddFailure(propertyName, FeatureConstants.ClaimSetDuplicateResourceMessage);
                }
            }

            if(!(resourceClaim.Create || resourceClaim.Delete || resourceClaim.Read || resourceClaim.Update))
            {
                context.AddFailure(propertyName, FeatureConstants.ClaimSetResourceClaimWithNoActionMessage);
            }

            if (!dbResourceClaimsAsDict.TryGetValue(resourceClaim.Name.ToLower(), out var resource))
            {
                context.AddFailure(propertyName, FeatureConstants.ClaimSetResourceNotFoundMessage);
            } 
            if (resourceClaim.DefaultAuthStrategiesForCRUD != null && resourceClaim.DefaultAuthStrategiesForCRUD.Any())
            {
                foreach (var defaultAS in resourceClaim.DefaultAuthStrategiesForCRUD.Where(x => x != null))
                {
                    if (!dbAuthStrategiesAsList.Contains(defaultAS.AuthStrategyName.ToLower()))
                    {
                        context.MessageFormatter.AppendArgument("AuthStrategyName", defaultAS.AuthStrategyName);
                        context.AddFailure(propertyName, FeatureConstants.ClaimSetAuthStrategyNotFoundMessage);
                    }
                }
            }
            if (resourceClaim.AuthStrategyOverridesForCRUD != null && resourceClaim.AuthStrategyOverridesForCRUD.Any())
            {
                foreach (var authStrategyOverride in resourceClaim.AuthStrategyOverridesForCRUD.Where(x => x != null))
                {
                    if (!dbAuthStrategiesAsList.Contains(authStrategyOverride.AuthStrategyName.ToLower()))
                    {
                        context.MessageFormatter.AppendArgument("AuthStrategyName", authStrategyOverride.AuthStrategyName);
                        context.AddFailure(propertyName, FeatureConstants.ClaimSetAuthStrategyNotFoundMessage);
                    }
                }
            }

            if (resourceClaim.Children != null && resourceClaim.Children.Any())
            {
                foreach (var child in resourceClaim.Children)
                {
                    if (dbResourceClaimsAsDict.TryGetValue(child.Name.ToLower(), out var childResource))
                    {
                        context.MessageFormatter.AppendArgument("ChildResource", childResource.ResourceName);
                        if (childResource.ParentResourceClaimId == null)
                        {
                            context.AddFailure(propertyName, FeatureConstants.WrongChildResourceMessage);
                        }
                        else if (childResource.ParentResourceClaimId != resource?.ResourceClaimId)
                        {
                            context.MessageFormatter.AppendArgument("CorrectParentResource", childResource.ParentResourceClaim?.ResourceName);
                            context.AddFailure(propertyName, FeatureConstants.ChildToWrongParentResourceMessage);
                        }
                    }
                    Validate(dbResourceClaims, dbAuthStrategies, child, resourceClaim.Children, context, claimSetName);
                }
            }
        }
    }
}
