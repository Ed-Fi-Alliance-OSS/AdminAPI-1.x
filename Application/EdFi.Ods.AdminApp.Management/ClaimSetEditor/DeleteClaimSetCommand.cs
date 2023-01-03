// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public interface IDeleteClaimSetCommand
    {
        void Execute(IDeleteClaimSetModel claimSet);
    }

    public class DeleteClaimSetCommand : IDeleteClaimSetCommand
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly DeleteClaimSetCommandV53Service _v53Service;
        private readonly DeleteClaimSetCommandV6Service _v6Service;

        public DeleteClaimSetCommand(IOdsSecurityModelVersionResolver resolver,
            DeleteClaimSetCommandV53Service v53Service,
            DeleteClaimSetCommandV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }

        public void Execute(IDeleteClaimSetModel claimSet)
        {
            var securityModel = _resolver.DetermineSecurityModel();
            if (securityModel == EdFiOdsSecurityModelCompatibility.ThreeThroughFive)
                _v53Service.Execute(claimSet);
            else if (securityModel == EdFiOdsSecurityModelCompatibility.Six)
                _v6Service.Execute(claimSet);
            else
                throw new EdFiOdsSecurityModelCompatibilityException(securityModel);
        }
    }

    public interface IDeleteClaimSetModel
    {
        string Name { get; }
        int Id { get; }
    }
}
