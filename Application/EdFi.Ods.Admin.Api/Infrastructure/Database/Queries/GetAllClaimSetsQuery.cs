// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using ClaimSet = EdFi.Ods.AdminApp.Management.ClaimSetEditor.ClaimSet;

namespace EdFi.Ods.AdminApp.Management.Database.Queries
{
    public interface IGetAllClaimSetsQuery
    {
        IEnumerable<ClaimSet> Execute();
    }

    public class GetAllClaimSetsQuery : IGetAllClaimSetsQuery
    {
        private readonly ISecurityContext _securityContext;

        public GetAllClaimSetsQuery(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public IEnumerable<ClaimSet> Execute()
        {
            return _securityContext.ClaimSets
                .Select(x => new ClaimSet
                {
                    Id = x.ClaimSetId,
                    Name = x.ClaimSetName
                })
                .Distinct()
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
