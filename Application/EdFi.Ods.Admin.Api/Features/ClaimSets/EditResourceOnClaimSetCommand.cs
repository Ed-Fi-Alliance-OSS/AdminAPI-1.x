// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class EditResourceOnClaimSetCommand
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly EditResourceOnClaimSetCommandV53Service _v53Service;
        private readonly EditResourceOnClaimSetCommandV6Service _v6Service;

        public EditResourceOnClaimSetCommand(IOdsSecurityModelVersionResolver resolver,
            EditResourceOnClaimSetCommandV53Service v53Service,
            EditResourceOnClaimSetCommandV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }
        
        public void Execute(IEditResourceOnClaimSetModel model)
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

    public interface IEditResourceOnClaimSetModel
    {
        int ClaimSetId { get; }
        ResourceClaim ResourceClaim { get; }
    }
}
