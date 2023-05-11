// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class AddClaimSetCommand
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly AddClaimSetCommandV53Service _v53Service;
        private readonly AddClaimSetCommandV6Service _v6Service;

        public AddClaimSetCommand(IOdsSecurityModelVersionResolver resolver,
            AddClaimSetCommandV53Service v53Service,
            AddClaimSetCommandV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }

        public int Execute(IAddClaimSetModel claimSet)
        {
            var securityModel = _resolver.DetermineSecurityModel();
            if (securityModel == EdFiOdsSecurityModelCompatibility.ThreeThroughFive)
                return _v53Service.Execute(claimSet);
            else if (securityModel == EdFiOdsSecurityModelCompatibility.Six)
                return _v6Service.Execute(claimSet);
            else
                throw new EdFiOdsSecurityModelCompatibilityException(securityModel);
        }
    }

    public interface IAddClaimSetModel
    {
        string ClaimSetName { get; }
    }
}
