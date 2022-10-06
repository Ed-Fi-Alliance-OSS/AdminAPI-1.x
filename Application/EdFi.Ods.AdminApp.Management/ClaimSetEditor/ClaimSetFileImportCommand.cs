// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class ClaimSetFileImportCommand
    {
        private readonly AddClaimSetCommand _addClaimSetCommand;
        private readonly GetResourceClaimsQuery _getResourceClaimsQuery;
        private readonly AddOrEditResourcesOnClaimSetCommand _addOrUpdateResourcesOnClaimSetCommand;

        public ClaimSetFileImportCommand(AddClaimSetCommand addClaimSetCommand,
            GetResourceClaimsQuery getResourceClaimsQuery,
            AddOrEditResourcesOnClaimSetCommand addOrUpdateResourcesOnClaimSetCommand)
        {
            _addClaimSetCommand = addClaimSetCommand;
            _getResourceClaimsQuery = getResourceClaimsQuery;
            _addOrUpdateResourcesOnClaimSetCommand = addOrUpdateResourcesOnClaimSetCommand;
        }

        public void Execute(SharingModel model)
        {
            var sharingClaimSets = model.Template.ClaimSets;

            foreach (var claimSet in sharingClaimSets)
            {
                var claimSetId = _addClaimSetCommand.Execute(new AddClaimSetModel
                {
                    ClaimSetName = claimSet.Name
                });
                var resources = claimSet.ResourceClaims.Select(x => x.ToObject<ResourceClaim>()).ToList();
                _addOrUpdateResourcesOnClaimSetCommand.Execute(claimSetId, resources);
            }
        }
    }
}
