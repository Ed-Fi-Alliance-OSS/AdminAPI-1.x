// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.SecurityCompatiblity53.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.Admin.Api.Infrastructure.ErrorHandling;

namespace EdFi.Ods.Admin.Api.Infrastructure.ClaimSetEditor;

public class EditClaimSetCommandV53Service
{
    private readonly ISecurityContext _securityContext;
    private readonly IUsersContext _usersContext;

    public EditClaimSetCommandV53Service(ISecurityContext securityContext, IUsersContext usersContext)
    {
        _securityContext = securityContext;
        _usersContext = usersContext;
    }

    public int Execute(IEditClaimSetModel claimSet)
    {
        var existingClaimSet = _securityContext.ClaimSets.Single(x => x.ClaimSetId == claimSet.ClaimSetId);

        if (CloudOdsAdminApp.DefaultClaimSets.Contains(existingClaimSet.ClaimSetName) ||
                    CloudOdsAdminApp.SystemReservedClaimSets.Contains(existingClaimSet.ClaimSetName))
        {
            throw new AdminAppException($"Claim set ({existingClaimSet.ClaimSetName}) is system reserved.May not be modified.");
        }

        if (claimSet.ClaimSetName is null) throw new InvalidOperationException("Cannot have a null ClaimSetName");
        if (!claimSet.ClaimSetName.Equals(existingClaimSet.ClaimSetName, StringComparison.InvariantCultureIgnoreCase))
        {
            ReAssociateApplicationsToRenamedClaimSet(existingClaimSet.ClaimSetName, claimSet.ClaimSetName);
        }

        existingClaimSet.ClaimSetName = claimSet.ClaimSetName;

        void ReAssociateApplicationsToRenamedClaimSet(string existingClaimSetName, string newClaimSetName)
        {
            var associatedApplications = _usersContext.Applications
                .Where(x => x.ClaimSetName == existingClaimSetName);

            foreach (var application in associatedApplications)
            {
                application.ClaimSetName = newClaimSetName;
            }
        }

        _securityContext.SaveChanges();
        _usersContext.SaveChanges();

        return existingClaimSet.ClaimSetId;
    }
}
