// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;

namespace EdFi.Ods.AdminApp.Management.Configuration.Claims
{
    public interface ICloudOdsClaimSetConfigurator
    {
        void ApplyConfiguration(CloudOdsClaimSet configuration);
    }

    public class CloudOdsClaimSetConfigurator : ICloudOdsClaimSetConfigurator
    {
        private readonly ISecurityContext _securityContext;

        public CloudOdsClaimSetConfigurator(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public void ApplyConfiguration(CloudOdsClaimSet configuration)
        {
            var claimSetExists = Queryable.Any(_securityContext.ClaimSets, cs => cs.ClaimSetName == configuration.ClaimSetName);
            if (claimSetExists)
                return;

            var resourceTypes = configuration.Claims.Select(c => c.EntityName);
            var actionNames = Enumerable.Distinct<string>(configuration.Claims.SelectMany(c => c.Actions).Select(x => x.ActionName));

            var apiApplication = Queryable.Single(_securityContext.Applications, a => a.ApplicationName == configuration.ApplicationName);
            var resourceClaims = _securityContext.ResourceClaims.Where(rc => Enumerable.Contains(resourceTypes, rc.ResourceName)).ToList();
            var actions = _securityContext.Actions.Where(a => actionNames.Contains(a.ActionName)).ToList();
            
            var claimSet = new ClaimSet
            {
                Application = apiApplication,
                ClaimSetName = configuration.ClaimSetName
            };

            _securityContext.ClaimSets.Add(claimSet);

            foreach (var requiredClaim in configuration.Claims)
            {
                var resourceClaim = resourceClaims.Single(rc => rc.ResourceName.Equals(requiredClaim.EntityName));
                var authOverride = requiredClaim.AuthorizationStrategy != null
                    ? Queryable.FirstOrDefault(_securityContext.AuthorizationStrategies, a =>
                        a.Application.ApplicationId == apiApplication.ApplicationId && 
                        a.AuthorizationStrategyName == requiredClaim.AuthorizationStrategy.StrategyName)
                    : null;

                var overrides = authOverride != null ? new List<ClaimSetResourceClaimActionAuthorizationStrategyOverrides> { new
                ClaimSetResourceClaimActionAuthorizationStrategyOverrides{ AuthorizationStrategy = authOverride } } : null;

                foreach (var claimSetResourceClaim in requiredClaim.Actions.Select(action =>                 
                new ClaimSetResourceClaimAction
                    {
                        Action = actions.Single(a => a.ActionName == action.ActionName),
                        AuthorizationStrategyOverrides = overrides,
                        ClaimSet = claimSet,
                        ResourceClaim = resourceClaim
                    }))
                {
                    _securityContext.ClaimSetResourceClaimActions.Add(claimSetResourceClaim);
                }
            }

            _securityContext.SaveChanges();
        }
    }
}
