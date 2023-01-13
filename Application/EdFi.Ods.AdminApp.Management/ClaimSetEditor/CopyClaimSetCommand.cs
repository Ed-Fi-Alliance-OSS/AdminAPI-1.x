// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public interface ICopyClaimSetCommand
    {
        int Execute(ICopyClaimSetModel claimSet);
    }

    public class CopyClaimSetCommand : ICopyClaimSetCommand
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly CopyClaimSetCommandV53Service _v53Service;
        private readonly CopyClaimSetCommandV6Service _v6Service;

        public CopyClaimSetCommand(IOdsSecurityModelVersionResolver resolver,
            CopyClaimSetCommandV53Service v53Service,
            CopyClaimSetCommandV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }

        public int Execute(ICopyClaimSetModel claimSet)
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

    public interface ICopyClaimSetModel
    {
        string Name { get; }
        int OriginalId { get; }
    }
}
