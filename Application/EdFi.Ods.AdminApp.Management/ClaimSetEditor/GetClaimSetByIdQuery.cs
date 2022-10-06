// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Net;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EdFi.Security.DataAccess.Contexts;
using static EdFi.Ods.AdminApp.Management.ClaimSetEditor.GetClaimSetsByApplicationNameQuery;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class GetClaimSetByIdQuery : IGetClaimSetByIdQuery
    {
        private readonly ISecurityContext _securityContext;

        public GetClaimSetByIdQuery(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public ClaimSet Execute(int claimSetId)
        {
            var securityContextClaimSet = _securityContext.ClaimSets
                .SingleOrDefault(x => x.ClaimSetId == claimSetId);

            if (securityContextClaimSet != null)
            {
                return new ClaimSet
                {
                    Id = securityContextClaimSet.ClaimSetId,
                    Name = securityContextClaimSet.ClaimSetName,
                    IsEditable = !DefaultClaimSets.Contains(securityContextClaimSet.ClaimSetName)
                };
            }

            throw new AdminAppException("No such claim set exists in the database.")
            {
                StatusCode = HttpStatusCode.NotFound
            };
        }
    }

    public interface IGetClaimSetByIdQuery
    {
        ClaimSet Execute(int securityContextClaimSetId);
    }
}
