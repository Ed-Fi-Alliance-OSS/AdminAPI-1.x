// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.Admin.Api.Infrastructure.ClaimSetEditor;
using FluentValidation;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets;

public class ResourceClaimValidator
{
    private static List<string>? _duplicateResources;
    public ResourceClaimValidator()
    {
        _duplicateResources = new List<string>();
    }

    public void Validate<T>(Lookup<string, ResourceClaim> dbResourceClaims,
        List<string> dbAuthStrategies, ResourceClaimModel resourceClaim, List<ResourceClaimModel> existingResourceClaims,
        ValidationContext<T> context, string? claimSetName)
    {
        context.MessageFormatter.AppendArgument("ClaimSetName", claimSetName);
        context.MessageFormatter.AppendArgument("ResourceClaimName", resourceClaim.Name);

        var propertyName = "ResourceClaims";

        if (existingResourceClaims.Count(x => x.Name == resourceClaim.Name) > 1)
        {
            if (_duplicateResources != null && resourceClaim.Name != null && !_duplicateResources.Contains(resourceClaim.Name))
            {
                _duplicateResources.Add(resourceClaim.Name);
                context.AddFailure(propertyName, "Only unique resource claims can be added. The following is a duplicate resource: '{ResourceClaimName}'");
            }
        }

        if (!(resourceClaim.Create || resourceClaim.Delete || resourceClaim.Read || resourceClaim.Update))
        {
            context.AddFailure(propertyName, "Only valid resources can be added. A resource must have at least one action associated with it to be added. The following is an invalid resource: '{ResourceClaimName}'");
        }

        var resources = dbResourceClaims[resourceClaim.Name!.ToLower()].ToList();
        if (!resources.Any())
        {
            context.AddFailure(propertyName, "This Claim Set contains a resource which is not in the system. Claimset Name: '{ClaimSetName}' Resource name: '{ResourceClaimName}'.\n");
        }
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
                        else if (!resources.Select(x => x.Id).Contains(childResource.ParentId))
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
}
