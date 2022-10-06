// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets
{
    public class ResourceClaimValidator
    {
        public static void Validate<T>(IQueryable<Security.DataAccess.Models.ResourceClaim> dbResourceClaims,
                IQueryable<Security.DataAccess.Models.AuthorizationStrategy> dbAuthStrategies, ResourceClaimModel resourceClaim, List<ResourceClaimModel> existingResourceClaims,
                ValidationContext<T> context, string? claimSetName)
        {
            context.MessageFormatter.AppendArgument("ClaimSetName", claimSetName);
            context.MessageFormatter.AppendArgument("ResourceClaimName", resourceClaim.Name);
            var propertyName = "ResourceClaims";

            if (existingResourceClaims.Count(x => x.Name == resourceClaim.Name) > 1 )
            {
                context.AddFailure(propertyName, FeatureConstants.ClaimSetDuplicateResourceMessage);
            }

            if(!(resourceClaim.Create || resourceClaim.Delete || resourceClaim.Read || resourceClaim.Update))
            {
                context.AddFailure(propertyName, FeatureConstants.ClaimSetResourceClaimWithNoActionMessage);
            }
            var resource = dbResourceClaims.SingleOrDefault(x => x.ResourceName.Equals(resourceClaim.Name, StringComparison.InvariantCultureIgnoreCase));
            if (resource == null)
            {
                context.AddFailure(propertyName, FeatureConstants.ClaimSetResourceNotFoundMessage);
            } 
            if (resourceClaim.DefaultAuthStrategiesForCRUD != null && resourceClaim.DefaultAuthStrategiesForCRUD.Any())
            {
                foreach (var defaultAS in resourceClaim.DefaultAuthStrategiesForCRUD.Where(x => x != null))
                {
                    if (!dbAuthStrategies.Any(x => x.AuthorizationStrategyName.Equals(defaultAS.AuthStrategyName,StringComparison.InvariantCultureIgnoreCase)))
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
                    if (!dbAuthStrategies.Any(x => x.AuthorizationStrategyName.Equals(authStrategyOverride.AuthStrategyName, StringComparison.InvariantCultureIgnoreCase)))
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
                    var childResource = dbResourceClaims.FirstOrDefault(x => x.ResourceName.Equals(child.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (childResource != null)
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
