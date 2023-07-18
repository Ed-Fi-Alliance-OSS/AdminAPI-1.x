// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using FluentValidation;

namespace EdFi.Ods.AdminApi.Features.ClaimSets;

public class ResourceClaimValidator
{
    private static List<string>? _duplicateResources;
    public ResourceClaimValidator()
    {
        _duplicateResources = new List<string>();
    }

    public void Validate<T>(Lookup<string, ResourceClaim> dbResourceClaims,
        List<string?> dbAuthStrategies, ResourceClaimModel resourceClaim, List<ResourceClaimModel> existingResourceClaims,
        ValidationContext<T> context, string? claimSetName)
    {
        context.MessageFormatter.AppendArgument("ClaimSetName", claimSetName);
        context.MessageFormatter.AppendArgument("ResourceClaimName", resourceClaim.Name);

        var propertyName = "ResourceClaims";
        ValidateDuplicateResourceClaim(resourceClaim, existingResourceClaims, context, propertyName);

        ValidateCRUD(resourceClaim, context, propertyName);

        var resources = dbResourceClaims[resourceClaim.Name!.ToLower()].ToList();
        ValidateIfExist(context, propertyName, resources);
        ValidateAuthStrategies(dbAuthStrategies, resourceClaim, context, propertyName);
        ValidateAuthStrategiesOverride(dbAuthStrategies, resourceClaim, context, propertyName);
        ValidateChildren(dbResourceClaims, dbAuthStrategies, resourceClaim, context, claimSetName, propertyName, resources);
    }

    public void Validate<T>(Lookup<int, ResourceClaim> dbResourceClaims, ResourceClaimActionModel resourceClaim, 
        ValidationContext<T> context, string? claimSetName, int? parentResourceClaimId)
    {
        context.MessageFormatter.AppendArgument("ClaimSetName", claimSetName);
        context.MessageFormatter.AppendArgument("ResourceClaimName", resourceClaim.Id);

        var propertyName = "ResourceClaims";
        var resources = dbResourceClaims[resourceClaim.Id].ToList();
        ValidateIfExist(context, propertyName, resources);
        ValidateIfItIsChild(context, parentResourceClaimId, propertyName, resources);
        ValidateCRUD(resourceClaim, context, propertyName);
    }

    private static void ValidateIfItIsChild<T>(ValidationContext<T> context, int? parentResourceClaimId, string propertyName, List<ResourceClaim> resources)
    {
        var dbResourceClaim = resources.FirstOrDefault();
        if (dbResourceClaim!.IsParent == false)
        {
            if (parentResourceClaimId.GetValueOrDefault() != dbResourceClaim.ParentId)
            {
                context.AddFailure(propertyName, $"The parentResourceClaimId is not valid for the child {dbResourceClaim.Id}. Set the proper value {dbResourceClaim.ParentId}");
            }
        }
    }

    private static void ValidateIfExist<T>(ValidationContext<T> context, string propertyName, List<ResourceClaim> resources)
    {
        if (!resources.Any())
        {
            context.AddFailure(propertyName, "This Claim Set contains a resource which is not in the system. Claimset Name: '{ClaimSetName}' Resource name: '{ResourceClaimName}'.\n");
        }
    }

    private static void ValidateDuplicateResourceClaim<T>(ResourceClaimModel resourceClaim, List<ResourceClaimModel> existingResourceClaims, ValidationContext<T> context, string propertyName)
    {
        if (existingResourceClaims.Count(x => x.Name == resourceClaim.Name) > 1)
        {
            if (_duplicateResources != null && resourceClaim.Name != null && !_duplicateResources.Contains(resourceClaim.Name))
            {
                _duplicateResources.Add(resourceClaim.Name);
                context.AddFailure(propertyName, "Only unique resource claims can be added. The following is a duplicate resource: '{ResourceClaimName}'");
            }
        }
    }

