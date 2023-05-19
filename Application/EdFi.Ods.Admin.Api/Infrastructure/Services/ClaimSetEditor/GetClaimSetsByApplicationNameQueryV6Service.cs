// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class GetClaimSetsByApplicationNameQueryV6Service
    {
        private readonly ISecurityContext _securityContext;
        private readonly IUsersContext _usersContext;

        public GetClaimSetsByApplicationNameQueryV6Service(ISecurityContext securityContext, IUsersContext usersContext)
        {
            _securityContext = securityContext;
            _usersContext = usersContext;
        }

        public IEnumerable<ClaimSet> Execute(string applicationName)
        {
            var claimSets = GetClaimSetsByApplicationName(applicationName);

            SetApplicationCountPerClaimSet(claimSets);

            return claimSets;
        }

        private ClaimSet[] GetClaimSetsByApplicationName(string applicationName)
        {
            return _securityContext.ClaimSets
              .Where(x => !x.ForApplicationUseOnly && !CloudOdsAdminApp.SystemReservedClaimSets.Contains(x.ClaimSetName))
              .Where(x => x.Application.ApplicationName == applicationName)
              .OrderBy(x => x.ClaimSetName)
              .Select(x => new ClaimSet
              {
                  Id = x.ClaimSetId,
                  Name = x.ClaimSetName,
                  IsEditable = !x.IsEdfiPreset
              }).ToArray();
        }

        private void SetApplicationCountPerClaimSet(ClaimSet[] claimSets)
        {
            var claimSetsNames = claimSets.Select(x => x.Name);

            var applicationsCounts = _usersContext.Applications
                .Where(x => claimSetsNames.Contains(x.ClaimSetName))
                .GroupBy(x => x.ClaimSetName)
                .Select(x => new
                {
                    ClaimSetName = x.Key,
                    ApplicationsCount = x.Count()
                })
                .ToList();

            foreach (var claimSet in claimSets)
            {
                var applicationsCount = applicationsCounts
                    .SingleOrDefault(x => x.ClaimSetName == claimSet.Name)
                    ?.ApplicationsCount;
                claimSet.ApplicationsCount = applicationsCount.GetValueOrDefault();
            }
        }
    }
}
