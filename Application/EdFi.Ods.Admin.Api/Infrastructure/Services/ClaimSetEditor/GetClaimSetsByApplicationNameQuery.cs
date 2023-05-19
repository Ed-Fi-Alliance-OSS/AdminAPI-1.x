// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class GetClaimSetsByApplicationNameQuery : IGetClaimSetsByApplicationNameQuery
    {
        private readonly IOdsSecurityModelVersionResolver _resolver;
        private readonly GetClaimSetsByApplicationNameQueryV53Service _v53Service;
        private readonly GetClaimSetsByApplicationNameQueryV6Service _v6Service;

        public GetClaimSetsByApplicationNameQuery(IOdsSecurityModelVersionResolver resolver,
            GetClaimSetsByApplicationNameQueryV53Service v53Service,
            GetClaimSetsByApplicationNameQueryV6Service v6Service)
        {
            _resolver = resolver;
            _v53Service = v53Service;
            _v6Service = v6Service;
        }

        public IEnumerable<ClaimSet> Execute(string applicationName)
        {
            var securityModel = _resolver.DetermineSecurityModel();
            if (securityModel == EdFiOdsSecurityModelCompatibility.ThreeThroughFive)
                return _v53Service.Execute(applicationName);
            else if (securityModel == EdFiOdsSecurityModelCompatibility.Six)
                return _v6Service.Execute(applicationName);
            else
                throw new EdFiOdsSecurityModelCompatibilityException(securityModel);
        }
    }

    public interface IGetClaimSetsByApplicationNameQuery
    {
        IEnumerable<ClaimSet> Execute(string securityContextApplicationName);
    }
}
