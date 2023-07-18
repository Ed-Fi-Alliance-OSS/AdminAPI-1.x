// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using Swashbuckle.AspNetCore.Annotations;
using EdFi.Ods.AdminApi.Infrastructure.Documentation;
using EdFi.Ods.AdminApi.Features.Applications;

namespace EdFi.Ods.AdminApi.Features.ClaimSets;

[SwaggerSchema(Title = "ClaimSet")]
public class ClaimSetModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsSystemReserved { get; set; }
    public List<SimpleApplicationModel> Applications { get; set; } = new();
}

[SwaggerSchema(Title = "ClaimSetWithResources")]
public class ClaimSetDetailsModel : ClaimSetModel
{
    public List<ResourceClaimModel> ResourceClaims { get; set; } = new();
}

[SwaggerSchema(Title = "ResourceClaim")]
public class ResourceClaimModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool Read { get; set; }
    public bool Create { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }
    public AuthorizationStrategyModel?[] DefaultAuthStrategiesForCRUD { get; set; }
    public AuthorizationStrategyModel?[] AuthStrategyOverridesForCRUD { get; set; }

    [SwaggerSchema(Description = "Children are collection of ResourceClaim")]
    public List<ResourceClaimModel> Children { get; set; }
    public ResourceClaimModel()
    {
        Children = new List<ResourceClaimModel>();
        DefaultAuthStrategiesForCRUD = Array.Empty<AuthorizationStrategyModel>();
        AuthStrategyOverridesForCRUD = Array.Empty<AuthorizationStrategyModel>();
    }
}

[SwaggerSchema(Title = "ResourceClaimActionModel")]
public class ResourceClaimActionModel
{
    public int Id { get; set; }
    public bool Read { get; set; }
    public bool Create { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }
}

[SwaggerSchema(Title = "SimpleResourceClaimModel")]
public class SimpleResourceClaimModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }

    [SwaggerSchema(Description = "Children are collection of SimpleResourceClaimModel")]
    public List<SimpleResourceClaimModel> Children { get; set; }
    public SimpleResourceClaimModel()
    {
        Children = new List<SimpleResourceClaimModel>();
    }
}

[SwaggerSchema(Title = "AuthorizationStrategy")]
public class AuthorizationStrategyModel
{
    [SwaggerExclude]
    public int AuthStrategyId { get; set; }

    public string? AuthStrategyName { get; set; }

    [SwaggerExclude]
    public string? DisplayName { get; set; }

    public bool IsInheritedFromParent { get; set; }
}

public class EditClaimSetModel : IEditClaimSetModel
{
    public string? ClaimSetName { get; set; }

    public int ClaimSetId { get; set; }
}

public class UpdateResourcesOnClaimSetModel : IUpdateResourcesOnClaimSetModel
{
    public int ClaimSetId { get; set; }

    public List<ResourceClaim>? ResourceClaims { get; set; } = new List<ResourceClaim>();
}

public class DeleteClaimSetModel : IDeleteClaimSetModel
{
    public string? Name { get; set; }

    public int Id { get; set; }
}

#region Requests
[SwaggerSchema(Title = "AddClaimSetRequest")]
public class AddClaimSetRequest
{
    [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
    public string? Name { get; set; }

    [SwaggerSchema(Description = FeatureConstants.ResourceClaimsDescription, Nullable = false)]
    public List<ResourceClaimModel>? ResourceClaims { get; set; }
}

[SwaggerSchema(Title = "EditClaimSetRequest")]
public class EditClaimSetRequest
{
    [SwaggerSchema(Description = "ClaimSet id", Nullable = false)]
    public int Id { get; set; }

    [SwaggerSchema(Description = FeatureConstants.ClaimSetNameDescription, Nullable = false)]
    public string? Name { get; set; }

    [SwaggerSchema(Description = FeatureConstants.ResourceClaimsDescription, Nullable = false)]
    public List<ResourceClaimModel>? ResourceClaims { get; set; }
}

[SwaggerSchema(Title = "EditResourceClaimOnClaimSetRequest")]
public class EditResourceClaimOnClaimSetRequest
{
    [SwaggerSchema(Description = "ClaimSet id", Nullable = false)]
    public int Id { get; set; }

    [SwaggerSchema(Description = "Parent ResourceClaim id", Nullable = true)]
    public int? ParentResourceClaimId { get; set; }

    [SwaggerSchema(Nullable = false)]
    public ResourceClaimActionModel ResourceClaim { get; set; }
}

[SwaggerSchema(Title = "OverrideAuthStategiesOnClaimSetRequest")]
public class OverrideAuthStategiesOnClaimSetRequest
{
    [SwaggerSchema(Description = "ClaimSet id", Nullable = false)]
    public int ClaimSetId { get; set; }

    [SwaggerSchema(Description = "ResourceClaim id", Nullable = false)]
    public int ResourceClaimId { get; set; }

    [SwaggerSchema(Description = "AuthorizationStrategyForCreate id", Nullable = false)]
    public int AuthorizationStrategyForCreate { get; set; }

    [SwaggerSchema(Description = "AuthorizationStrategyForRead id", Nullable = false)]
    public int AuthorizationStrategyForRead { get; set; }

    [SwaggerSchema(Description = "AuthorizationStrategyForUpdate id", Nullable = false)]
    public int AuthorizationStrategyForUpdate { get; set; }

    [SwaggerSchema(Description = "AuthorizationStrategyForDelete id", Nullable = false)]
    public int AuthorizationStrategyForDelete { get; set; }
}
#endregion
