// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Security.DataAccess.Contexts;
using Swashbuckle.AspNetCore.Annotations;
using EdFi.Ods.Admin.Api.Infrastructure.Documentation;

namespace EdFi.Ods.Admin.Api.Features.ClaimSets
{
    [SwaggerSchema(Title = "ClaimSet")]
    public class ClaimSetModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsSystemReserved { get; set; }
        public int ApplicationsCount { get; set; }
    }

    [SwaggerSchema(Title = "ClaimSetWithResources")]
    public class ClaimSetDetailsModel : ClaimSetModel
    {
        public List<ResourceClaimModel>? ResourceClaims { get; set; }
    }

    [SwaggerSchema(Title = "ResourceClaim")]
    public class ResourceClaimModel
    {
        public string? Name { get; set; }
        public bool Read { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public AuthorizationStrategyModel[]? DefaultAuthStrategiesForCRUD { get; set; }
        public AuthorizationStrategyModel[]? AuthStrategyOverridesForCRUD { get; set; }

        [SwaggerSchema(Description = "Children are collection of ResourceClaim")]
        public List<ResourceClaimModel>? Children { get; set; }
        public ResourceClaimModel()
        {
            Children = new List<ResourceClaimModel>();
        }
    }

    [SwaggerSchema(Title = "AuthorizationStrategy")]
    public class AuthorizationStrategyModel
    {
        [SwaggerExclude]
        public int AuthStrategyId { get; set; }

        public string? AuthStrategyName { get; set; }

        [SwaggerExclude]
        public string? DisplayName { get;set; }

        public bool IsInheritedFromParent { get; set; }
    }

    public interface IAddClaimSetModel
    {
        string? Name { get; set; }
        List<ResourceClaimModel>? ResourceClaims { set; get; }
    }

    public interface IEditClaimSetAndResourcesModel
    {
        int Id { get; set; }
        string? Name { get; set; }
        List<ResourceClaimModel>? ResourceClaims { get; set; }
    }

    public class EditClaimSetModel : IEditClaimSetModel
    {
        public string? ClaimSetName { get; set; }

        public int ClaimSetId { get; set; }
    }

    public class UpdateResourcesOnClaimSetModel : IUpdateResourcesOnClaimSetModel
    {
        public int ClaimSetId { get; set; }

        public List<ResourceClaim>? ResourceClaims { get; set; }
    }

    public class DeleteClaimSetModel : IDeleteClaimSetModel
    {
        public string? Name { get; set; }

        public int Id { get; set; }
    }

    public static class ResourceClaimsModelExtensions
    {
        public static void ResolveAuthStrategies(this List<ResourceClaimModel> resourceClaims, ISecurityContext securityContext)
        {
            var dbAuthStrategies = securityContext.AuthorizationStrategies;

            foreach (var claim in resourceClaims)
            {
                if (claim.DefaultAuthStrategiesForCRUD != null && claim.DefaultAuthStrategiesForCRUD.Any())
                {
                    foreach (var defaultAS in claim.DefaultAuthStrategiesForCRUD.Where(x => x != null))
                    {
                        var authStrategy = dbAuthStrategies.SingleOrDefault(x => x.AuthorizationStrategyName.Equals(defaultAS.AuthStrategyName, StringComparison.InvariantCultureIgnoreCase));
                        if (authStrategy != null)
                        {
                            defaultAS.AuthStrategyId = authStrategy.AuthorizationStrategyId;
                            defaultAS.DisplayName = authStrategy.DisplayName;
                        }
                    }
                }
                if (claim.AuthStrategyOverridesForCRUD != null && claim.AuthStrategyOverridesForCRUD.Any())
                {
                    foreach (var authStrategyOverride in claim.AuthStrategyOverridesForCRUD.Where(x => x != null))
                    {
                        var authStrategy = dbAuthStrategies.SingleOrDefault(x => x.AuthorizationStrategyName.Equals(authStrategyOverride.AuthStrategyName, StringComparison.InvariantCultureIgnoreCase));
                        if (authStrategy != null)
                        {
                            authStrategyOverride.AuthStrategyId = authStrategy.AuthorizationStrategyId;
                            authStrategyOverride.DisplayName = authStrategy.DisplayName;
                        }
                    }
                }
                if (claim.Children != null && claim.Children.Any())
                {
                    ResolveAuthStrategies(claim.Children, securityContext);
                }
            }
        }
    }
}