    private void ValidateChildren<T>(Lookup<string, ResourceClaim> dbResourceClaims, List<string?> dbAuthStrategies, ResourceClaimModel resourceClaim, ValidationContext<T> context, string? claimSetName, string propertyName, List<ResourceClaim> resources)
    {
        if (resourceClaim.Children.Any())
        {
            foreach (var child in resourceClaim.Children)
            {
                var childResources = dbResourceClaims[child.Name!.ToLower()].ToList();
                if (childResources.Any())
                {
                    foreach (var childResource in childResources)
                    {
                        context.MessageFormatter.AppendArgument("ChildResource", childResource.Name);
                        if (childResource.ParentId == 0)
                        {
                            context.AddFailure(propertyName, "'{ChildResource}' can not be added as a child resource.");
                        }

                        else if (!resources.Where(x => x is not null).Select(x => x.Id).Contains(childResource.ParentId))
                        {
                            context.MessageFormatter.AppendArgument("CorrectParentResource", childResource.ParentName);
                            context.AddFailure(propertyName, "Child resource: '{ChildResource}' added to the wrong parent resource. Correct parent resource is: '{CorrectParentResource}'");
                        }
                    }
                }
                Validate(dbResourceClaims, dbAuthStrategies, child, resourceClaim.Children, context, claimSetName);
            }
        }
    }

    private static void ValidateAuthStrategiesOverride<T>(List<string?> dbAuthStrategies, ResourceClaimModel resourceClaim, ValidationContext<T> context, string propertyName)
    {
        if (resourceClaim.AuthStrategyOverridesForCRUD.Any())
        {
            foreach (var authStrategyOverride in resourceClaim.AuthStrategyOverridesForCRUD)
            {
                if (authStrategyOverride?.AuthStrategyName != null && !dbAuthStrategies.Contains(authStrategyOverride.AuthStrategyName))
                {
                    context.MessageFormatter.AppendArgument("AuthStrategyName", authStrategyOverride.AuthStrategyName);
                    context.AddFailure(propertyName, "This resource claim contains an authorization strategy which is not in the system. Claimset Name: '{ClaimSetName}' Resource name: '{ResourceClaimName}' Authorization strategy: '{AuthStrategyName}'.\n");
                }
            }
        }
    }

    private static void ValidateAuthStrategies<T>(List<string?> dbAuthStrategies, ResourceClaimModel resourceClaim, ValidationContext<T> context, string propertyName)
    {
        if (resourceClaim.DefaultAuthStrategiesForCRUD.Any())
        {
            foreach (var defaultAS in resourceClaim.DefaultAuthStrategiesForCRUD)
            {
                if (defaultAS?.AuthStrategyName != null && !dbAuthStrategies.Contains(defaultAS.AuthStrategyName))
                {
                    context.MessageFormatter.AppendArgument("AuthStrategyName", defaultAS.AuthStrategyName);
                    context.AddFailure(propertyName, "This resource claim contains an authorization strategy which is not in the system. Claimset Name: '{ClaimSetName}' Resource name: '{ResourceClaimName}' Authorization strategy: '{AuthStrategyName}'.\n");
                }
            }
        }
    }

    private static void ValidateCRUD<T>(ResourceClaimModel resourceClaim, ValidationContext<T> context, string propertyName)
    {
        if (!(resourceClaim.Create || resourceClaim.Delete || resourceClaim.Read || resourceClaim.Update))
        {
            context.AddFailure(propertyName, $"Only valid resources can be added. {FeatureConstants.ResourceClaimOneActionNotSet} The following is an invalid resource: '{{ResourceClaimName}}'");
        }
    }

    private static void ValidateCRUD<T>(ResourceClaimActionModel resourceClaim, ValidationContext<T> context, string propertyName)
    {
        if (!(resourceClaim.Create || resourceClaim.Delete || resourceClaim.Read || resourceClaim.Update))
        {
            context.AddFailure(propertyName, FeatureConstants.ResourceClaimOneActionNotSet);
        }
    }
}
