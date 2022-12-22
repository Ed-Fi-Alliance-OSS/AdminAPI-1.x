// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using EdFi.Security.DataAccess.Contexts;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class UpdateResourcesOnClaimSetCommandV6Service
    {
        private readonly ISecurityContext _context;
        private readonly AddOrEditResourcesOnClaimSetCommand _addOrEditResourcesOnClaimSetCommand;

        public UpdateResourcesOnClaimSetCommandV6Service(ISecurityContext context,
            AddOrEditResourcesOnClaimSetCommand addOrEditResourcesOnClaimSetCommand)
        {
            _context = context;
            _addOrEditResourcesOnClaimSetCommand = addOrEditResourcesOnClaimSetCommand;
        }

        public void Execute(IUpdateResourcesOnClaimSetModel model)
        {
            var resourceClaimsForClaimSet =
                _context.ClaimSetResourceClaimActions.Where(x => x.ClaimSet.ClaimSetId == model.ClaimSetId).ToList();
            _context.ClaimSetResourceClaimActions.RemoveRange(resourceClaimsForClaimSet);
            _context.SaveChanges();

            _addOrEditResourcesOnClaimSetCommand.Execute(model.ClaimSetId, model.ResourceClaims);
        }
    }
}
