// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class GetResourcesByClaimSetIdQuery : IGetResourcesByClaimSetIdQuery
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly GetResourcesByClaimSetIdQueryV53Service _v53Service;
        private readonly GetResourcesByClaimSetIdQueryV6Service _v6Service;

        public GetResourcesByClaimSetIdQuery(IOdsSecurityModelVersionResolver resolver,
            GetResourcesByClaimSetIdQueryV53Service v53Service,
            GetResourcesByClaimSetIdQueryV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }

        public IEnumerable<ResourceClaim> AllResources(int claimSetId)
        {
            List<ResourceClaim> parentResources;
            var securityModel = _resolver.DetermineSecurityModel();
            if (securityModel == EdFiOdsSecurityModelCompatibility.ThreeThroughFive)
            {
                parentResources = _v53Service.GetParentResources(claimSetId);
                var childResources = _v53Service.GetChildResources(claimSetId);
                _v53Service.AddChildResourcesToParents(childResources, parentResources);
            }
            else if (securityModel == EdFiOdsSecurityModelCompatibility.Six)
            {
                parentResources = _v6Service.GetParentResources(claimSetId);
                var childResources = _v6Service.GetChildResources(claimSetId);
                _v6Service.AddChildResourcesToParents(childResources, parentResources);
            }
            else
                throw new EdFiOdsSecurityModelCompatibilityException(securityModel);    

            return parentResources;
        }

        public ResourceClaim SingleResource(int claimSetId, int resourceClaimId)
        {
            var parentResources = AllResources(claimSetId).ToList();
            var parentResourceClaim = parentResources
                .SingleOrDefault(x => x.Id == resourceClaimId);
            var childResources = new List<ResourceClaim>();
            if (parentResourceClaim == null)
            {
                foreach (var resourceClaims in parentResources.Select(x => x.Children)) childResources.AddRange(resourceClaims);
                return childResources.SingleOrDefault(x => x.Id == resourceClaimId);
            }

            return parentResourceClaim;
        }
    }

    public interface IGetResourcesByClaimSetIdQuery
    {
        IEnumerable<ResourceClaim> AllResources(int securityContextClaimSetId);
        ResourceClaim SingleResource(int claimSetId, int resourceClaimId);
    }
}
