using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.Ods.AdminApp.Management.Database.Queries;

public class GetResourceClaims53Query : IGetResourceClaimsQuery
{
    private readonly ISecurityContext _securityContext;

    public GetResourceClaims53Query(ISecurityContext securityContext)
    {
        _securityContext = securityContext;
    }

    public IEnumerable<ResourceClaim> Execute()
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
            .OrderBy(x => x.Name)
            .ToList();
    }
}
