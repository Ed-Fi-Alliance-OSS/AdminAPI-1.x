// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class UpdateResourcesOnClaimSetCommand
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly UpdateResourcesOnClaimSetCommandV53Service _v53Service;
        private readonly UpdateResourcesOnClaimSetCommandV6Service _v6Service;

        public UpdateResourcesOnClaimSetCommand(IOdsSecurityModelVersionResolver resolver,
            UpdateResourcesOnClaimSetCommandV53Service v53Service,
            UpdateResourcesOnClaimSetCommandV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }

        public void Execute(IUpdateResourcesOnClaimSetModel model)
        {
            var securityModel = _resolver.DetermineSecurityModel();
            if (securityModel == EdFiOdsSecurityModelCompatibility.ThreeThroughFive)
                _v53Service.Execute(model);
            else if (securityModel == EdFiOdsSecurityModelCompatibility.Six)
                _v6Service.Execute(model);
            else
                throw new EdFiOdsSecurityModelCompatibilityException(securityModel);
        }
    }

    public interface IUpdateResourcesOnClaimSetModel
    {
        int ClaimSetId { get; }
        List<ResourceClaim> ResourceClaims { get; }
    }
}
