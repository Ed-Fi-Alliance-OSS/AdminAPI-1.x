// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public interface IDeleteResouceClaimOnClaimSetCommand
{
    void Execute(IClaimSetResourceClaimModel resourceClaimOnClaimSetModel);
}

public class DeleteResouceClaimOnClaimSetCommand : IDeleteResouceClaimOnClaimSetCommand
{
    private readonly ISecurityContext _context;

    public DeleteResouceClaimOnClaimSetCommand(ISecurityContext context)
    {
        _context = context;
    }

    public void Execute(IClaimSetResourceClaimModel resourceClaimOnClaimSetModel)
    {
        var resourceClaimsForClaimSetId =
                  _context.ClaimSetResourceClaimActions.Where(x => x.ClaimSetId == resourceClaimOnClaimSetModel.ClaimSetId && x.ResourceClaimId == resourceClaimOnClaimSetModel.ResourceClaimId).ToList();
        foreach (var resourceClaimAction in resourceClaimsForClaimSetId)
        {
            var resourceClaimActionAuthorizationStrategyOverrides = _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.
                Where(x => x.ClaimSetResourceClaimActionId == resourceClaimAction.ClaimSetResourceClaimActionId);

            _context.ClaimSetResourceClaimActionAuthorizationStrategyOverrides.RemoveRange(resourceClaimActionAuthorizationStrategyOverrides);
        }

        _context.ClaimSetResourceClaimActions.RemoveRange(resourceClaimsForClaimSetId);
        _context.SaveChanges();
    }
}

public interface IClaimSetResourceClaimModel
{
    int ClaimSetId { get; }
    int ResourceClaimId { get; }
}
