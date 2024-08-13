// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using EdFi.Security.DataAccess.Contexts;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetResourceClaimsQuery
{
    IEnumerable<ResourceClaim> Execute();
    IEnumerable<ResourceClaim> Execute(CommonQueryParams commonQueryParams, int? id, string? name);
}

public class GetResourceClaimsQuery : IGetResourceClaimsQuery
{
    private readonly ISecurityContext _securityContext;
    private readonly IOptions<AppSettings> _options;

    public GetResourceClaimsQuery(ISecurityContext securityContext, IOptions<AppSettings> options)
    {
        _securityContext = securityContext;
        _options = options;
    }

    public IEnumerable<ResourceClaim> Execute()
    {
        return Query().ToList();
    }

    public IEnumerable<ResourceClaim> Execute(CommonQueryParams commonQueryParams, int? id, string? name)
    {
        return Query()
            .Where(c => id == null || c.Id == id)
            .Where(c => name == null || c.Name == name)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }

    private IEnumerable<ResourceClaim> Query()
    {
        var resources = new List<ResourceClaim>();
        var parentResources = _securityContext.ResourceClaims.Where(x => x.ParentResourceClaim == null).ToList();
        var childResources = _securityContext.ResourceClaims.Where(x => x.ParentResourceClaim != null).ToList();
        foreach (var parentResource in parentResources)
        {
            var children = childResources.Where(x => x.ParentResourceClaimId == parentResource.ResourceClaimId);
            resources.Add(new ResourceClaim
            {
                Children = children.Select(child => new ResourceClaim()
                {
                    Id = child.ResourceClaimId,
                    Name = child.ResourceName,
                    ParentId = parentResource.ResourceClaimId,
                    ParentName = parentResource.ResourceName,
                }).ToList(),
                Name = parentResource.ResourceName,
                Id = parentResource.ResourceClaimId
            });
        }
        return resources
            .Distinct()
            .OrderBy(x => x.Name);
    }
}
